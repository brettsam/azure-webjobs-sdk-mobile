using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus.UnitTests.EasyTable;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using WebJobs.Mobile.EasyTables;
using Xunit;

namespace WebJobs.Mobile.Test.EasyTables
{
    public class EasyTableUtilityTests
    {
        public static IEnumerable<object[]> ValidTypes
        {
            get
            {
                var parameters = EasyTableTestHelper.GetAllValidParameters().ToArray();

                var otherTypes = typeof(EasyTableUtilityTests)
                    .GetMethod("OtherTypes", BindingFlags.NonPublic | BindingFlags.Static)
                    .GetParameters();

                return new[]
                {
                    new object[] { parameters[0], typeof(JObject) },
                    new object[] { parameters[1], typeof(TodoItem) },
                    new object[] { parameters[2], typeof(JObject) },
                    new object[] { parameters[3], typeof(TodoItem) },
                    new object[] { parameters[4], typeof(JObject) },
                    new object[] { parameters[5], typeof(TodoItem) },
                    new object[] { parameters[6], typeof(JObject) },
                    new object[] { parameters[7], typeof(TodoItem) },
                    new object[] { parameters[8], typeof(JObject) },
                    new object[] { parameters[9], typeof(TodoItem) },
                    new object[] { parameters[10], typeof(IMobileServiceTable) },
                    new object[] { parameters[11], typeof(TodoItem) },
                    new object[] { parameters[12], typeof(TodoItem) },
                    new object[] { otherTypes[0], typeof(IEnumerable<TodoItem>) },
                    new object[] { otherTypes[1], typeof(TodoItem[]) },
                    new object[] { otherTypes[2], typeof(TodoItem) },
                };
            }
        }

        [Theory]
        [MemberData("ValidTypes")]
        public void GetCoreType_Returns_CorrectType(ParameterInfo parameter, Type expectedType)
        {
            // Act
            Type coreType = EasyTableUtility.GetCoreType(parameter.ParameterType);

            // Assert
            Assert.Equal(coreType, expectedType);
        }

        [Theory]
        [InlineData(typeof(TodoItem), true)]
        [InlineData(typeof(JObject), true)]
        [InlineData(typeof(IMobileServiceTable), false)]
        [InlineData(typeof(IAsyncCollector<TodoItem>), false)]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(NoId), false)]
        [InlineData(typeof(TwoId), false)]
        [InlineData(typeof(PrivateId), false)]
        public void IsValidEasyTableType_CorrectlyEvaluates(Type typeToEvaluate, bool expected)
        {
            // Act
            bool result = EasyTableUtility.IsValidItemType(typeToEvaluate);

            // Assert
            Assert.Equal(expected, result);
        }

        private static void OtherTypes(
            IEnumerable<IEnumerable<TodoItem>> enumOfEnumOfT,
            TodoItem[][] arrayOfArrays,
            out IAsyncCollector<TodoItem> collectorOut)
        {
            collectorOut = null;
        }

        private class TwoId
        {
            public string ID { get; set; }
            public string id { get; set; }
        }

        private class PrivateId
        {
            private string Id { get; set; }
        }
    }
}