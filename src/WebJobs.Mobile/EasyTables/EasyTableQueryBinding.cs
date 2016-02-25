using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Microsoft.Azure.WebJobs.ServiceBus.EasyTables
{
    internal class EasyTableQueryBinding : IBinding
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
            return new ParameterDescriptor();
        }

        private IValueProvider CreateQueryValueProvider(Type coreType)
        {
            Type genericType = typeof(EasyTableQueryValueProvider<>).MakeGenericType(coreType);
            return (IValueProvider)Activator.CreateInstance(genericType, _parameter, _context);
        }
    }
}