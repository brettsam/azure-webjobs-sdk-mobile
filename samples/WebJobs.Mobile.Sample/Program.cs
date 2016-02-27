using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace WebJobs.Mobile.Sample
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    internal class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        private static void Main()
        {
            JobHostConfiguration jobConfig = new JobHostConfiguration();
            jobConfig.UseTimers();
            jobConfig.UseEasyTables();

            var host = new JobHost(jobConfig);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}