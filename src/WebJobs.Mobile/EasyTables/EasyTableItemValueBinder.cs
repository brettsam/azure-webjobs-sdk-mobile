using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.ServiceBus.EasyTables
{
    internal class EasyTableItemValueBinder<T> : IValueBinder
    {
        private ParameterInfo _parameter;
        private EasyTableContext _context;
        private string _id;
        private JToken _originalItem;

        public EasyTableItemValueBinder(ParameterInfo parameter, EasyTableContext context, string id)
        {
            _parameter = parameter;
            _context = context;
            _id = id;
        }

        public async Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            if (value == null || _originalItem == null)
            {
                return;
            }

            JToken currentValue = null;
            bool isJObject = value.GetType() == typeof(JObject);

            if (isJObject)
            {
                currentValue = value as JToken;
            }
            else
            {
                currentValue = JToken.FromObject(value);
            }

            if (HasChanged(currentValue))
            {
                // make sure it's not the Id that has changed
                if (!string.Equals(GetId(_originalItem), GetId(currentValue), StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("Cannot update the 'Id' property.");
                }

                if (isJObject)
                {
                    IMobileServiceTable table = _context.Client.GetTable(_context.ResolvedTableName);
                    await table.UpdateAsync((JObject)value);
                }
                else
                {
                    IMobileServiceTable<T> table = _context.Client.GetTable<T>();
                    await table.UpdateAsync((T)value);
                }
            }
        }

        private static string GetId(JToken item)
        {
            JToken idToken = item["Id"];
            if (idToken == null)
            {
                idToken = item["id"];
            }

            return idToken.ToString();
        }

        private bool HasChanged(JToken value)
        {
            return !JToken.DeepEquals(_originalItem, value);
        }

        public Type Type
        {
            get { return _parameter.ParameterType; }
        }

        public object GetValue()
        {
            if (typeof(T) == typeof(JObject))
            {
                IMobileServiceTable table = _context.Client.GetTable(_context.ResolvedTableName);
                JToken item = null;

                IgnoreNotFoundException(() =>
                    {
                        item = table.LookupAsync(_id).Result;
                        _originalItem = item.DeepClone();
                    });

                return item;
            }
            else
            {
                // treat this as POCO
                IMobileServiceTable<T> table = _context.Client.GetTable<T>();
                T item = default(T);

                IgnoreNotFoundException(() =>
                    {
                        item = table.LookupAsync(_id).Result;
                        _originalItem = JToken.FromObject(item);
                    });

                return item;
            }
        }

        public string ToInvokeString()
        {
            return string.Empty;
        }

        private static void IgnoreNotFoundException(Action action)
        {
            try
            {
                action();
            }
            catch (AggregateException ex)
            {
                foreach (Exception e in ex.InnerExceptions)
                {
                    MobileServiceInvalidOperationException mobileEx =
                        e as MobileServiceInvalidOperationException;
                    if (mobileEx == null ||
                        mobileEx.Response.StatusCode != System.Net.HttpStatusCode.NotFound)
                    {
                        throw;
                    }
                }
            }
        }
    }
}