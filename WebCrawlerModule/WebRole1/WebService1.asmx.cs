using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace WebRole1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {                
        private static Thread retrievingThread;
        private static readonly int numberOfWebCrawlers = 5;
        private static readonly int numberOfThreadsPerWebCrawler = 4;

        public static string[] BleacherReportMachCntr = new string[2];
        public static string[] CnnMachCntr = new string[2];
        public static string[] EspnMachCntr = new string[2];
        public static string[] ForbesMachCntr = new string[2];
        public static string[] ImdbMachCntr = new string[2];

        public static int TotalNumberOfDataInTable;
        public static int TotalNumberOfBlobFiles;
        public static int TotalNumberOfUrlsCrawled;

        public static int BleacherReportQueueSize;
        public static int CNNQueueSize;
        public static int ESPNQueueSize;
        public static int ForbesQueueSize;
        public static int IMDBQueueSize;

        public static int BleacherReportStoppedThreadCount;
        public static int CNNStoppedThreadCount;
        public static int ESPNStoppedThreadCount;
        public static int ForbesStoppedThreadCount;
        public static int IMDBStoppedThreadCount;

        public static int[] NumberOfUrlsCrawled = new int[numberOfWebCrawlers];

        public static string[] BleacherReportThreadsStatus          = new string[numberOfThreadsPerWebCrawler];
        public static string[] CNNThreadsStatus                     = new string[numberOfThreadsPerWebCrawler];
        public static string[] ESPNThreadsStatus                    = new string[numberOfThreadsPerWebCrawler];
        public static string[] ForbesThreadsStatus                  = new string[numberOfThreadsPerWebCrawler];
        public static string[] IMDBThreadsStatus                    = new string[numberOfThreadsPerWebCrawler];

        public static string[] BleacherReportCurrentUrlsCrawled     = new string[numberOfThreadsPerWebCrawler];
        public static string[] CNNCurrentUrlsCrawled                = new string[numberOfThreadsPerWebCrawler];
        public static string[] ESPNCurrentUrlsCrawled               = new string[numberOfThreadsPerWebCrawler];
        public static string[] ForbesCurrentUrlsCrawled             = new string[numberOfThreadsPerWebCrawler];
        public static string[] IMDBCurrentUrlsCrawled               = new string[numberOfThreadsPerWebCrawler];

        public static int[] BleacherReportSwitchStates              = new int[numberOfThreadsPerWebCrawler];
        public static int[] CNNSwitchStates                         = new int[numberOfThreadsPerWebCrawler];
        public static int[] ESPNSwitchStates                        = new int[numberOfThreadsPerWebCrawler];
        public static int[] ForbesSwitchStates                      = new int[numberOfThreadsPerWebCrawler];
        public static int[] IMDBSwitchStates                        = new int[numberOfThreadsPerWebCrawler];

        public static string[] LastTenUrlsCrawled                   = new string[10];

        /// <summary>
        /// start the Thread that reads the data frome azure storages
        /// </summary>
        [WebMethod]
        public void StartAutoRetrieve()
        {
            if (retrievingThread == null)
            {
                retrievingThread = new Thread(new ThreadStart(RetrievingFunction));
                retrievingThread.Name = "Retrieving Thread";
                retrievingThread.Start();
            }
        }

        /// <summary>
        /// method to get all machine counter values of each webcrawler
        /// </summary>
        /// <returns>2D array containing machine counters of each web crawler</returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetAllMachineCounterValues()
        {
            string[][] array = { BleacherReportMachCntr, CnnMachCntr, EspnMachCntr, ForbesMachCntr, ImdbMachCntr };
            return new JavaScriptSerializer().Serialize(array);
        }

        /// <summary>
        /// method to get the total indexed data of each storage type
        /// </summary>
        /// <returns>array containing number of entities stored in table, blob and urls</returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetAllIndexedDataCount()
        {
            int[] array = { TotalNumberOfDataInTable, TotalNumberOfBlobFiles, TotalNumberOfUrlsCrawled };
            return new JavaScriptSerializer().Serialize(array);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetAllURLQueueSize()
        {
            int[] array = { BleacherReportQueueSize, CNNQueueSize, ESPNQueueSize, ForbesQueueSize, IMDBQueueSize };
            return new JavaScriptSerializer().Serialize(array);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetAllStoppedThreadCount()
        {
            int[] array = { BleacherReportStoppedThreadCount, CNNStoppedThreadCount, ESPNStoppedThreadCount, ForbesStoppedThreadCount, IMDBStoppedThreadCount };
            return new JavaScriptSerializer().Serialize(array);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetWebLogs()
        {
            string[] array = { GetWebLogFromQueue("logqueue"), GetWebLogFromQueue("bleacherreportlogqueue"), GetWebLogFromQueue("cnnlogqueue"), GetWebLogFromQueue("espnlogqueue"), GetWebLogFromQueue("forbeslogqueue"), GetWebLogFromQueue("imdblogqueue") };

            return new JavaScriptSerializer().Serialize(array);
        }

        [WebMethod]
        public void StartAll()
        {
            AzureStorageManager.SetAllSwitches(true);
            AzureStorageManager.ChangeStatusAfterSwitching("starting");
        }

        [WebMethod]
        public void StopAll()
        {
            AzureStorageManager.SetAllSwitches(false);
            AzureStorageManager.ChangeStatusAfterSwitching("stopping (finishing last task)");
        }

        [WebMethod]
        public int GetNumberOfUrlsCrawledForWebCrawler(int webCrawlerIndex)
        {
            return NumberOfUrlsCrawled[webCrawlerIndex];
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetThreadStatusOfWebCrawler(int webCrawlerIndex)
        {
            switch (webCrawlerIndex)
            {
                case 0:
                    return new JavaScriptSerializer().Serialize(BleacherReportThreadsStatus);
                case 1:
                    return new JavaScriptSerializer().Serialize(CNNThreadsStatus);                    
                case 2:
                    return new JavaScriptSerializer().Serialize(ESPNThreadsStatus);                    
                case 3:
                    return new JavaScriptSerializer().Serialize(ForbesThreadsStatus);                    
                case 4:
                    return new JavaScriptSerializer().Serialize(IMDBThreadsStatus);                    
                default:
                    return null;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCurrentURLCrawledOfWebCrawler(int webCrawlerIndex)
        {
            switch (webCrawlerIndex)
            {
                case 0:
                    return new JavaScriptSerializer().Serialize(BleacherReportCurrentUrlsCrawled);
                case 1:
                    return new JavaScriptSerializer().Serialize(CNNCurrentUrlsCrawled);
                case 2:
                    return new JavaScriptSerializer().Serialize(ESPNCurrentUrlsCrawled);
                case 3:
                    return new JavaScriptSerializer().Serialize(ForbesCurrentUrlsCrawled);
                case 4:
                    return new JavaScriptSerializer().Serialize(IMDBCurrentUrlsCrawled);
                default:
                    return null;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetSwitchStates(int webCrawlerIndex)
        {
            switch (webCrawlerIndex)
            {
                case 0:
                    return new JavaScriptSerializer().Serialize(BleacherReportSwitchStates);
                case 1:
                    return new JavaScriptSerializer().Serialize(CNNSwitchStates);
                case 2:
                    return new JavaScriptSerializer().Serialize(ESPNSwitchStates);
                case 3:
                    return new JavaScriptSerializer().Serialize(ForbesSwitchStates);
                case 4:
                    return new JavaScriptSerializer().Serialize(IMDBSwitchStates);
                default:
                    return null;
            }
        }

        [WebMethod]
        public void StartSpecificSwitch(string partitionKey, string rowKey)
        {
            AzureStorageManager.SetSpecificSwitch(true, partitionKey, rowKey);
        }

        [WebMethod]
        public void StopSpecificSwitch(string partitionKey, string rowKey)
        {
            AzureStorageManager.SetSpecificSwitch(false, partitionKey, rowKey);
        }

        [WebMethod]
        public string GetRecentlyCrawledUrls(int webCrawlerIndex)
        {
            string partitionKey;
            switch (webCrawlerIndex)
            {
                case 0:
                    partitionKey = "bleacherreport.com";
                    break;
                case 1:
                    partitionKey = "cnn.com";
                    break;
                case 2:
                    partitionKey = "espn.com";
                    break;
                case 3:
                    partitionKey = "forbes.com";
                    break;
                case 4:
                    partitionKey = "imdb.com";
                    break;
                default:
                    return null;
            }
            LastTenUrlsCrawled = GetLast10UrlsCrawled(partitionKey);
            return new JavaScriptSerializer().Serialize(LastTenUrlsCrawled);           
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetExtractedData(int webCrawlerIndex, string rowKey)
        {
            string partitionKey;
            switch (webCrawlerIndex)
            {
                case 0:
                    partitionKey = "bleacherreport.com";
                    break;
                case 1:
                    partitionKey = "cnn.com";
                    break;
                case 2:
                    partitionKey = "espn.com";
                    break;
                case 3:
                    partitionKey = "forbes.com";
                    break;
                case 4:
                    partitionKey = "imdb.com";
                    break;
                default:
                    return null;
            }

            HTMLData data = AzureStorageManager.GetHTMLDataElement(partitionKey, rowKey);
            return new JavaScriptSerializer().Serialize(data);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetHTMLBodyContent(string blobFileReference)
        {
            
            return new JavaScriptSerializer().Serialize(AzureStorageManager.GetContentFromBlobFile(blobFileReference));
        }

        [WebMethod]
        public void RequestForReset()
        {
            ResetAllCrawlers();
        }

        // boolean flag indicating if webservice should continue collecting data from the storage - stops temporarily when resetting
        private bool isNotResetting = true;

        /// <summary>
        /// method that runs over time to collect up-to-date data from the azure storage
        /// </summary>
        private void RetrievingFunction()
        {
            while (isNotResetting)
            {
                // Data collection for machine counters
                BleacherReportMachCntr = GetMachineCounterValues("Bleacher Report");
                CnnMachCntr = GetMachineCounterValues("CNN");
                EspnMachCntr = GetMachineCounterValues("ESPN");
                ForbesMachCntr = GetMachineCounterValues("Forbes");
                ImdbMachCntr = GetMachineCounterValues("IMDB");
                
                // Data collection for total number of URL data stored in every type of storage
                TotalNumberOfDataInTable = AzureStorageManager.GetURLCountInStorage("numberofdataintable", "All").Urls;
                TotalNumberOfBlobFiles = AzureStorageManager.GetURLCountInStorage("numberofblobfiles", "All").Urls;
                TotalNumberOfUrlsCrawled = AzureStorageManager.GetURLCountInStorage("numberurlscrawledtable", "All").Urls;

                // Collection for the current size of url queues of web crawlers
                BleacherReportQueueSize = AzureStorageManager.GetQueueCount("bleachreporturlqueue");
                CNNQueueSize = AzureStorageManager.GetQueueCount("cnnurlqueue");
                ESPNQueueSize = AzureStorageManager.GetQueueCount("espnurlqueue");
                ForbesQueueSize = AzureStorageManager.GetQueueCount("forbesurlqueue");
                IMDBQueueSize = AzureStorageManager.GetQueueCount("imdburlqueue");

                // Collection for every number of stopped threads in every web crawlers
                BleacherReportStoppedThreadCount = CountStoppedThreads("Bleacher Report");
                CNNStoppedThreadCount = CountStoppedThreads("CNN"); ;
                ESPNStoppedThreadCount = CountStoppedThreads("ESPN"); ;
                ForbesStoppedThreadCount = CountStoppedThreads("Forbes"); ;
                IMDBStoppedThreadCount = CountStoppedThreads("IMDB");

                // Collection for every number of urls crawled by each web crawler
                NumberOfUrlsCrawled = GetNumberUrlsCrawledForEachDomain();

                // Collection for every thread status of all web crawlers
                BleacherReportThreadsStatus = GetStatusFromListOfThreadStatusEntities("Bleacher Report");
                CNNThreadsStatus = GetStatusFromListOfThreadStatusEntities("CNN");
                ESPNThreadsStatus = GetStatusFromListOfThreadStatusEntities("ESPN");
                ForbesThreadsStatus = GetStatusFromListOfThreadStatusEntities("Forbes");
                IMDBThreadsStatus = GetStatusFromListOfThreadStatusEntities("IMDB");

                // Collection of data for every currently crawled urls by all threads across web crawlers
                BleacherReportCurrentUrlsCrawled = GetUrlsFromListOfCurrentUrls("Bleacher Report");
                CNNCurrentUrlsCrawled = GetUrlsFromListOfCurrentUrls("CNN");
                ESPNCurrentUrlsCrawled = GetUrlsFromListOfCurrentUrls("ESPN");
                ForbesCurrentUrlsCrawled = GetUrlsFromListOfCurrentUrls("Forbes");
                IMDBCurrentUrlsCrawled = GetUrlsFromListOfCurrentUrls("IMDB");

                // switch states (on/off) of every thread
                BleacherReportSwitchStates = GetSwitchStatesFromListOfSwitchStates("Bleacher Report");
                CNNSwitchStates = GetSwitchStatesFromListOfSwitchStates("CNN");
                ESPNSwitchStates = GetSwitchStatesFromListOfSwitchStates("ESPN");
                ForbesSwitchStates = GetSwitchStatesFromListOfSwitchStates("Forbes");
                IMDBSwitchStates = GetSwitchStatesFromListOfSwitchStates("IMDB");

                
                Thread.Sleep(100);
            }            
        }       
        
        /// <summary>
        /// method to get machine counters of a web crawler
        /// </summary>
        /// <param name="partitionKey">key for web crawler</param>
        /// <returns>cpu usage and available ram</returns>
        private string[] GetMachineCounterValues(string partitionKey)
        {
            string[] array = new string[2];

            array[0] = AzureStorageManager.RetrieveEntityFromMachineCountersTable(partitionKey, "CPU Usage").Value.ToString();
            array[1] = AzureStorageManager.RetrieveEntityFromMachineCountersTable(partitionKey, "Available Memory").Value.ToString();

            return array;
        }

        /// <summary>
        /// method to get number of stopped threads of a web crawler
        /// </summary>
        /// <param name="partitionKey">key for web crawler</param>
        /// <returns>number of stopeed threads</returns>
        private int CountStoppedThreads(string partitionKey)
        {
            List<ThreadStatus> threads = AzureStorageManager.GetAllThreadStatus(partitionKey);
            int stoppedThreadCount = 0;
            foreach (ThreadStatus threadEntity in threads)
            {
                if (threadEntity.Status.Equals("stopped"))
                {
                    stoppedThreadCount++;
                }
            }

            return stoppedThreadCount;
        }

        /// <summary>
        /// method to get web log message from a log queue
        /// </summary>
        /// <param name="queueReference">reference of the queue</param>
        /// <returns>a message report log</returns>
        private string GetWebLogFromQueue(string queueReference)
        {
            return AzureStorageManager.GetMessageFromLog(queueReference);
        }

        /// <summary>
        /// Method to get the number of urls crawled by each web crawler
        /// </summary>
        /// <returns>array containing values of every web crawler</returns>
        private int[] GetNumberUrlsCrawledForEachDomain()
        {
            string[] sites = { "Bleacher Report", "CNN", "ESPN", "Forbes", "IMDB" };
            int[] arrayToReturn = new int[numberOfWebCrawlers];
            int index = 0;
            foreach (string site in sites)
            {
                arrayToReturn[index] =  AzureStorageManager.GetURLCountInStorage("numberurlscrawledtable", site).Urls;
                index++;
            }

            return arrayToReturn;
        }

        /// <summary>
        /// method that gets all table entities from the switcher table of a single partition then gets the status of each
        /// </summary>
        /// <param name="partitionKey">key for web crawler</param>
        /// <returns>returns the string that contains the status value from the table entity</returns>
        private string[] GetStatusFromListOfThreadStatusEntities(string partitionKey)
        {
            List<ThreadStatus> threads = AzureStorageManager.GetAllThreadStatus(partitionKey);
            List<string> status = new List<string>();
            foreach (ThreadStatus threadStatus in threads)
            {
                status.Add(threadStatus.Status);
            }

            return status.ToArray();
        }

        /// <summary>
        /// Gets all currently crawled url of all threads in a web crawler
        /// </summary>
        /// <param name="partitionKey">key for web crawler</param>
        /// <returns>array of currently crawled urls</returns>
        private string[] GetUrlsFromListOfCurrentUrls(string partitionKey)
        {
            List<CurrentURLEntity> urls = AzureStorageManager.GetAllCurrentlyCrawledURL(partitionKey);
            List<string> urlList = new List<string>();
            foreach (CurrentURLEntity url in urls)
            {                
                urlList.Add(System.Net.WebUtility.UrlDecode(url.URL));
            }

            return urlList.ToArray();
        }

        /// <summary>
        /// get all switch states for all threads of a web crawler
        /// </summary>
        /// <param name="partitionKey">key for web crawler</param>
        /// <returns>switch states as 0 or 1</returns>
        private int[] GetSwitchStatesFromListOfSwitchStates(string partitionKey)
        {
            List<ThreadSwitch> switches = AzureStorageManager.GetAllSwitchStates(partitionKey);
            List<int> urlList = new List<int>();
            foreach (ThreadSwitch switcher in switches)
            {
                
                urlList.Add(Convert.ToInt32(switcher.IsSwitched));
            }

            return urlList.ToArray();
        }

        /// <summary>
        /// gets the list of last 10 crawled urls by a web crawler
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        private string[] GetLast10UrlsCrawled(string partitionKey)
        {
            // get all entities from azure table with specified partitionKey and return as list
            List<HTMLData> listOfData = AzureStorageManager.GetAllElementsUnderSamePartition(partitionKey);
            // sort list by date by descending order
            listOfData.Sort((x, y) => y.Timestamp.CompareTo(x.Timestamp));
            // return only 10 of those list
            List<HTMLData> firstTenItems = listOfData.Take(10).ToList();
            List<string> urls = new List<string>();
            foreach(HTMLData data in firstTenItems)
            {
                urls.Add(System.Net.WebUtility.UrlDecode(data.RowKey));
            }
            return urls.ToArray();
        }

        /// <summary>
        /// method for the entire procedure for resetting all web crawlers
        /// </summary>
        private void ResetAllCrawlers()
        {
            // temporarily stops this web service from collecting data in the storages
            isNotResetting = false;
            // sets all thread switches to off
            StopAll();
            // deletes all messages from all queues
            string[] queueReferences = { "bleachreporturlqueue", "cnnurlqueue", "espnurlqueue", "forbesurlqueue", "imdburlqueue" };
            foreach(string queue in queueReferences)
            {
                AzureStorageManager.DeleteAllMessagesFromQueue(queue);

            }
            //empty blob container
            AzureStorageManager.DeleteBlobContainer("htmlbodyblobcontainer");
            AzureStorageManager.RecreateBlobContainer("htmlbodyblobcontainer");
            //empty table
            AzureStorageManager.DeleteTable("htmldatatable");
            AzureStorageManager.RecreateTable("htmldatatable");
            //set all number of stored data in storages to 0
            AzureStorageManager.ResetAllNumberOfUrls();
            // clear report log queue
            string[] logQueueReferences = { "bleacherreportlogqueue", "cnnlogqueue", "espnlogqueue", "forbeslogqueue", "imdblogqueue", "logqueue" };
            foreach (string queue in logQueueReferences)
            {
                AzureStorageManager.DeleteAllMessagesFromQueue(queue);
            }
            AzureStorageManager.SwitchAllResetFlags();
            // sets all thread switches to on
            StartAll();
            // start collecting data
            isNotResetting = true;
        }
    }
}
