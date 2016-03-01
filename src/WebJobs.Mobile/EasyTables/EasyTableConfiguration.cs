// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace WebJobs.Extensions.EasyTables
{
    /// <summary>
    /// Defines the configuration options for the EasyTable binding.
    /// </summary>
    public class EasyTableConfiguration
    {
        internal const string AzureWebJobsMobileAppUriName = "AzureWebJobsMobileAppUri";

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public EasyTableConfiguration()
        {
            this.MobileAppUri = ConfigurationManager.AppSettings.Get(AzureWebJobsMobileAppUriName);
        }

        /// <summary>
        /// Gets or sets the Mobile App URI for the Easy Table.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings",
            Justification = "The Uri will be validated later. It is easier to allow strings here.")]
        public string MobileAppUri { get; set; }
    }
}