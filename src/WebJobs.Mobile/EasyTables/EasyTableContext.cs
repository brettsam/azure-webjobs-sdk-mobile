using Microsoft.WindowsAzure.MobileServices;
using WebJobs.Extensions.EasyTables;

namespace Microsoft.Azure.WebJobs.ServiceBus.EasyTables
{
    internal class EasyTableContext
    {
        public EasyTableConfiguration Config { get; set; }

        public IMobileServiceClient Client { get; set; }

        public string ResolvedTableName { get; set; }

        public string ResolvedId { get; set; }
    }
}