// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.MobileServices;
using WebJobs.Extensions.EasyTables;

namespace WebJobs.Mobile.Sample
{
    public class Functions
    {
        public static void InsertItem(
            [TimerTrigger("00:01")] TimerInfo timer,
            [EasyTable] out Item newItem)
        {
            newItem = new Item()
            {
                Id = Guid.NewGuid().ToString(),
                Text = new Random().Next().ToString()
            };
        }

        public static async Task EnqueueItemToProcess(
            [TimerTrigger("00:01")] TimerInfo timer,
            [EasyTable] IMobileServiceTableQuery<Item> itemQuery,
            [Queue("ToProcess")] IAsyncCollector<string> queueItems)
        {
            IEnumerable<string> itemsToProcess = await itemQuery
                .Where(i => !i.IsProcessed)
                .Select(i => i.Id)
                .ToListAsync();

            foreach (string itemId in itemsToProcess)
            {
                await queueItems.AddAsync(itemId);
            }
        }

        public static void DequeueAndProcess(
            [QueueTrigger("ToProcess")] string itemId,
            [EasyTable(id: "{QueueTrigger}")] Item itemToProcess)
        {
            itemToProcess.IsProcessed = true;
            itemToProcess.ProcessedAt = DateTimeOffset.Now;
        }

        public static async Task DeleteProcessedItems(
            [TimerTrigger("00:05")] TimerInfo timerInfo,
            [EasyTable] IMobileServiceTable<Item> table)
        {
            IEnumerable<Item> processedItems = await table.CreateQuery()
                .Where(i => i.IsProcessed && i.ProcessedAt < DateTime.Now.AddMinutes(-5))
                .ToListAsync();

            foreach (Item i in processedItems)
            {
                await table.DeleteAsync(i);
            }
        }
    }
}