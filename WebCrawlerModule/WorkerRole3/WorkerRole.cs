using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace WorkerRole3
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole3 is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;


            bool result = base.OnStart();
            
            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole3 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole3 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {

                // Check if Thread is Restarted
                if (AzureStorageManager.RetrieveRestartFlag(KeyIdentifier).Value > 0)
                {
                    IsStart = true;
                    AzureStorageManager.UpdateRestartFlag(KeyIdentifier);
                }

                // Check if web crawler should be initialized
                if (IsStart)
                {
                    PerformInitialSetUp();
                    IsStart = false;
                }

                thread1 = StartWorkerThread(thread1, "THR-DOG");
                thread2 = StartWorkerThread(thread2, "THR-CAT");
                thread3 = StartWorkerThread(thread3, "THR-HRS");
                thread4 = StartWorkerThread(thread4, "THR-RBT");

                thread5 = StartCountingThread(thread5, "THR-CTR");

                await Task.Delay(100);
            }
        }

        private static Thread thread1;
        private static Thread thread2;
        private static Thread thread3;
        private static Thread thread4;
        private static Thread thread5;

        private static readonly string KeyIdentifier = "IMDB";
        private static int blobBlockCount = 0;
        private static bool IsStart = true;

        private Thread StartWorkerThread(Thread thread, string name)
        {

            if (thread == null)
            {
                thread = new Thread(new ThreadStart(CrawilingProcedure));
                thread.Name = name;
                thread.Start();
            }

            return thread;
        }

        private Thread StartCountingThread(Thread thread, string name)
        {

            if (thread == null)
            {
                thread = new Thread(new ThreadStart(MachineCounters));
                thread.Name = name;
                thread.Start();
            }

            return thread;
        }

        private static void PerformInitialSetUp()
        {
            PerformLoggingOperations("Analyzing Robots.txt from seed: " + WebCrawler.seedUrl);
            string[] urlsFromSeed = WebCrawler.StartCollectingFromSeed();
            PerformStartUpQueueOperationOnUrls(urlsFromSeed);
            PerformLoggingOperations("Successfully Finished Analyzing Robots.txt from seed: " + WebCrawler.seedUrl);
        }

        private static void CrawilingProcedure()
        {
            while (true)
            {
                if (CheckIfThreadIsSwitched())
                {
                    // SET TO LOADING
                    string statusLog = AzureStorageManager.UpdateThreadStatus(KeyIdentifier, Thread.CurrentThread.Name, "loading");
                    PerformLoggingOperations(statusLog);

                    string urlString = GetURL();

                    if (urlString != null)
                    {
                        string changeUrlLog = AzureStorageManager.UpdateCurrentlyCrawledUrlTableEntity(KeyIdentifier, Thread.CurrentThread.Name, urlString);
                        PerformLoggingOperations(changeUrlLog);

                        try
                        {
                            // check first if retrieved url is xml or html/htm
                            bool urlIsXML = WebCrawler.IsXML(urlString);

                            if (!urlIsXML)
                            {
                                PerformIfNotXML(urlString);
                            }

                            // get all valid urls (for all urls)
                            string[] collectedUrls = WebCrawler.GetAllValidUrls(urlString).Distinct().ToArray();
                            PerformLoggingOperations("Collected a total of " + collectedUrls.Length + " Urls from web content of " + urlString);

                            // filter unneeded ones
                            string[] filteredUrls = WebCrawler.FilterResults(collectedUrls);
                            PerformLoggingOperations("Filtered " + (collectedUrls.Length - filteredUrls.Length) + " urls");

                            // check if acquired urls are already visited
                            string[] finalUrls = RemoveAlreadyVisitedUrls(filteredUrls);
                            PerformLoggingOperations("Number of already visited urls: " + (filteredUrls.Length - finalUrls.Length));

                            // add urls to queue
                            PerformQueueOperationOnUrls(finalUrls);

                            // increment number of urls crawled
                            AzureStorageManager.IncrementNumberOfUrls(KeyIdentifier);
                        }

                        catch (Exception e)
                        {
                            PerformLoggingOperations(e.ToString());
                        }

                    }
                }
                else
                {
                    //SET TO STOPPED
                    string statusLog = AzureStorageManager.UpdateThreadStatus(KeyIdentifier, Thread.CurrentThread.Name, "stopped");
                    PerformLoggingOperations(statusLog);
                    string changeUrlLog = AzureStorageManager.UpdateCurrentlyCrawledUrlTableEntity(KeyIdentifier, Thread.CurrentThread.Name, "none");
                    PerformLoggingOperations(changeUrlLog);
                }

                Thread.Sleep(100);
            }
        }

        private static void PerformIfNotXML(string urlString)
        {
            // get all required data and add to table (for html/htm only)
            HTMLData dataToBeSaved = WebCrawler.CreateDataFromUrl(urlString, blobBlockCount.ToString());
            string log = AzureStorageManager.AddHTMLDataEntityToTable(dataToBeSaved);
            PerformLoggingOperations(log);

            string bodyContentText = WebCrawler.GetBodyTextFromUrl(urlString);
            string blockBlobRefenceName = dataToBeSaved.BodyContent;
            string uploadLog = AzureStorageManager.SaveTextBodyToBlockBlob(bodyContentText, blockBlobRefenceName);
            PerformLoggingOperations(uploadLog);
            blobBlockCount++;
        }

        private static string[] RemoveAlreadyVisitedUrls(string[] urls)
        {
            List<string> filteredUrls = new List<string>();
            foreach (string url in urls)
            {
                HTMLData retrievedData = AzureStorageManager.RetrieveHTMLDataEntityFromTable(url, WebCrawler.rootDomainIdentifier);
                if (retrievedData == null)
                {
                    filteredUrls.Add(url);
                }
            }
            return filteredUrls.ToArray();
        }

        private static string GetURL()
        {
            string urlString = AzureStorageManager.GetUrlMessageFromQueue();
            if (urlString != null)
            {
                // SET TO CRAWLING
                string statusLog = AzureStorageManager.UpdateThreadStatus(KeyIdentifier, Thread.CurrentThread.Name, "crawling");
                PerformLoggingOperations(statusLog);
                PerformLoggingOperations(urlString + " Has Been Grabbed From Queue!");
                return urlString;
            }
            else
            {
                // SET TO IDLE
                string statusLog = AzureStorageManager.UpdateThreadStatus(KeyIdentifier, Thread.CurrentThread.Name, "idle");
                PerformLoggingOperations(statusLog);
                PerformLoggingOperations("There are no more urls from Seed: " + WebCrawler.seedUrl);
                string changeUrlLog = AzureStorageManager.UpdateCurrentlyCrawledUrlTableEntity(KeyIdentifier, Thread.CurrentThread.Name, "none");
                PerformLoggingOperations(changeUrlLog);
                return null;
            }
        }

        private static void PerformQueueOperationOnUrls(string[] urls)
        {
            foreach (string url in urls)
            {
                try
                {
                    if (!CheckIfThreadIsSwitched())
                    {
                        break;
                    }
                }
                catch
                {
                }
                string log = AzureStorageManager.AddUrlToQueue(url);
                PerformLoggingOperations(log);
            }
        }

        private static void PerformStartUpQueueOperationOnUrls(string[] urls)
        {
            foreach (string url in urls)
            {
                string log = AzureStorageManager.AddUrlToQueue(url);
                PerformLoggingOperations(log);
            }
        }

        private static void PerformLoggingOperations(string logMessage)
        {
            Trace.TraceInformation(Thread.CurrentThread.Name + " : " + logMessage);
            AzureStorageManager.AddLogMessage(Thread.CurrentThread.Name + " : " + logMessage);
            AzureStorageManager.AddIndividualLogMessage(Thread.CurrentThread.Name + " : " + logMessage);
        }

        private static void MachineCounters()
        {
            PerformanceCounter theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            PerformanceCounter theMemCounter = new PerformanceCounter("Memory", "Available MBytes");
            while (true)
            {
                Thread.Sleep(300);
                double ram = theMemCounter.NextValue();
                double cpu = theCPUCounter.NextValue();
                string logResult = AzureStorageManager.UpdateMachineCounterValues(KeyIdentifier, "CPU Usage", cpu);
                PerformLoggingOperations(logResult);
                logResult = AzureStorageManager.UpdateMachineCounterValues(KeyIdentifier, "Available Memory", ram);
                PerformLoggingOperations(logResult);
            }
        }

        private static bool CheckIfThreadIsSwitched()
        {
            ThreadSwitch threadSwitch = AzureStorageManager.RetrieveThreadSwitchStatus(Thread.CurrentThread.Name, KeyIdentifier);
            if (threadSwitch != null)
            {
                return threadSwitch.IsSwitched;
            }
            else
            {
                return false;
            }
        }
    }
}
