namespace DotNetCoreWebApi.Providers
{
    public class AppSettings
    {
        public string? ApplicationName { get; set; }
        public string? ServiceName { get; set; }
        public string? XpathNamespace { get; set; }
        public string? XpathNamespacePrefix { get; set; }
        public string? XpathUrl { get; set; }

        public bool IsValid()
        {
            return !(
                        string.IsNullOrEmpty(ApplicationName) ||
                        string.IsNullOrEmpty(ServiceName) ||
                        string.IsNullOrEmpty(XpathNamespace) ||
                        string.IsNullOrEmpty(XpathNamespacePrefix) ||
                        string.IsNullOrEmpty(XpathUrl)
                    );
        }
    }
}
