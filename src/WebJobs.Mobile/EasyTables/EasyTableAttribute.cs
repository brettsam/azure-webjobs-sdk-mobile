// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace WebJobs.Extensions.EasyTables
{
    /// <summary>
    ///
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class EasyTableAttribute : Attribute
    {
        /// <summary>
        /// Binds the parameter to an EasyTable input or output binding.
        /// </summary>
        /// <param name="tableName">The name of the table to which the parameter applies.</param>
        /// <param name="id">The Id of the item to retrieve from the specified (or implied) table.</param>
        public EasyTableAttribute(string tableName = null, string id = null)
        {
            this.TableName = tableName;
            this.Id = id;
        }

        /// <summary>
        /// The name of the table to which the parameter applies.
        /// Required if using a JObject parameter; otherwise it is implied from the parameter type.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// The Id of the item to retrieve from the specified (or implied) table.
        /// </summary>
        public string Id { get; private set; }
    }
}