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

namespace WebCrawler
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        // Soap client for web service that handles azure storage operations
        public static ASManager.AzureStorageManagerSoapClient ASMClient = new ASManager.AzureStorageManagerSoapClient();

        // Name for all queues that are used by all threads
        public static readonly string MainLogQueueReference = "logqueue";
        public static readonly string ErrorLogQueueReference = "errorlogqueue";
        public static readonly string ControllerQueueReference = "controllerqueue";

        // Crawler Thread objects
        public static Thread CNNCrawler1;
        public static Thread CNNCrawler2;
        public static Thread SICrawler1;
        public static Thread SICrawler2;

        // WebCrawlerManager per different domain sites to be crawled
        public static WebCrawlerManager CNNWebCrawler = new WebCrawlerManager("https://edition.cnn.com/robots.txt", "cnnurlqueue", "cnnlogqueue", "https://edition.cnn.com", "cnn.com");
        public static WebCrawlerManager SIWebCrawler = new WebCrawlerManager("https://www.si.com/robots.txt", "siurlqueue", "silogqueue", "https://www.si.com", "si.com");

        // State machines for each thread
        public CrawlerStateMachine CNNStateMachine1 = new CrawlerStateMachine(true, CNNWebCrawler);
        public CrawlerStateMachine CNNStateMachine2 = new CrawlerStateMachine(false, CNNWebCrawler);
        public CrawlerStateMachine SIStateMachine1 = new CrawlerStateMachine(true, SIWebCrawler);
        public CrawlerStateMachine SIStateMachine2 = new CrawlerStateMachine(false, SIWebCrawler);        

        public override void Run()
        {
            Trace.TraceInformation("WebCrawler is running");

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

            Trace.TraceInformation("WebCrawler has been started");
            // Sets all crawler threads to start
            CNNCrawler1 = StartWorkerThread(CNNCrawler1, "CNN_SPDR", CNNStateMachine1);
            CNNCrawler2 = StartWorkerThread(CNNCrawler2, "CNN_WORM", CNNStateMachine2);
            SICrawler1 = StartWorkerThread(SICrawler1, "SI_SCPN", SIStateMachine1);
            SICrawler2 = StartWorkerThread(SICrawler2, "SI_CPLR", SIStateMachine2);            

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WebCrawler is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WebCrawler has stopped");
        }

        /// <summary>
        /// Performs machine counters as well as waits for message for controls such as STARTING, STOPPING or RESTARTING the webcrawler threads
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                MachineCounters();
                // get queue message
               if (ASMClient.CountMessagesInQueue(ControllerQueueReference) > 0)
                {
                    string controlMessage = ASMClient.RetrieveMessageFromQueue(ControllerQueueReference);
                    if (controlMessage == "stop")
                    {
                        PerformLoggingOperations("STOP Command detected.");
                        SetAllStateMachines(State.Stopped);
                        WebCrawlerManager.StopIterations = true;
                    }
                    else if (controlMessage == "start")
                    {
                        PerformLoggingOperations("START Command detected.");
                        WebCrawlerManager.StopIterations = false;
                        SetAllStateMachines(State.Loading);  
                    }
                    else if (controlMessage == "restart")
                    {
                        PerformLoggingOperations("RESTART Command detected");
                        SetAllStateMachines(State.Restarting);
                        WebCrawlerManager.StopIterations = true;
                        RestartSequence();
                        // set to start
                        WebCrawlerManager.StopIterations = false;
                        SetAllStateMachines(State.Starting);
                        PerformLoggingOperations("RESTART Completed...");
                    }
                    else
                    {
                        PerformErrorLoggingOperations("Error: Control Message is Invalid");
                    }
                }
                await Task.Delay(1000);
            }
        }

        private void RestartSequence()
        {
            try
            {
                // clear url queues
                ASMClient.DeleteAllMessagesFromQueue(CNNWebCrawler.UrlQueueReference);
                ASMClient.DeleteAllMessagesFromQueue(SIWebCrawler.UrlQueueReference);
                // set number of urls crawled to 0
                ASMClient.ResetAllNumberOfUrls();
                // clear table
                ASMClient.DeleteTable();
                // clear blob
                ASMClient.DeleteBlobContainer();
                ASMClient.RecreateBlobContainer();
                // clear logs
                ASMClient.DeleteAllMessagesFromQueue(CNNWebCrawler.LogQueueReference);
                ASMClient.DeleteAllMessagesFromQueue(SIWebCrawler.LogQueueReference);
                ASMClient.DeleteAllMessagesFromQueue(MainLogQueueReference);
                ASMClient.DeleteAllMessagesFromQueue(ErrorLogQueueReference);
            }
            catch
            {
                RestartSequence();
            }
        }

        /// <summary>
        /// Sets all states of thread to stop, start or restart
        /// </summary>
        /// <param name="state"></param>
        private void SetAllStateMachines(State state)
        {
            CNNStateMachine1.CurrentState = state;
            CNNStateMachine2.CurrentState = state;
            SIStateMachine1.CurrentState = state;
            SIStateMachine2.CurrentState = state;
        }

        /// <summary>
        /// Initializer of crawling threads
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="name"></param>
        /// <param name="stateMachine"></param>
        /// <returns></returns>
        private Thread StartWorkerThread(Thread thread, string name, CrawlerStateMachine stateMachine)
        {

            if (thread == null)
            {
                thread = new Thread(new ThreadStart(stateMachine.DoWork));
                thread.Name = name;
                thread.Start();
            }

            return thread;
        }

        /// <summary>
        /// Initializer of machine counters thread
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="name"></param>
        /// <returns></returns>
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

        // Perfomrance counters assessing the CPU usage and available memory
        private static PerformanceCounter theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private static PerformanceCounter theMemCounter = new PerformanceCounter("Memory", "Available MBytes");

        /// <summary>
        /// Retrieves CPU Usage and Available Memory at intervals
        /// </summary>
        private static void MachineCounters()
        {
                
            double ram = theMemCounter.NextValue();
            double cpu = theCPUCounter.NextValue();
            try
            {
                double[] machineCounters = { ram, cpu };
                ASMClient.UpdateMachineCountersValue(machineCounters);
            }
            catch (Exception e)
            {
                PerformErrorLoggingOperations(e.ToString());
            }     
        }

        /// <summary>
        /// adds message log to the general log queue
        /// </summary>
        /// <param name="message"></param>
        public static void PerformLoggingOperations(string message)
        {
            Trace.TraceInformation(Thread.CurrentThread.Name + " : " + message);
            ASMClient.AddLogToQueue(Thread.CurrentThread.Name + " : " + message, MainLogQueueReference);
        }

        /// <summary>
        /// Adds message log to a specific queue
        /// </summary>
        /// <param name="message"></param>
        /// <param name="queueReference"></param>
        public static void PerformLoggingOperationsOnSpecificQueue(string message, string queueReference)
        {
            PerformLoggingOperations(message);
            ASMClient.AddLogToQueue(Thread.CurrentThread.Name + " : " + message, queueReference);
        }

        /// <summary>
        /// Adds Error message log
        /// </summary>
        /// <param name="errorMessage"></param>
        public static void PerformErrorLoggingOperations(string errorMessage)
        {
            Trace.TraceInformation(Thread.CurrentThread.Name + " : " + errorMessage);
            ASMClient.AddLogToQueue(Thread.CurrentThread.Name + " : " + errorMessage, ErrorLogQueueReference);
        }
        


    }
}
