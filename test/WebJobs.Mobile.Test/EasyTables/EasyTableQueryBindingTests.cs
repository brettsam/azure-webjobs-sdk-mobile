using System.Linq;
using System.Threading.Tasks;
using WebJobs.Extensions.EasyTables;
using Xunit;

namespace Microsoft.Azure.WebJobs.ServiceBus.UnitTests.EasyTable
{
    public class EasyTableQueryBindingTests
    {
        [Fact]
        public async Task BindAsync_Returns_CorrectValueProvider()
        {
            // Arrange
            var parameter = EasyTableTestHelper.GetValidInputQueryParameters().Single();
            var expectedType = typeof(EasyTableQueryValueProvider<TodoItem>);
            var easyTableContext = new EasyTableContext();
            var binding = new EasyTableQueryBinding(parameter, easyTableContext);

            // Act
            var valueProvider = await binding.BindAsync(null, null);

            // Assert
            Assert.Equal(expectedType, valueProvider.GetType());
        }
    }
}