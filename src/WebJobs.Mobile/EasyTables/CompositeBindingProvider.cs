using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace WebJobs.Mobile.EasyTables
{
    internal class CompositeBindingProvider : IBindingProvider
    {
        private IEnumerable<IBindingProvider> _innerBindingProviders;

        public CompositeBindingProvider(params IBindingProvider[] innerBindingProviders)
        {
            _innerBindingProviders = innerBindingProviders.AsEnumerable();
        }

        public async Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            IBinding match = null;
            foreach (IBindingProvider provider in _innerBindingProviders)
            {
                match = await provider.TryCreateAsync(context);
                if (match != null)
                {
                    return match;
                }
            }
            return match;
        }
    }
}