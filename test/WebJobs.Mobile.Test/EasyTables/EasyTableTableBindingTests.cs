using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WebJobs.Extensions.EasyTables;
using Xunit;

namespace Microsoft.Azure.WebJobs.ServiceBus.UnitTests.EasyTable
{
    public class EasyTableTableBindingTests
    {
        public static IEnumerable<object[]> ValidParameters
        {
            get
            {
                var itemParams = EasyTableTestHelper.GetValidInputTableParameters().ToArray();

                return new[]
                {
                    new object[] {itemParams[0], typeof(EasyTableTableValueProvider<JObject>)},
                    new object[] {itemParams[1], typeof(EasyTableTableValueProvider<TodoItem>)}
                };
            }
        }

        [Theory]
        [MemberData("ValidParameters")]
        public async Task BindAsync_Returns_CorrectValueProvider(ParameterInfo parameter, Type expectedType)
        {
            // Arrange
            var easyTableContext = new EasyTableContext();
            var binding = new EasyTableTableBinding(parameter, easyTableContext);

            // Act
            var valueProvider = await binding.BindAsync(null, null);

            // Assert
            Assert.Equal(expectedType, valueProvider.GetType());
        }
    }
}