using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using WebJobs.Mobile.EasyTables;

namespace WebJobs.Extensions.EasyTables
{
    /// <summary>
    /// Provides an <see cref="IBinding"/> for valid input query parameters decorated with
    /// an <see cref="EasyTableAttribute"/>.
    /// </summary>
    /// <remarks>
    /// The method parameter type must be of Type <see cref="IMobileServiceTableQuery{T}"/>,
    /// where T is any Type with a public string Id property.
    /// </remarks>
    internal class EasyTableQueryBinding : IBinding, IBindingProvider
    {
        private ParameterInfo _parameter;
        private EasyTableContext _context;

        public EasyTableQueryBinding(ParameterInfo parameter, EasyTableContext context)
        {
            _parameter = parameter;
            _context = context;
        }

        public bool FromAttribute
        {
            get { return true; }
        }

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return BindAsync(null, context.ValueContext);
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            Type coreType = _parameter.ParameterType.GetGenericArguments()[0];
            return Task.FromResult(CreateQueryValueProvider(coreType));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor
            {
                Name = _parameter.Name
            };
        }

        private IValueProvider CreateQueryValueProvider(Type coreType)
        {
            Type genericType = typeof(EasyTableQueryValueProvider<>).MakeGenericType(coreType);
            return (IValueProvider)Activator.CreateInstance(genericType, _parameter, _context);
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (IsValidQueryType(context.Parameter.ParameterType))
            {
                return Task.FromResult<IBinding>(this);
            }

            return Task.FromResult<IBinding>(null);
        }

        internal static bool IsValidQueryType(Type paramType)
        {
            if (paramType.IsGenericType &&
               paramType.GetGenericTypeDefinition() == typeof(IMobileServiceTableQuery<>))
            {
                // IMobileServiceTableQuery<JObject> is not supported.
                Type coreType = EasyTableUtility.GetCoreType(paramType);
                if (coreType != typeof(JObject) &&
                    EasyTableUtility.IsCoreTypeValidItemType(paramType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}