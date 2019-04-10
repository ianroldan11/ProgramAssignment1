using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace WebRole1.trie
{
    public enum DataState
    {        
        Uninitialized,
        Downloading,
        Preparing,
        Ready
    }
    /// <summary>
    /// Summary description for TrieManager
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class TrieManager : System.Web.Services.WebService
    {
        public static Trie querrySuggestionTrie;
        public static string[] WikiDataList;
        public static string[] ViewsDataList;

        public static string WikiDataFileName = "wikidataset";
        public static string ViewsDataFileName = "viewsdataset";
        public static string DownloadDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static Thread WikiDataDownloaderThread;
        public static Thread ViewsDataDownloaderThread;
        public static Thread MachineCounterThread;

        public static DataState TrieData = DataState.Uninitialized;
        public static DataState PopCountData = DataState.Uninitialized;

        private static PerformanceCounter theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private static PerformanceCounter theMemCounter = new PerformanceCounter("Memory", "Available MBytes");

        public static double CpuUsageValue;
        public static double AvailableMemoryValue;

        public static List<Tuple<string, int>> RecentlySearchedData = new List<Tuple<string, int>>();

        public static int FinishedViewsDataCount = 0;
        
        
        public void AddRecentlySearchedData(Tuple<string, int> dataToBeSaved)
        {
            // check if already in list
            var sameData = RecentlySearchedData.Select(x => x).Where(x => x.Item1 == dataToBeSaved.Item1);
            if (sameData.Count() > 0)
            {
                // remove old data if already in list
                RecentlySearchedData.RemoveAll(x => x.Item1 == dataToBeSaved.Item1);
            }
            // store new data
            RecentlySearchedData.Add(dataToBeSaved);
            // check if list count is greater than 5    
            if (RecentlySearchedData.Count() > 5)
            {
                // remove first data if list count is greater than 5
                RecentlySearchedData.Remove(RecentlySearchedData.First());
            }
        }

        private static void DownloadWikiDataList()
        {
            TrieData = DataState.Downloading;
            DownloadFromAzureBlob(WikiDataFileName);
            TrieData = DataState.Preparing;
            WikiDataList = ReadFromFile(DownloadDirectory + WikiDataFileName);
            querrySuggestionTrie = new Trie(WikiDataList);
            TrieData = DataState.Ready;
        }

        private static void DownloadViewsDataList()
        {
            PopCountData = DataState.Downloading;
            DownloadFromAzureBlob(ViewsDataFileName);
            PopCountData = DataState.Preparing;
            ViewsDataList = ReadFromFile(DownloadDirectory + ViewsDataFileName);
            SetPopularityCountInTrie();
            PopCountData = DataState.Ready;
        }

        public static void ReportError(string message)
        {
            AzureStorageManager.AddMessageToQueue(message, "trieerrorlogqueue");
        }

        public static void DownloadFromAzureBlob(string fileNameToDownload)
        {
            try
            {
                // check if file does not yet exist
                if (!File.Exists(DownloadDirectory + fileNameToDownload))
                {
                    // get blob container
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference("wikipediablobcontainer");
                    // check if blob container exists
                    if (container.Exists())
                    {
                        // download the file
                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileNameToDownload);
                        using (var fileStream = File.OpenWrite(DownloadDirectory + fileNameToDownload))
                        {
                            blockBlob.DownloadToStream(fileStream);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                ReportError(e.ToString());
            }
            
        }

        /// <summary>
        /// reads from specified file and returns data as string array
        /// </summary>
        /// <param name="fileDirectory"></param>
        /// <returns></returns>
        private static string[] ReadFromFile(string fileDirectory)
        {
            List<string> listOfWords = new List<string>();
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                string line;
                StreamReader sr = new StreamReader(fileDirectory);
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    listOfWords.Add(line);
                    //Read the next line
                    line = sr.ReadLine();
                }
                //close the file
                sr.Close();
            }
            catch(Exception e)
            {
                ReportError(e.ToString());
            }
            return listOfWords.ToArray();
        }

        /// <summary>
        /// counts cpu usage and available ram every interval
        /// </summary>
        public static void MachineCounterInterval()
        {
            while (true)
            {
                AvailableMemoryValue = theMemCounter.NextValue();
                CpuUsageValue = theCPUCounter.NextValue();

                Thread.Sleep(200);
            }  
        }

        /// <summary>
        /// iterates every data viewdatalist and set the popularity to every appropriate entry in the trie
        /// </summary>
        public static void SetPopularityCountInTrie()
        {
            foreach (string viewData in ViewsDataList)
            {
                try
                {                    
                    string[] array = viewData.Split(' ');                    
                    TrieNode targetNode = Trie.GetSpecificEntry(querrySuggestionTrie.RootNode, "", array[1], true);
                    if (targetNode != null)
                    {
                        targetNode.AddPopularity(Int32.Parse(array[2]), array[1]);
                    }
                }
                catch (Exception e)
                {
                    ReportError(e.ToString());
                }
                FinishedViewsDataCount++;
            }
        }

        /// <summary>
        /// starts machine counting
        /// </summary>
        [WebMethod]
        public void StartMachineCounter()
        {
            if (MachineCounterThread == null)
            {
                ThreadStart threadStart = new ThreadStart(MachineCounterInterval);
                MachineCounterThread = new Thread(threadStart);
                MachineCounterThread.Start();                
            }
        }

        /// <summary>
        /// method to download then store all data from wikidataset
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string StartupTrieStructure()
        {
            if (WikiDataDownloaderThread == null)
            {
                ThreadStart threadStart = new ThreadStart(DownloadWikiDataList);
                WikiDataDownloaderThread = new Thread(threadStart);
                WikiDataDownloaderThread.Start();
                return "Preparing Trie Structure";
            }
            else
            {
                return "Trie Structure was or is already being prepared";
            }
        }

        /// <summary>
        /// Consumes data of viewsdataset file to apply popularity count to the stored data in trie
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string ConfigurePopularityCount()
        {
            if (TrieData == DataState.Ready)
            {
                if (ViewsDataDownloaderThread == null && querrySuggestionTrie != null)
                {
                    ThreadStart threadStart = new ThreadStart(DownloadViewsDataList);
                    ViewsDataDownloaderThread = new Thread(threadStart);
                    ViewsDataDownloaderThread.Start();
                    return "Configuring Popularity Count";
                }
                else
                {
                    return "Popularity Count was or is already being configured";
                }
            }
            else
            {
                return "Trie Structure is not yet complete, cannot start Pop Count Configuration";
            }            
        }

        /// <summary>
        /// get the state of the configuration of data (downloading, preparing or ready)
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetDataStates()
        {
            string[] dataStates = { TrieData.ToString(), PopCountData.ToString() };

            return new JavaScriptSerializer().Serialize(dataStates);
        }

        /// <summary>
        /// get number of data stored in wikidataset file and number of data that has been stored in trie
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetWikiDataCount()
        {
            int[] dataCount = new int[2];
            if (WikiDataList == null)
            {
                dataCount[0] = 0;
            }
            else
            {
                dataCount[0] = WikiDataList.Count();
            }
            dataCount[1] = Trie.dataStoredCount;            

            return new JavaScriptSerializer().Serialize(dataCount);
        }

        /// <summary>
        /// method to get machine counter values
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetMachineCounterValues()
        {
            double[] machineCounters = { Math.Round(AvailableMemoryValue, 2), Math.Round(CpuUsageValue, 2) };

            return new JavaScriptSerializer().Serialize(machineCounters);
        }        

        /// <summary>
        /// general method for query suggestion by traversing through the trie
        /// </summary>
        /// <param name="searchWord"></param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetSuggestionResults(string searchWord)
        {
            searchWord = searchWord.Replace(' ', '_');
            List<Tuple<string, int>> listToReturn = new List<Tuple<string, int>>();

            if (TrieData == DataState.Ready)
            {
                try
                {
                    // get target node that matches the word being queried
                    Tuple<TrieNode, string> existingNode = Trie.GetNodeAfterTraversingString("", searchWord, querrySuggestionTrie.RootNode, true);
                    if (existingNode != null)
                    {
                        // retrieve all words under that node
                        foreach (Tuple<string, int> word in Trie.RetrieveAllWordsUnderANode(existingNode.Item1, existingNode.Item2, searchWord).Select(x => x))
                        {
                            listToReturn.Add(word);
                        }
                    }
                    else
                    {
                        // perform traversal with case insensitive format
                        existingNode = Trie.GetNodeAfterTraversingString("", searchWord, querrySuggestionTrie.RootNode, false);
                        if (existingNode != null)
                        {
                            foreach (Tuple<string, int> word in Trie.RetrieveAllWordsUnderANode(existingNode.Item1, existingNode.Item2, searchWord).Select(x => x))
                            {
                                listToReturn.Add(word);
                            }
                        }
                        else
                        {
                            // perform traversal with pascal case format
                            searchWord = string.Join("_", searchWord.Split('_').Select(x => x[0].ToString().ToUpper() + x.Substring(1).ToLower()));
                            existingNode = Trie.GetNodeAfterTraversingString("", searchWord, querrySuggestionTrie.RootNode, true);
                            if (existingNode != null)
                            {
                                foreach (Tuple<string, int> word in Trie.RetrieveAllWordsUnderANode(existingNode.Item1, existingNode.Item2, searchWord).Select(x => x))
                                {
                                    listToReturn.Add(word);
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }  
            // return items while replacing underscores with spaces
            return new JavaScriptSerializer().Serialize(listToReturn.OrderByDescending(x => x.Item2).ThenBy(x => x.Item1).Select(x => new Tuple<string, int>(x.Item1.Replace('_', ' '), x.Item2)).Take(20));
        }

        /// <summary>
        /// add a popularity count to a query suggestion data
        /// </summary>
        /// <param name="stringToTraverse"></param>
        /// <param name="isCaseSensitive"></param>
        [WebMethod]
        public void AddPopularityCountToEntry(string stringToTraverse, int isCaseSensitive)
        {
            stringToTraverse = stringToTraverse.Replace(' ', '_');
            TrieNode targetNode = Trie.GetSpecificEntry(querrySuggestionTrie.RootNode, "", stringToTraverse, Convert.ToBoolean(isCaseSensitive));
            targetNode.AddPopularity(1, stringToTraverse);
        }

        /// <summary>
        /// store data entry to list of recently searched data
        /// </summary>
        /// <param name="stringToTraverse"></param>
        [WebMethod]
        public void AddDataToRecentlySearchedTable(string stringToTraverse)
        {
            stringToTraverse = stringToTraverse.Replace(' ', '_');
            TrieNode targetNode = Trie.GetSpecificEntry(querrySuggestionTrie.RootNode, "", stringToTraverse, true);
            AddRecentlySearchedData(new Tuple<string, int>(stringToTraverse, targetNode.GetPopularityOfEntry(stringToTraverse)));
        }

        /// <summary>
        /// get 5 entries that were recently searched by user
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRecentlySearchedData()
        {            

            return new JavaScriptSerializer().Serialize(RecentlySearchedData.ToArray().Reverse());
        }

        /// <summary>
        /// Get number of items retrieved from viewsdataset file
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetViewsDataCount()
        {
            int[] dataCount = new int[2];
            if (ViewsDataList == null)
            {
                dataCount[0] = 0;
            }
            else
            {
                dataCount[0] = ViewsDataList.Count();
            }
            dataCount[1] = FinishedViewsDataCount;

            return new JavaScriptSerializer().Serialize(dataCount);
        }
    }
}
