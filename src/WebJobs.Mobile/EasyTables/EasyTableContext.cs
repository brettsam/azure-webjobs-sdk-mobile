// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using Microsoft.WindowsAzure.MobileServices;

namespace WebJobs.Extensions.EasyTables
{
    internal class EasyTableContext
    {
        public EasyTableConfiguration Config { get; set; }

        public IMobileServiceClient Client { get; set; }

        public string ResolvedTableName { get; set; }

        public string ResolvedId { get; set; }
    }
}