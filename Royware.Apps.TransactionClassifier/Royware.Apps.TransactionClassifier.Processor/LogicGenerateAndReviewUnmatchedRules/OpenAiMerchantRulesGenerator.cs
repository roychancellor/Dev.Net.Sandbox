using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
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
        private static readonly JsonSerializerOptions _propertyNameCaseInsensitiveCamelCase = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private readonly IOptionsMonitor<AppSettings> _appSettings;
        private readonly HttpClient _client;

        public OpenAiMerchantRulesGenerator(IOptionsMonitor<AppSettings> appSettings, IHttpClientFactory clientFactory)
        {
            _appSettings = appSettings;
            _client = clientFactory.CreateClient();
            _client.Timeout = TimeSpan.FromSeconds(120);

        }

        public List<MerchantRule> CallAIForCandidateRules(object payload)
        {
            throw new NotImplementedException();
        }

        public List<MerchantRule> HumanReview(List<MerchantRuleProposal> candidateRules)
        {
            throw new NotImplementedException();
        }

        public object PrepareAIPayload(List<Transaction> unmatched, List<string> knownCategories)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MerchantRuleProposal>> GetMerchantRuleProposalsAsync(List<Transaction> batchTransactions,
                                                                                    List<Category> categories,
                                                                                    CancellationToken cancellationToken = default)
        {
            if (batchTransactions.Count == 0)
            {
                return [];
            }
            
            // 1. Build payload
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

            // 2. Messages
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

            // 3. HTTP call
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, _appSettings.CurrentValue.AiRequestUrl)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _appSettings.CurrentValue.AiApiKey);

            var response = await _client.SendAsync(requestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"OpenAI call failed ({response.StatusCode}): {errorBody}");
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            // 4. Extract model output

            using var doc = JsonDocument.Parse(responseJson);

            string modelText = doc.RootElement
                               .GetProperty("output")
                               .EnumerateArray()
                               .SelectMany(o => o.GetProperty("content").EnumerateArray())
                               .Where(c => c.GetProperty("type").GetString() == "output_text")
                               .Select(c => c.GetProperty("text").GetString())
                               .FirstOrDefault()
                            ?? throw new InvalidOperationException("No output_text returned by model");

            var cleanModelText = CleanModelText(modelText);

            // THIS string has real newlines, not \n
            var proposals = JsonSerializer.Deserialize<List<MerchantRuleProposal>>(cleanModelText, _propertyNameCaseInsensitiveCamelCase);
            
            return proposals ?? [];
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
    }
}
