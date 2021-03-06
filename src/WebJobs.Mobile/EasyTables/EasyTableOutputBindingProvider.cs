﻿// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.ServiceBus;
using Newtonsoft.Json.Linq;
using WebJobs.Extensions.EasyTables;

namespace WebJobs.Mobile.EasyTables
{
    /// <summary>
    /// Provides an <see cref="IBinding"/> for valid output parameters decorated with
    /// an <see cref="EasyTableAttribute"/>.
    /// </summary>
    /// <remarks>
    /// The method parameter type can be one of the following:
    /// <list type="bullet">
    /// <item><description><see cref="ICollector{T}"/>, where T is either <see cref="JObject"/> or any type with a public string Id property.</description></item>
    /// <item><description><see cref="IAsyncCollector{T}"/>, where T is either <see cref="JObject"/> or any type with a public string Id property.</description></item>
    /// <item><description>out <see cref="JObject"/></description></item>
    /// <item><description>out <see cref="JObject"/>[]</description></item>
    /// <item><description>out T, where T is any Type with a public string Id property</description></item>
    /// <item><description>out T[], where T is any Type with a public string Id property</description></item>
    /// </list>
    /// </remarks>
    internal class EasyTableOutputBindingProvider : IBindingProvider
    {
        private EasyTableContext _easyTableContext;
        private JobHostConfiguration _jobHostConfig;

        public EasyTableOutputBindingProvider(JobHostConfiguration jobHostConfig, EasyTableContext easyTableContext)
        {
            _jobHostConfig = jobHostConfig;
            _easyTableContext = easyTableContext;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ParameterInfo parameter = context.Parameter;

            if (IsValidOutType(parameter.ParameterType) ||
                IsValidCollectorType(parameter.ParameterType))
            {
                return CreateBinding(parameter);
            }

            return Task.FromResult<IBinding>(null);
        }

        internal Task<IBinding> CreateBinding(ParameterInfo parameter)
        {
            IBinding genericBinding = GenericBinder.BindGenericCollector(parameter, _jobHostConfig.GetOrCreateConverterManager(),
                    typeof(EasyTableAsyncCollector<>), _easyTableContext);
            return Task.FromResult(genericBinding);
        }

        internal static bool IsValidOutType(Type paramType)
        {
            if (paramType.IsByRef)
            {
                Type coreType = paramType.GetElementType();
                if (coreType.IsArray)
                {
                    coreType = coreType.GetElementType();
                }

                if (EasyTableUtility.IsValidItemType(coreType))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsValidCollectorType(Type paramType)
        {
            if (paramType.IsGenericType)
            {
                Type genericType = paramType.GetGenericTypeDefinition();
                if (genericType == typeof(ICollector<>) || genericType == typeof(IAsyncCollector<>))
                {
                    if (EasyTableUtility.IsCoreTypeValidItemType(paramType))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}