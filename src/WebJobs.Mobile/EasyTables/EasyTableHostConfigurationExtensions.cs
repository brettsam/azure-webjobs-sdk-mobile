// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using WebJobs.Extensions.EasyTables;

namespace Microsoft.Azure.WebJobs
{
    /// <summary>
    ///
    /// </summary>
    public static class EasyTablesHostConfigurationExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="config"></param>
        /// <param name="easyTablesConfig"></param>
        public static void UseEasyTables(this JobHostConfiguration config, EasyTableConfiguration easyTablesConfig = null)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
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
                    throw new ArgumentNullException("context");
                }

                // Register our extension binding providers
                IExtensionRegistry extensions = context.Config.GetService<IExtensionRegistry>();
                extensions.RegisterExtension<IBindingProvider>(
                    new EasyTableAttributeBindingProvider(context.Config, this.easyTablesConfig, context.Config.NameResolver));
            }
        }
    }
}