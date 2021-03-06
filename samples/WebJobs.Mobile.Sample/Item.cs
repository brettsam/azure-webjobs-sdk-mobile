﻿// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;

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