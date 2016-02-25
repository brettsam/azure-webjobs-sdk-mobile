using System.Configuration;

namespace WebJobs.Extensions.EasyTables
{
    /// <summary>
    ///
    /// </summary>
    public class EasyTableConfiguration
    {
        internal const string AzureWebJobsEasyTableUriName = "AzureWebJobsEasyTableUri";

        /// <summary>
        ///
        /// </summary>
        public EasyTableConfiguration()
        {
            this.EasyTableUri = ConfigurationManager.AppSettings.Get(AzureWebJobsEasyTableUriName);
        }

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string EasyTableUri { get; set; }
    }
}