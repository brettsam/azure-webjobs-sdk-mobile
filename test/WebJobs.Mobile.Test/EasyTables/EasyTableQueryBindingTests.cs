using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
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

        [Theory]
        [InlineData(typeof(IMobileServiceTableQuery<TodoItem>), true)]
        [InlineData(typeof(IMobileServiceTableQuery<JObject>), false)]
        [InlineData(typeof(IMobileServiceTableQuery<NoId>), false)]
        [InlineData(typeof(TodoItem), false)]
        public void IsValidQueryType_ValidatesCorrectly(Type paramType, bool expected)
        {
            // Act
            bool result = EasyTableQueryBinding.IsValidQueryType(paramType);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}