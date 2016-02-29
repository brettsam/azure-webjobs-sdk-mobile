using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace WebJobs.Mobile.EasyTables
{
    internal static class EasyTableUtility
    {
        public static bool IsCoreTypeValidItemType(Type type)
        {
            Type coreType = GetCoreType(type);
            return IsValidItemType(coreType);
        }

        // Evaluates whether the specified type is valid for use with EasyTables. The type
        // must contain a single public string 'Id' property or be of type JObject.
        public static bool IsValidItemType(Type itemType)
        {
            if (itemType == typeof(JObject))
            {
                return true;
            }

            // POCO types must have a string id property (case insensitive).
            IEnumerable<PropertyInfo> idProperties = itemType.GetProperties()
                .Where(p => string.Equals("id", p.Name, StringComparison.OrdinalIgnoreCase) && p.PropertyType == typeof(string));

            if (idProperties.Count() != 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the core EasyTable type for the supplied parameter.
        /// </summary>
        /// <remarks>
        /// For example, the core Type is T in the following parameters:
        /// <list type="bullet">
        /// <item><description><see cref="ICollector{T}"/></description></item>
        /// <item><description>T[]</description></item>
        /// <item><description>out T</description></item>
        /// <item><description>out T[]</description></item>
        /// </list>
        /// </remarks>
        /// <param name="parameter">The Type to evaluate.</param>
        /// <returns>The core Type</returns>
        public static Type GetCoreType(Type type)
        {
            Type coreType = type;
            if (coreType.IsByRef)
            {
                coreType = coreType.GetElementType();
            }

            if (coreType.IsArray)
            {
                return coreType.GetElementType();
            }

            if (coreType.IsGenericType)
            {
                // TODO: throw if more than 1 generic arg
                return coreType.GetGenericArguments()[0];
            }

            return coreType;
        }
    }
}