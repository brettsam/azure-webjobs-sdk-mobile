using System.Linq;
using Microsoft.WindowsAzure.MobileServices;
using WebJobs.Extensions.EasyTables;
using Xunit;

namespace Microsoft.Azure.WebJobs.ServiceBus.UnitTests.EasyTable
{
    public class EasyTableQueryValueProviderTests
    {
        [Fact]
        public void GetValue_ReturnsCorrectType()
        {
            var parameter = EasyTableTestHelper.GetValidInputQueryParameters().Single();
            var context = new EasyTableContext()
            {
                Client = new MobileServiceClient("http://someuri")
            };
            var provider = new EasyTableQueryValueProvider<TodoItem>(parameter, context);

            var value = provider.GetValue();

            Assert.True(typeof(IMobileServiceTableQuery<TodoItem>).IsAssignableFrom(value.GetType()));
        }
    }
}