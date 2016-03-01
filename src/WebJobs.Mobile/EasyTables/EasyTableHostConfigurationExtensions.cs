// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using WebJobs.Extensions.EasyTables;

namespace Microsoft.Azure.WebJobs
{
    /// <summary>
    /// Extension methods for EasyTable integration.
    /// </summary>
    public static class EasyTablesHostConfigurationExtensions
    {
        /// <summary>
        /// Enables use of the EasyTable extensions.
        /// </summary>
        /// <param name="config">The <see cref="JobHostConfiguration"/> to configure.</param>
        /// <param name="easyTablesConfig">The <see cref="EasyTableConfiguration"/> to use.</param>
        public static void UseEasyTables(this JobHostConfiguration config, EasyTableConfiguration easyTablesConfig = null)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (easyTablesConfig == null)
            {
                easyTablesConfig = new EasyTableConfiguration();
            }

            // Register our extension configuration provider
            IExtensionRegistry extensions = config.GetService<IExtensionRegistry>();
            extensions.RegisterExtension<IExtensionConfigProvider>(new EasyTablesExtensionConfig(easyTablesConfig));
        }

        private class EasyTablesExtensionConfig : IExtensionConfigProvider
        {
            private EasyTableConfiguration easyTablesConfig;

            public EasyTablesExtensionConfig(EasyTableConfiguration easyTablesConfig)
            {
                this.easyTablesConfig = easyTablesConfig;
            }

            public void Initialize(ExtensionConfigContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                // Register our extension binding providers
                IExtensionRegistry extensions = context.Config.GetService<IExtensionRegistry>();
                extensions.RegisterExtension<IBindingProvider>(
                    new EasyTableAttributeBindingProvider(context.Config, this.easyTablesConfig, context.Config.NameResolver));
            }
        }
    }
}