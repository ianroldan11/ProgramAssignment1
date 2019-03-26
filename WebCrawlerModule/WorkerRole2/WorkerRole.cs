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

namespace WorkerRole2
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole2 is running");

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
                        
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;            

            return result;            
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole2 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole2 has stopped");
        }        

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                // Check if Thread is Restarted
                if (AzureStorageManager.RetrieveRestartFlag(KeyIdentifier).Value > 0)
                {
                    // sets the flag to starting position
                    IsStart = true;
                    // sets the restart flag stored in the table as false
                    AzureStorageManager.UpdateRestartFlag(KeyIdentifier);
                }

                // Check if web crawler should be initialized
                if (IsStart)
                {
                    // perform analysis on robots.txt
                    PerformInitialSetUp();
                    // closes the flag for starting position
                    IsStart = false;
                }

                // Creates instances for working threads
                thread1 = StartWorkerThread(thread1, "THR-DOG");
                thread2 = StartWorkerThread(thread2, "THR-CAT");
                thread3 = StartWorkerThread(thread3, "THR-HRS");
                thread4 = StartWorkerThread(thread4, "THR-RBT");
                // Separate Thread that monitors CPU usage and RAM space
                thread5 = StartCountingThread(thread5, "THR-CTR");

                await Task.Delay(100);
            }
        }
        // Threads
        private static Thread thread1;
        private static Thread thread2;
        private static Thread thread3;
        private static Thread thread4;
        private static Thread thread5;
        // Unique number for each of the blob files where HTML body content will be stored
        private static int blobBlockCount = 0;
        // Key Identifier unique for this web crawler worker role
        private static readonly string KeyIdentifier = "Bleacher Report";
        // boolean flag to tell if crawler is at starting position
        private static bool IsStart = true;

        /// <summary>
        /// Starts Worker thread
        /// </summary>
        /// <param name="thread">Thread object that will be working</param>
        /// <param name="name">Unique name for the thread</param>
        /// <returns>new thread object</returns>
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

        /// <summary>
        /// Starts Counting thread
        /// </summary>
        /// <param name="thread">Thread object that will be counting the machine</param>
        /// <param name="name">name of the thread</param>
        /// <returns>new thread object</returns>
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

        /// <summary>
        /// Happens on startup - analyzes robots.txt, gets sitemaps and retrieves directories that are disallowed
        /// </summary>
        private static void PerformInitialSetUp()
        {
            PerformLoggingOperations("Analyzing Robots.txt from seed: " + WebCrawler.seedUrl);
            string[] urlsFromSeed = WebCrawler.StartCollectingFromSeed();
            PerformStartUpQueueOperationOnUrls(urlsFromSeed);
            PerformLoggingOperations("Successfully Finished Analyzing Robots.txt from seed: " + WebCrawler.seedUrl);
        }

        /// <summary>
        /// entire procedure of crawling
        /// </summary>
        private static void CrawilingProcedure()
        {
            // loops forever
            while (true)
            {
                // if switch for thread is on
                if (CheckIfThreadIsSwitched())
                {      
                    // SET TO LOADING STATUS
                    string statusLog = AzureStorageManager.UpdateThreadStatus(KeyIdentifier, Thread.CurrentThread.Name, "loading");
                    PerformLoggingOperations(statusLog);

                    // gets url from queue
                    string urlString = GetURL();

                    // makes sure that there is a retrievable message from the url queue
                    if (urlString != null)
                    {
                        // updates the url to be displayed on the client side as the currently crawled url
                        string changeUrlLog = AzureStorageManager.UpdateCurrentlyCrawledUrlTableEntity(KeyIdentifier, Thread.CurrentThread.Name, urlString);
                        PerformLoggingOperations(changeUrlLog);

                        try
                        {
                            // check first if retrieved url is xml or html/htm
                            bool urlIsXML = WebCrawler.IsXML(urlString);
                            // performs only if it is not an xml file (i.e. html)
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
                    //SET TO STOPPED STATUS
                    string statusLog = AzureStorageManager.UpdateThreadStatus(KeyIdentifier, Thread.CurrentThread.Name, "stopped");
                    PerformLoggingOperations(statusLog);
                    string changeUrlLog = AzureStorageManager.UpdateCurrentlyCrawledUrlTableEntity(KeyIdentifier, Thread.CurrentThread.Name, "none");
                    PerformLoggingOperations(changeUrlLog);
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// gets all pertinent data from an html - Title, publish date, body
        /// </summary>
        /// <param name="urlString"></param>
        private static void PerformIfNotXML(string urlString)
        {
            // get all required data and add to table (for html/htm only)
            //Creates an entity to be store in the azure table
            HTMLData dataToBeSaved = WebCrawler.CreateDataFromUrl(urlString, blobBlockCount.ToString());
            // adds the entity to the table
            string log = AzureStorageManager.AddHTMLDataEntityToTable(dataToBeSaved);
            PerformLoggingOperations(log);
            // gets the body content of the url
            string bodyContentText = WebCrawler.GetBodyTextFromUrl(urlString);
            // creates a block blob to store the body content
            string blockBlobRefenceName = dataToBeSaved.BodyContent;
            string uploadLog = AzureStorageManager.SaveTextBodyToBlockBlob(bodyContentText, blockBlobRefenceName);
            PerformLoggingOperations(uploadLog);
            blobBlockCount++;
        }

        /// <summary>
        /// excludes urls that is already visited
        /// </summary>
        /// <param name="urls">array of urls that will be inspected</param>
        /// <returns>filtered urls</returns>
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

        /// <summary>
        /// Gets a url string from the queue
        /// </summary>
        /// <returns>url string to be crawled</returns>
        private static string GetURL()
        {
            string urlString = AzureStorageManager.GetUrlMessageFromQueue();
            if (urlString != null)
            {
                // SET TO CRAWLING STATUS
                string statusLog = AzureStorageManager.UpdateThreadStatus(KeyIdentifier, Thread.CurrentThread.Name, "crawling");
                PerformLoggingOperations(statusLog);
                PerformLoggingOperations(urlString + " Has Been Grabbed From Queue!");
                return urlString;
            }
            else
            {
                // SET TO IDLE STATUS
                string statusLog = AzureStorageManager.UpdateThreadStatus(KeyIdentifier, Thread.CurrentThread.Name, "idle");
                PerformLoggingOperations(statusLog);
                PerformLoggingOperations("There are no more urls from Seed: " + WebCrawler.seedUrl);
                string changeUrlLog = AzureStorageManager.UpdateCurrentlyCrawledUrlTableEntity(KeyIdentifier, Thread.CurrentThread.Name, "none");
                PerformLoggingOperations(changeUrlLog);
                return null;
            }
        }

        /// <summary>
        /// Adds an array of urls to the queue
        /// </summary>
        /// <param name="urls"></param>
        private static void PerformQueueOperationOnUrls(string[] urls)
        {
            foreach (string url in urls)
            {
                try
                { 
                    // cancels the process if the thread is switched off
                    if (!CheckIfThreadIsSwitched())
                    {
                        break;
                    }
                }
                catch
                {
                }
                // adds url to queue
                string log = AzureStorageManager.AddUrlToQueue(url);                
                PerformLoggingOperations(log);                
            }
        }

        /// <summary>
        /// PerformStartUpQueueOperationOnUrls that only happens on startup or after restarting - process will not be canceled even if the threads are switched off
        /// </summary>
        /// <param name="urls"></param>
        private static void PerformStartUpQueueOperationOnUrls(string[] urls)
        {
            foreach (string url in urls)
            {
                string log = AzureStorageManager.AddUrlToQueue(url);
                PerformLoggingOperations(log);
            }                
        }

        /// <summary>
        /// Saves report log messages to a queue
        /// </summary>
        /// <param name="logMessage"></param>
        private static void PerformLoggingOperations(string logMessage)
        {
            Trace.TraceInformation(Thread.CurrentThread.Name + " : " + logMessage);
            AzureStorageManager.AddLogMessage(Thread.CurrentThread.Name + " : " + logMessage);
            AzureStorageManager.AddIndividualLogMessage(Thread.CurrentThread.Name + " : " + logMessage);
        }             

        /// <summary>
        /// process for reading the state of the machine counters
        /// </summary>
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

        /// <summary>
        /// checks the table containing the switch states of each thread of the crawler
        /// </summary>
        /// <returns>the switch state</returns>
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
