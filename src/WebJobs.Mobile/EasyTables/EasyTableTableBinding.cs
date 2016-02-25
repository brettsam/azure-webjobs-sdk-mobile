using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.ServiceBus.EasyTables
{
    internal class EasyTableTableBinding : IBinding
    {
        private ParameterInfo _parameter;
        private EasyTableContext _context;

        public EasyTableTableBinding(ParameterInfo parameter, EasyTableContext context)
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
            Type paramType = _parameter.ParameterType;
            Type coreType = typeof(JObject);
            if (paramType.IsGenericType &&
                paramType.GetGenericTypeDefinition() == typeof(IMobileServiceTable<>))
            {
                coreType = paramType.GetGenericArguments()[0];
            }

            return Task.FromResult(CreateTableValueProvider(coreType));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor();
        }

        private IValueProvider CreateTableValueProvider(Type coreType)
        {
            Type genericType = typeof(EasyTableTableValueProvider<>).MakeGenericType(coreType);
            return (IValueProvider)Activator.CreateInstance(genericType, _parameter, _context);
        }
    }
}