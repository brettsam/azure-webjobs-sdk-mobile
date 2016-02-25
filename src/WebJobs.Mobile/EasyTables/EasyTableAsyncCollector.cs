using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Azure.WebJobs.ServiceBus.EasyTables;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace WebJobs.Extensions.EasyTables
{
    internal class EasyTableAsyncCollector<T> : IFlushCollector<T>
    {
        private EasyTableContext _context;

        public EasyTableAsyncCollector(EasyTableContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T item, CancellationToken cancellationToken = default(CancellationToken))
        {
            IMobileServiceClient client = new MobileServiceClient(_context.Config.EasyTableUri);

            if (item is JObject)
            {
                IMobileServiceTable table = client.GetTable(_context.ResolvedTableName);
                await table.InsertAsync(item as JObject);
            }
            else
            {
                IMobileServiceTable<T> table = client.GetTable<T>();
                await table.InsertAsync(item);
            }
        }

        public Task FlushAsync()
        {
            // do nothing
            return Task.FromResult(0);
        }
    }
}