using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Bindings.Path;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Microsoft.Azure.WebJobs.ServiceBus.EasyTables
{
    internal class EasyTableItemBinding : IBinding
    {
        private ParameterInfo _parameter;
        private EasyTableContext _context;
        private BindingTemplate _bindingTemplate;

        public EasyTableItemBinding(ParameterInfo parameter, EasyTableContext context, BindingProviderContext bindingContext)
        {
            _parameter = parameter;
            _context = context;

            // set up binding for '{ItemId}'-type bindings
            if (_context.ResolvedId != null)
            {
                _bindingTemplate = BindingTemplate.FromString(_context.ResolvedId);
                _bindingTemplate.ValidateContractCompatibility(bindingContext.BindingDataContract);
            }
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

            string id = ResolveId(context.BindingData);
            return BindAsync(id, context.ValueContext);
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            Type paramType = _parameter.ParameterType;
            return Task.FromResult<IValueProvider>(CreateItemValueProvider(paramType, value as string));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor();
        }

        internal string ResolveId(IReadOnlyDictionary<string, object> bindingData)
        {
            string id = null;
            if (_bindingTemplate != null)
            {
                id = _bindingTemplate.Bind(bindingData);
            }
            return id;
        }

        private IValueProvider CreateItemValueProvider(Type coreType, string id)
        {
            Type genericType = typeof(EasyTableItemValueBinder<>).MakeGenericType(coreType);
            return (IValueProvider)Activator.CreateInstance(genericType, _parameter, _context, id);
        }
    }
}