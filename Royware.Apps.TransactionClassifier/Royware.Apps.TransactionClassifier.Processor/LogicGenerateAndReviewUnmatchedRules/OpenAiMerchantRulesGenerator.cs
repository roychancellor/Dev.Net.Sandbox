using Microsoft.Extensions.Options;
using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.LogicGenerateUnmatchedRules;
using Royware.Apps.TransactionClassifier.Processor.Models;
using Royware.Apps.TransactionClassifier.Providers.ApplicationSettings;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Royware.Apps.TransactionClassifier.Processor.LogicGenerateAndReviewUnmatchedRules
{
    public class OpenAiMerchantRulesGenerator : IMerchantRulesGeneration
    {
        private static readonly Logger _log = Loggers.Batch;

        private static readonly JsonSerializerOptions _propertyNameCaseInsensitiveCamelCase = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private readonly IOptionsMonitor<AppSettings> _appSettings;
        private readonly HttpClient _client;
        private readonly string _aiApiKey;

        public OpenAiMerchantRulesGenerator(IOptionsMonitor<AppSettings> appSettings, IHttpClientFactory clientFactory)
        {
            _appSettings = appSettings;
            _client = clientFactory.CreateClient();
            _client.Timeout = TimeSpan.FromSeconds(120);
            _aiApiKey = Environment.GetEnvironmentVariable("OPEN_API_KEY") ?? throw new ApplicationException($"UNABLE TO RETRIEVE OPEN_API_KEY");
        }

        public string PrepareAIRequestPayload(List<Transaction> batchTransactions, List<Category> categories)
        {
            if (batchTransactions.Count == 0 || categories.Count == 0)
            {
                return string.Empty;
            }

            var payload = new
            {
                knownCategories = categories.Select(c => c.CategoryName).ToList(),
                transactions = batchTransactions
                              .Where(t => t.IsProcessable() && !t.IsResolved)
                              .Select(t => new
                              {
                                  t.TransactionId,
                                  t.Description, // normalized
                                  t.Domain,
                                  t.AccountType,
                                  t.Amount
                              })
                              .ToList()
            };

            var payloadJson = JsonSerializer.Serialize(payload);

            var systemPrompt =
        """
You are a transaction normalization assistant.

Your job is to propose canonical merchant rules for future automatic categorization.

Rules:
- Use ONLY the provided knownCategories
- Do NOT invent categories
- RequiredTerms must uniquely identify the merchant
- ExcludedTerms must prevent false positives
- Confidence is optional (0-1)
- Return ONLY valid JSON
- Use Domain and AccountType to disambiguate merchants and categories.
""";

            var schemaToReturn = $@"[
  {{
    ""TransactionId"": 123456,
    ""NormalizedMerchant"": """",
    ""Category"": """",
    ""RequiredTerms"": [],
    ""ExcludedTerms"": [],
    ""Confidence"": 0.0,
    ""Notes"": """"
  }}
]";
            var userPrompt = $"""
For each transaction, suggest:

1. NormalizedMerchant
2. Category (must be one of knownCategories)
3. RequiredTerms
4. ExcludedTerms
5. Confidence (0-1, optional)
6. Notes (optional)

Return JSON using this exact schema:

{schemaToReturn}

Payload:
{payloadJson}
""";

            var request = new OpenAiResponseRequest
            {
                Model = _appSettings.CurrentValue.AiModel,
                Temperature = 0,
                MaxOutputTokens = _appSettings.CurrentValue.AiMaxOutputTokens,
                Input =
                {
                    new InputItem
                    {
                        Role = "system",
                        Content =
                        {
                            new InputContent
                            {
                                Text = systemPrompt
                            }
                        }
                    },
                    new InputItem
                    {
                        Role = "user",
                        Content =
                        {
                            new InputContent
                            {
                                Text = userPrompt
                            }
                        }
                    }
                }
            };


            var requestJson = JsonSerializer.Serialize(request, _propertyNameCaseInsensitiveCamelCase);
            return requestJson;
        }

        public async Task<List<MerchantRuleProposal>> GetMerchantRuleProposalsAsync(string requestAsJson,
                                                                                    CancellationToken cancellationToken = default)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, _appSettings.CurrentValue.AiRequestUrl)
            {
                Content = new StringContent(requestAsJson, Encoding.UTF8, "application/json")
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _aiApiKey);

            var response = await _client.SendAsync(requestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"OpenAI call failed ({response.StatusCode}): {errorBody}");
            }

            var responseAsJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var cleanModelText = ExtractCleanAiResponseModelText(responseAsJson);

            var proposedMerchantRules = JsonSerializer.Deserialize<List<MerchantRuleProposal>>(cleanModelText, _propertyNameCaseInsensitiveCamelCase);

            return proposedMerchantRules ?? [];
        }

        public void AssignCorrelations(List<MerchantRuleProposal> toAssign)
        {
            foreach (var mrp in toAssign)
            {
                mrp.MerchantRuleCorrelation = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            }
        }

        public List<MerchantRule> HumanReview(List<MerchantRuleProposal> proposedRules, FileMetaData fileMeta, List<Transaction> currentBatch)
        {
            if (proposedRules.Count == 0)
            {
                return [];
            }

            Console.WriteLine($"For each rule, choose to [A]ccept, [R]eject, or [E]dit");
            Console.WriteLine($"TransactionId\tNormalized Merchant\tCategory\tRequired Terms\tExcluded Terms\tConfidence\tNotes\tActions");
            _log.Info($"Starting human review of AI-generated rules");

            var toReturn = new List<MerchantRule>();
            foreach (var pr in proposedRules)
            {
                var associatedTrans = currentBatch.Where(tx => tx.TransactionId == pr.TransactionId).FirstOrDefault();
                if (associatedTrans == null)
                {
                    var errMsg = $"During human review, there is no matching transaction for the proposed merchant rule | PROPOSED RULE TRANS ID: {pr.TransactionId}";
                    _log.Error(errMsg);
                    throw new Exception(errMsg);
                }

                var shouldPromptUserForCurrentRule = true;
                while (shouldPromptUserForCurrentRule)
                {
                    _log.Info($"{pr}");
                    Console.Write($"{pr} | CHOICE (A/R/E): ");
                    var choice = Console.ReadKey();

                    if (choice.KeyChar == 'R' || choice.KeyChar == 'r')
                    {
                        Console.WriteLine($"PRESS Y TO CONFIRM REJECT OR N TO GO BACK AND CHOOSE A DIFFERENT OPTION.");
                        var confirmRejectChoice = Console.ReadKey();
                        if (confirmRejectChoice.KeyChar == 'y' || confirmRejectChoice.KeyChar == 'Y')
                        {
                            _log.Info($"TRANS ID: {pr.TransactionId} | CHOICE: Reject");
                            shouldPromptUserForCurrentRule = false;
                        }
                    }
                    else if (choice.KeyChar == 'E' || choice.KeyChar == 'e')
                    {
                        _log.Info($"TRANS ID: {pr.TransactionId} | CHOICE: Edit");
                        var prToEdit = pr.Clone();
                        Edit(prToEdit);
                        BuildMerchantRule(fileMeta, toReturn, associatedTrans, prToEdit);
                        shouldPromptUserForCurrentRule = false;
                    }
                    else if (choice.KeyChar == 'a' || choice.KeyChar == 'A')
                    {
                        _log.Info($"TRANS ID: {pr.TransactionId} | CHOICE: Accept");
                        BuildMerchantRule(fileMeta, toReturn, associatedTrans, pr);
                        shouldPromptUserForCurrentRule = false;
                    }
                    else
                    {
                        Console.WriteLine($"Valid choices are A, R, or E");
                    }
                }
            }
            return toReturn;
        }

        private static void BuildMerchantRule(FileMetaData fileMeta, List<MerchantRule> toReturn, Transaction associatedTrans, MerchantRuleProposal crToEdit)
        {
            var mrToAdd = MerchantRule.MappedFrom(crToEdit, fileMeta);
            toReturn.Add(mrToAdd);
            Console.WriteLine();
        }

        private static string ExtractCleanAiResponseModelText(string responseJson)
        {
            var doc = JsonDocument.Parse(responseJson);
            string modelText = doc.RootElement
                               .GetProperty("output")
                               .EnumerateArray()
                               .SelectMany(o => o.GetProperty("content").EnumerateArray())
                               .Where(c => c.GetProperty("type").GetString() == "output_text")
                               .Select(c => c.GetProperty("text").GetString())
                               .FirstOrDefault()
                            ?? throw new InvalidOperationException("No output_text returned by model");

            var cleanModelText = CleanModelText(modelText);

            return cleanModelText;
        }

        private static string CleanModelText(string modelText)
        {
            // Trim whitespace
            modelText = modelText.Trim();

            // Remove triple backticks and 'json' marker
            if (modelText.StartsWith("```"))
            {
                int firstNewline = modelText.IndexOf('\n');
                int lastFence = modelText.LastIndexOf("```");
                if (firstNewline > 0 && lastFence > firstNewline)
                {
                    modelText = modelText.Substring(firstNewline + 1, lastFence - firstNewline - 1);
                }
            }

            // Remove any single backticks
            modelText = modelText.Trim('`').Trim();

            // Optional: extract from first '[' to last ']' to ensure valid JSON
            int start = modelText.IndexOf('[');
            int end = modelText.LastIndexOf(']');
            if (start >= 0 && end > start)
            {
                modelText = modelText[start..(end + 1)];
            }

            return modelText;
        }

        private static void Edit(MerchantRuleProposal mrp)
        {
            var shouldContinueEditing = true;
            while (shouldContinueEditing)
            {
                Console.WriteLine($"Press the corresponding key to edit a field");
                Console.WriteLine($"[M] - Normalized Merchant | [C] - Category | [R] - Required Terms | [E] - Excluded Terms | [F] - Confidence | [N] - Notes | [X] - EXIT EDITING");
                var choice = Console.ReadKey().KeyChar;

                if (choice == 'm' || choice == 'M')
                {
                    mrp.NormalizedMerchant = GetNewStringValue("merchant name", mrp.NormalizedMerchant);
                }
                else if (choice == 'c' || choice == 'C')
                {
                    mrp.Category = GetNewStringValue("category", mrp.Category);
                }
                else if (choice == 'r' || choice == 'R')
                {
                    mrp.RequiredTerms = GetNewStringValues("required terms", mrp.RequiredTerms);
                }
                else if (choice == 'e' || choice == 'E')
                {
                    mrp.ExcludedTerms = GetNewStringValues("excluded terms", mrp.ExcludedTerms);
                }
                else if (choice == 'f' || choice == 'F')
                {
                    mrp.Confidence = GetNewDecimalValue("confidence", mrp.Confidence);
                }
                else if (choice == 'n' || choice == 'N')
                {
                    mrp.Notes = GetNewStringValue("notes", mrp.Notes);
                }
                else if (choice == 'x' || choice == 'X')
                {
                    shouldContinueEditing = false;
                }
                else
                {
                    Console.WriteLine($"INVALID CHOICE. TRY AGAIN.");
                }
                /*
                 * NormalizedMerchant = NormalizedMerchant,
                Category = Category,
                RequiredTerms = RequiredTerms,
                ExcludedTerms = ExcludedTerms,
                Confidence = Confidence,
                Notes = Notes
                 */
            }
        }

        private static string GetNewStringValue(string prompt, string? valueToEdit)
        {
            var isValid = false;
            string newValue = string.Empty;
            valueToEdit = string.IsNullOrWhiteSpace(valueToEdit) ? "[EMPTY]" : valueToEdit;
            while (!isValid)
            {
                Console.Write($"Type in a new {prompt} and press Enter | EXISTING: {valueToEdit} | NEW: ");
                var userEntry = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userEntry))
                {
                    Console.Write($"New value must not be whitespace. Try again.");
                }
                else
                {
                    newValue = userEntry;
                    isValid = true;
                }
            }
            return newValue;
        }

        private static decimal GetNewDecimalValue(string prompt, decimal? valueToEdit)
        {
            var isValid = false;
            decimal newValue = 0;
            while (!isValid)
            {
                Console.Write($"Type in a new {prompt} and press Enter | EXISTING: {valueToEdit} | NEW: ");
                var userEntry = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userEntry))
                {
                    Console.Write($"New value must not be whitespace. Try again.");
                }
                else
                {
                    newValue = decimal.TryParse(userEntry, out newValue) ? newValue : 0;
                    isValid = true;
                }
            }
            return newValue;
        }

        private static List<string> GetNewStringValues(string prompt, List<string>? valuesToEdit)
        {
            var isValid = false;
            var finishedEditing = false;
            var newValues = new List<string>();
            valuesToEdit = valuesToEdit == null || valuesToEdit.Count == 0 ? ["[EMPTY]"] : valuesToEdit;
            while (!isValid && !finishedEditing)
            {
                Console.Write($"Type in the number next to the {prompt} value to edit and press Enter.");
                for (int i = 0; i < valuesToEdit.Count; i++)
                {
                    Console.WriteLine($"[{i + 1}] - {valuesToEdit[i]}");
                    newValues.Add(valuesToEdit[i]);
                }
                Console.WriteLine($"[X] - EXIT EDITING THIS LIST");
                var userEntry = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userEntry))
                {
                    Console.Write($"New value must not be whitespace. Try again.");
                }
                else if (userEntry.Equals("x") || userEntry.Equals("X"))
                {
                    finishedEditing = true;
                }
                else
                {
                    int userChoiceInt = int.TryParse(userEntry, out userChoiceInt) ? userChoiceInt : -1;
                    if (userChoiceInt < 1 || userChoiceInt > newValues.Count)
                    {
                        Console.WriteLine($"{userEntry} is invalid. Try again.");
                        isValid = false;
                        continue;
                    }
                    newValues[userChoiceInt] = userEntry;
                }
            }
            return newValues;
        }
    }
}
