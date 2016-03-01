﻿// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json.Linq;
using WebJobs.Mobile.EasyTables;
using Xunit;

namespace WebJobs.Mobile.Test.EasyTables
{
    public class EasyTableOutputBindingProviderTests
    {
        [Theory]
        [InlineData(typeof(TodoItem), true, true)]
        [InlineData(typeof(JObject), true, true)]
        [InlineData(typeof(TodoItem[]), true, true)]
        [InlineData(typeof(JObject[]), true, true)]
        [InlineData(typeof(TodoItem), false, false)]
        [InlineData(typeof(JObject), false, false)]
        [InlineData(typeof(TodoItem[]), false, false)]
        [InlineData(typeof(JObject[]), false, false)]
        [InlineData(typeof(NoId), true, false)]
        [InlineData(typeof(ICollector<TodoItem>), true, false)]
        public void IsValidOutType_ValidatesCorrectly(Type parameterType, bool isOutParameter, bool expected)
        {
            // Arrange
            Type typeToTest = isOutParameter ? parameterType.MakeByRefType() : parameterType;

            // Act
            bool result = EasyTableOutputBindingProvider.IsValidOutType(typeToTest);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(typeof(ICollector<TodoItem>), false, true)]
        [InlineData(typeof(IAsyncCollector<TodoItem>), false, true)]
        [InlineData(typeof(ICollector<JObject>), false, true)]
        [InlineData(typeof(IAsyncCollector<JObject>), false, true)]
        [InlineData(typeof(ICollector<TodoItem>), true, false)]
        [InlineData(typeof(TodoItem), false, false)]
        [InlineData(typeof(ICollector<NoId>), false, false)]
        public void IsValidCollectorType_ValidatesCorrectly(Type parameterType, bool isOutParameter, bool expected)
        {
            // Arrange
            Type typeToTest = isOutParameter ? parameterType.MakeByRefType() : parameterType;

            // Act
            bool result = EasyTableOutputBindingProvider.IsValidCollectorType(typeToTest);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}