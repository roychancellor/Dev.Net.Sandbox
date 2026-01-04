using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.Net.Http;
using DotNetCoreWebApi.Providers;
using Microsoft.Extensions.Options;
using NLog;

namespace DotNetCoreWebApi.Logic
{
    public class RequestLogic : IRequestLogic
    {
        private static NLog.ILogger _reqLog = LogManager.GetLogger("RequestLogger");

        private readonly IOptionsMonitor<AppSettings> _appSettingsMonitor;
        private AppSettings? _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string X_API_KEY = "X-Api-Key";
        private readonly HttpClient _httpClient;

        public RequestLogic(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IOptionsMonitor<AppSettings> appSettingsMonitor)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClientFactory.CreateClient();
            _appSettingsMonitor = appSettingsMonitor;
            Init();
        }

        private void Init()
        {
            string msg;
            _reqLog.Trace("RequestLogic: Initializing");
            _appSettings = _appSettingsMonitor.CurrentValue;
            if (_appSettings == null)
            {
                msg = "Injected app settings object is null";
                _reqLog.Error(msg);
                throw new Exception(msg);
            }
            if (!_appSettings.IsValid())
            {
                msg = "One or more properties of AppSettings is null or empty.";
                _reqLog.Error(msg);
                throw new Exception(msg);
            }
        }

        public async Task<HandleResponse> HandleRequest()
        {
            _reqLog.Debug($"Handling request");
            Init();
            
            string msg;
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                msg = "RequestLogic: HttpContext is null";
                _reqLog.Error(msg);
                throw new Exception(msg);
            }
            // Get the API Key from the header
            _reqLog.Trace("Getting API key from request header");
            var aPIKey = httpContext?.Request.Headers.FirstOrDefault(h => h.Key.Equals(X_API_KEY)).Value;
            if (string.IsNullOrEmpty(aPIKey))
            {
                throw new BadRequestException($"The API key must be present in the {X_API_KEY} header");
            }

            _reqLog.Debug($"Inbound IP Address: {httpContext?.Connection.RemoteIpAddress} | {X_API_KEY} Header: {aPIKey}");

            // Get the XML from the Request Body as a string
            _reqLog.Trace("Getting raw XML from request body");
            string requestBody;
            using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            if (string.IsNullOrEmpty(requestBody))
            {
                throw new BadRequestException("Request body is required");
            }

            // Put the request into XDocument
            _reqLog.Trace($"Creating XDocument from XML string\n{requestBody}");
            XDocument xDoc;
            try
            {
                xDoc = XDocument.Parse(requestBody);
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Unable to parse request body as XML\n{ex.Message}");
            }

            // Extract the URL string using XPath
            _reqLog.Trace("Extracting 'URL' element using XPath");
            var nsManager = new XmlNamespaceManager(new NameTable());
            nsManager.AddNamespace(_appSettings?.XpathNamespacePrefix ?? "rw", _appSettings?.XpathNamespace ?? "");
            var xPath = _appSettings?.XpathUrl;
            var uRLNode = xDoc.XPathSelectElement(xPath ?? "", nsManager) ?? throw new BadRequestException($"Bad XML - Unable to extract the URL element");
            string URL = uRLNode.Value;
            if (string.IsNullOrEmpty(URL))
            {
                throw new BadRequestException($"The URL element must be present in XML");
            }

            _reqLog.Trace($"Extracted URL: {URL}");
            _reqLog.Debug("Request handled");

            return new HandleResponse { ApiKey = aPIKey, UrlToCall = URL };
        }

        public async Task<string> ProcessRequest(HandleResponse handleResponse)
        {
            _reqLog.Debug("Processing request");
            
            _reqLog.Trace("Calling API");
            string aPIResponseBody;
            string msg;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, handleResponse.UrlToCall);
                request.Headers.Add(X_API_KEY, handleResponse.ApiKey);
                var response = await _httpClient.SendAsync(request);

                // Check if the response is successful (status code in the range 200-299)
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    aPIResponseBody = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Return error message if the API call was not successful
                    msg = $"API request failed with status code: {response.StatusCode}";
                    _reqLog.Error(msg);
                    throw new BadRequestException(msg);
                }
            }
            catch (HttpRequestException ex)
            {
                msg = $"An error occurred while making the API request: {ex.Message}";
                _reqLog.Error(msg);
                throw new ServiceUnavailableException(msg);
            }

            _reqLog.Debug("Request processed");
            
            return aPIResponseBody;
        }
    }
}
