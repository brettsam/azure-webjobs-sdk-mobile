// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace WebJobs.Extensions.EasyTables
{
    internal class EasyTableAttributeBindingProvider : IBindingProvider
    {
        private EasyTableConfiguration _easyTableConfig;
        private JobHostConfiguration _jobHostConfig;
        private INameResolver _nameResolver;

        public EasyTableAttributeBindingProvider(JobHostConfiguration config, EasyTableConfiguration easyTableConfig, INameResolver nameResolver)
        {
            _jobHostConfig = config;
            _easyTableConfig = easyTableConfig;
            _nameResolver = nameResolver;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ParameterInfo parameter = context.Parameter;
            EasyTableAttribute attribute = parameter.GetCustomAttribute<EasyTableAttribute>(inherit: false);
            if (attribute == null)
            {
                return Task.FromResult<IBinding>(null);
            }

            if (string.IsNullOrEmpty(_easyTableConfig.EasyTableUri))
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InstalledUICulture,
                    "The Easy Tables Uri must be set either via a '{0}' app setting or directly in code via EasyTableConfiguration.EasyTableUri.",
                    EasyTableConfiguration.AzureWebJobsEasyTableUriName));
            }

            ValidateParameter(parameter);

            EasyTableContext easyTableContext = new EasyTableContext
            {
                Config = this._easyTableConfig,
                Client = new MobileServiceClient(this._easyTableConfig.EasyTableUri),
                ResolvedId = Resolve(attribute.Id),
                ResolvedTableName = Resolve(attribute.TableName)
            };

            Type paramType = parameter.ParameterType;

            if (IsOutputBinding(parameter))
            {
                IBinding genericBinding = GenericBinder.BindGenericCollector(parameter, _jobHostConfig.GetOrCreateConverterManager(),
                    typeof(EasyTableAsyncCollector<>), easyTableContext);
                return Task.FromResult(genericBinding);
            }

            if (paramType.IsGenericType &&
                paramType.GetGenericTypeDefinition() == typeof(IMobileServiceTableQuery<>))
            {
                return Task.FromResult<IBinding>(new EasyTableQueryBinding(parameter, easyTableContext));
            }

            if (typeof(IMobileServiceTable).IsAssignableFrom(paramType))
            {
                return Task.FromResult<IBinding>(new EasyTableTableBinding(parameter, easyTableContext));
            }

            // It must be a JObject or POCO
            return Task.FromResult<IBinding>(new EasyTableItemBinding(parameter, easyTableContext, context));
        }

        private static bool IsOutputBinding(ParameterInfo parameter)
        {
            if (parameter.IsOut)
            {
                return true;
            }

            Type paramType = parameter.ParameterType;

            if (paramType.IsGenericType)
            {
                Type genericType = paramType.GetGenericTypeDefinition();
                return genericType == typeof(ICollector<>) || genericType == typeof(IAsyncCollector<>);
            }

            return false;
        }

        private string Resolve(string value)
        {
            if (_nameResolver == null)
            {
                return value;
            }

            return _nameResolver.ResolveWholeString(value);
        }

        private static void ValidateParameter(ParameterInfo parameter)
        {
            // Output Bindings:
            //   out T
            //   out T[]
            //   ICollector<T>
            //   IAsyncCollector<T>
            //   out JObject
            //   out JObject[]
            //   ICollector<JObject>
            //   IAsyncCollector<JObject>

            // Input Bindings:
            // T
            // JObject
            // IMobileServiceTable
            // IMobileServiceTable<T>
            // IMobileServiceTableQuery<T>

            Type[] allowedGenericTypes = new[]
                {
                    typeof(ICollector<>),
                    typeof(IAsyncCollector<>),
                    typeof(IMobileServiceTable<>),
                    typeof(IMobileServiceTableQuery<>)
                };

            Type[] allowedTypes = new[]
                {
                    typeof(JObject),
                    typeof(IMobileServiceTable)
                };

            Type paramType = parameter.ParameterType;

            if (parameter.IsOut)
            {
                Type coreType = paramType.GetElementType();

                if (coreType.IsArray)
                {
                    coreType = coreType.GetElementType();
                }

                if (coreType == typeof(JObject))
                {
                    return;
                }

                ValidateCoreType(coreType);
                return;
            }
            else
            {
                if (paramType.IsGenericType)
                {
                    Type coreType = paramType.GetGenericArguments()[0];
                    Type genericType = paramType.GetGenericTypeDefinition();
                    if (allowedGenericTypes.Contains(genericType))
                    {
                        if ((genericType == typeof(IAsyncCollector<>) ||
                            genericType == typeof(ICollector<>)) &&
                            coreType == typeof(JObject))
                        {
                            return;
                        }

                        ValidateCoreType(coreType);
                        return;
                    }
                }
                {
                    if (allowedTypes.Contains(paramType))
                    {
                        return;
                    }

                    ValidateCoreType(paramType);
                    return;
                }
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Can't bind EasyTableAttribute to type '{0}'.", parameter.ParameterType));
        }

        private static void ValidateCoreType(Type coreType)
        {
            // POCO types must have a string id property (case insensitive).
            IEnumerable<PropertyInfo> idProperties = coreType.GetProperties()
                .Where(p => string.Equals("id", p.Name, StringComparison.OrdinalIgnoreCase) && p.PropertyType == typeof(string));

            if (idProperties.Count() != 1)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "There must be exactly one string property named 'id' (regardless of casing) on type '{0}'",
                    coreType.FullName));
            }
        }
    }
}