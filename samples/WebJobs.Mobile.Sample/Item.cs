using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJobs.Mobile.Sample
{
    public class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool IsProcessed { get; set; }
        public DateTimeOffset ProcessedAt { get; set; }

        // EasyTable properties
        public DateTimeOffset CreatedAt { get; set; }
    }
}