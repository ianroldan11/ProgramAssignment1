using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace WebRole1
{
    /// <summary>
    /// Summary description for AzureStorageManager
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class AzureStorageManager : System.Web.Services.WebService
    {
        private static readonly string HTMLDataMainTableReference = "htmldatatable";
        private static readonly string MachineCountersTableReference = "machinecounterstable";
        private static readonly string CurrentlyCrawledUrlsTableReference = "currentlycrawledurlstable";
        private static readonly string NumberOfCrawledUrlsTableReference = "numberofcrawledurlstable";
        private static readonly string ThreadStatusTableReference = "threadstatustable";
        private static readonly string HTMLBodyTextBlobContainerReference = "htmlbodyblobcontainer";
        private static readonly string ControllerQueueReference = "controllerqueue";
        private static readonly string CNNUrlQueueReference = "cnnurlqueue";

        private static readonly string SIUrlQueueReference = "siurlqueue";
        public static readonly string MainLogQueueReference = "logqueue";
        public static readonly string ErrorLogQueueReference = "errorlogqueue";
        public static readonly string CNNLogQueueReference = "cnnlogqueue";
        public static readonly string SILogQueueReference = "silogqueue";

        // Cloud Storage Instantiation------------------------------------------------------------------------------------------------------------------
        private static CloudStorageAccount GetCloudStorageAccount()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
            return storageAccount;
        }

        private static CloudQueue GetCloudQueue(string queueReference)
        {
            CloudStorageAccount storageAccount = GetCloudStorageAccount();
            CloudQueueClient QueueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue Queue = QueueClient.GetQueueReference(queueReference);

            return Queue;
        }

        private static CloudTable GetCloudTable(string tableReference)
        {
            CloudStorageAccount storageAccount = GetCloudStorageAccount();
            CloudTableClient TableClient = storageAccount.CreateCloudTableClient();
            CloudTable Table = TableClient.GetTableReference(tableReference);

            return Table;
        }

        private static CloudBlockBlob GetCloudBlockBlob(string blobContainerReference, string blockBlobReference)
        {
            CloudStorageAccount storageAccount = GetCloudStorageAccount();
            CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = BlobClient.GetContainerReference(blobContainerReference);
            CloudBlockBlob BlockBlob = container.GetBlockBlobReference(blockBlobReference);

            return BlockBlob;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------

        // Queue Methods------------------------------------------------------------------------------------------------------------------------------------
        [WebMethod]
        public void AddUrlToQueue(string url, string queueReference)
        {
            AddMessageToQueue(url, queueReference);
        }

        [WebMethod]
        public void AddLogToQueue(string log, string queueReference)
        {
            AddMessageToQueue(log, queueReference);
        }

        public static void AddMessageToQueue(string messageString, string queueReference)
        {
            try
            {
                CloudQueue urlQueue = GetCloudQueue(queueReference);
                CloudQueueMessage message = new CloudQueueMessage(messageString);
                urlQueue.AddMessage(message);
            }
            catch
            {

            }            
        }

        [WebMethod]
        public string RetrieveMessageFromQueue(string queueReference)
        {
            try
            {
                if (CountMessagesInQueue(queueReference) > 0)
                {
                    CloudQueue urlQueue = GetCloudQueue(queueReference);
                    CloudQueueMessage message = urlQueue.GetMessage(TimeSpan.FromMinutes(5));
                    string msg = message.AsString;
                    urlQueue.DeleteMessage(message);
                    return msg;
                }
                else
                {
                    return null;
                }                
            }
            catch
            {
                return null;
            }            
        }

        [WebMethod]
        public int? CountMessagesInQueue(string queueReference)
        {
            try
            {
                CloudQueue urlQueue = GetCloudQueue(queueReference);
                urlQueue.FetchAttributes();

                return urlQueue.ApproximateMessageCount;
            }

            catch
            {
                return null;
            }
            
        }

        // Table Methods------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------
        // Adding entities--------------------
        [WebMethod]
        public void AddHTMLDataEntityToTable(string keyWord, string url, DateTime? pubDate, string blobFile, string title)
        {
            try
            {
                HTMLData dataObject = new HTMLData(keyWord, url, pubDate, blobFile, title);
                CloudTable table = GetCloudTable(HTMLDataMainTableReference);
                TableOperation insertOperation = TableOperation.InsertOrReplace(dataObject);
                table.Execute(insertOperation);
            }
            catch
            {

            }            
        }
        //-----------------------

        public IEnumerable<HTMLData> RetrieveEntitiesWithSameKey(string PropertyName, string Value)
        {
            try
            {
                CloudTable table = GetCloudTable(HTMLDataMainTableReference);
                TableQuery<HTMLData> query = new TableQuery<HTMLData>().Where(TableQuery.GenerateFilterCondition(PropertyName, QueryComparisons.Equal, Value));

                return table.ExecuteQuery(query);
            }

            catch
            {
                return null;
            }            
        }


        public IEnumerable<HTMLData> RetrieveEntitiesWithSameRowKey(string RowKey)
        {
            try
            {
                return RetrieveEntitiesWithSameKey("RowKey", RowKey);
            }

            catch
            {
                return null;
            }            
        }

        [WebMethod]
        public int GetEntityCount(string PropertyName, string Value)
        {
            return RetrieveEntitiesWithSameKey(PropertyName, Value).Count();
        }

        [WebMethod]
        public void IncrementPopularityCount(string url)
        {
            CloudTable table = GetCloudTable(HTMLDataMainTableReference);
            TableQuery<HTMLData> query = new TableQuery<HTMLData>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, WebUtility.UrlEncode(url)));

            foreach (HTMLData data in table.ExecuteQuery(query))
            {
                try
                {
                    data.PopularityCount += 1;
                    TableOperation updateOperation = TableOperation.Merge(data);
                    table.Execute(updateOperation);
                }
                catch
                {

                }                
            }
        }

        [WebMethod]
        public void UpdateMachineCountersValue(double[] values)
        {
            CloudTable table = GetCloudTable(MachineCountersTableReference);
            TableQuery<MachineCountersEntity> query = new TableQuery<MachineCountersEntity>();

            int index = 0;
            foreach (MachineCountersEntity counter in table.ExecuteQuery(query))
            {
                try
                {
                    counter.Value = values[index];
                    TableOperation updateOperation = TableOperation.Merge(counter);
                    table.Execute(updateOperation);
                }
                catch
                {
                }                
                index++;
            }
        }

        [WebMethod]
        public void UpdateCurrentlyCrawledUrlValue(string threadName, string url)
        {
            CloudTable table = GetCloudTable(CurrentlyCrawledUrlsTableReference);
            TableQuery<CurrentlyCrawledUrlEntity> query = new TableQuery<CurrentlyCrawledUrlEntity>();

            TableOperation retrieveOperation = TableOperation.Retrieve<CurrentlyCrawledUrlEntity>("WebCrawler", threadName);
            TableResult retrievedResult = table.Execute(retrieveOperation);
            CurrentlyCrawledUrlEntity updateEntity = (CurrentlyCrawledUrlEntity)retrievedResult.Result;

            try
            {
                if (updateEntity != null)
                {
                    updateEntity.Url = WebUtility.UrlEncode(url);
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                }
            }
            catch
            {

            }
            
        }

        [WebMethod]
        public void IncrementNumberOfCrawledUrls(string rowKey)
        {
            CloudTable table = GetCloudTable(NumberOfCrawledUrlsTableReference);
            TableQuery<NumberOfCrawledUrlsEntity> query = new TableQuery<NumberOfCrawledUrlsEntity>();

            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<NumberOfCrawledUrlsEntity>("WebCrawler", rowKey);
                TableResult retrievedResult = table.Execute(retrieveOperation);
                NumberOfCrawledUrlsEntity updateEntity = (NumberOfCrawledUrlsEntity)retrievedResult.Result;

                if (updateEntity != null)
                {
                    updateEntity.Value += 1;
                    TableOperation updateOperation = TableOperation.Merge(updateEntity);
                    table.Execute(updateOperation);
                }
            }
            catch
            {

            }            
        }

        [WebMethod]
        public void UpdateThreadStatus(string rowKey, string status)
        {
            CloudTable table = GetCloudTable(ThreadStatusTableReference);
            TableQuery<ThreadStatusEntity> query = new TableQuery<ThreadStatusEntity>();
            ThreadStatusEntity updateEntity = (ThreadStatusEntity)PerformOperationToGetResult(table, rowKey);

            if (updateEntity != null)
            {
                updateEntity.Status = status;
                TableOperation updateOperation = TableOperation.Replace(updateEntity);
                table.Execute(updateOperation);
            }
        }

        private object PerformOperationToGetResult(CloudTable table, string rowKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<ThreadStatusEntity>("WebCrawler", rowKey);
            TableResult retrievedResult = table.Execute(retrieveOperation);

            return retrievedResult.Result;
        }

        [WebMethod]
        public void CreateBlobFile(string blobFileName, string content)
        {
            CloudBlockBlob blockBlob = GetCloudBlockBlob(HTMLBodyTextBlobContainerReference, blobFileName);
            using (StreamWriter writer = new StreamWriter(blockBlob.OpenWrite()))
            {
                writer.WriteLine(content);
            }
        }

        [WebMethod]
        public void DeleteAllMessagesFromQueue(string queueReference)
        {
            CloudQueue urlQueue = GetCloudQueue(queueReference);
            urlQueue.Clear();
        }

        [WebMethod]
        public void ResetAllNumberOfUrls()
        {
            CloudTable table = GetCloudTable(NumberOfCrawledUrlsTableReference);
            TableQuery<NumberOfCrawledUrlsEntity> query = new TableQuery<NumberOfCrawledUrlsEntity>();

            foreach (NumberOfCrawledUrlsEntity count in table.ExecuteQuery(query))
            {
                count.Value = 0;
                TableOperation updateOperation = TableOperation.Replace(count);
                table.Execute(updateOperation);
            }
        }

        [WebMethod]
        public void DeleteBlobContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(HTMLBodyTextBlobContainerReference);

            try
            {
                //Fetches attributes of container
                container.FetchAttributes();
                container.Delete();
            }
            catch
            {
            }
        }

        [WebMethod]
        public void RecreateBlobContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(HTMLBodyTextBlobContainerReference);

            CreateBlob(container);
        }

        public static void CreateBlob(CloudBlobContainer container)
        {
            try
            {
                container.CreateIfNotExists();
            }
            catch
            {
                Thread.Sleep(5000);
                CreateBlob(container);
            }
        }

        [WebMethod]
        public void DeleteTable()
        {
            CloudTable table = GetCloudTable(HTMLDataMainTableReference);
            TableQuery<HTMLData> query = new TableQuery<HTMLData>();

            foreach (HTMLData data in table.ExecuteQuery(query))
            {
                TableOperation deleteOperation = TableOperation.Delete(data);

                table.Execute(deleteOperation);
            }
        }

        // Methods Used Only by web role below------------------------------------------------------------------------------------------------------------------------
        [WebMethod]
        public void AddControlCommandToQueue(string command)
        {
            AddMessageToQueue(command, ControllerQueueReference);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string RetrieveMachineCounters()
        {
            CloudTable table = GetCloudTable(MachineCountersTableReference);
            TableQuery<MachineCountersEntity> query = new TableQuery<MachineCountersEntity>();

            return new JavaScriptSerializer().Serialize(table.ExecuteQuery(query).Select(x => Math.Round(x.Value, 2)).ToArray());
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CountUrlInQueues()
        {
            int?[] arrayToReturn = { CountMessagesInQueue(CNNUrlQueueReference), CountMessagesInQueue(SIUrlQueueReference) };

            return new JavaScriptSerializer().Serialize(arrayToReturn);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CountCrawledUrls()
        {
            CloudTable table = GetCloudTable(NumberOfCrawledUrlsTableReference);
            TableQuery<NumberOfCrawledUrlsEntity> query = new TableQuery<NumberOfCrawledUrlsEntity>();

            return new JavaScriptSerializer().Serialize(table.ExecuteQuery(query).Select(x => x.Value).ToArray());
        }

        [WebMethod]
        public int CountEntitiesInHTMLDataTable()
        {
            CloudTable table = GetCloudTable(HTMLDataMainTableReference);
            TableQuery<HTMLData> query = new TableQuery<HTMLData>();

            return table.ExecuteQuery(query).Count();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetThreadName()
        {
            CloudTable table = GetCloudTable(ThreadStatusTableReference);
            TableQuery<ThreadStatusEntity> query = new TableQuery<ThreadStatusEntity>();

            return new JavaScriptSerializer().Serialize(table.ExecuteQuery(query).Select(x => x.RowKey).ToArray());
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetThreadStatus()
        {
            CloudTable table = GetCloudTable(ThreadStatusTableReference);
            TableQuery<ThreadStatusEntity> query = new TableQuery<ThreadStatusEntity>();

            return new JavaScriptSerializer().Serialize(table.ExecuteQuery(query).Select(x => x.Status).ToArray());
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCurrentlyCrawledUrl()
        {
            CloudTable table = GetCloudTable(CurrentlyCrawledUrlsTableReference);
            TableQuery<CurrentlyCrawledUrlEntity> query = new TableQuery<CurrentlyCrawledUrlEntity>();

            return new JavaScriptSerializer().Serialize(table.ExecuteQuery(query).Select(x => x.Url).ToArray());
        }

        public string RetrieveLogFromQueue(string queueReference)
        {
            if (CountMessagesInQueue(queueReference) > 0)
            {
                try
                {
                    return RetrieveMessageFromQueue(queueReference, true);
                }
                catch
                {
                    return null;
                }                
            }
            else
            {
                return null;
            }
        }

        public string RetrieveMessageFromQueue(string queueReference, bool withTimeStamp)
        {
            CloudQueue urlQueue = GetCloudQueue(queueReference);
            CloudQueueMessage message = urlQueue.GetMessage(TimeSpan.FromMinutes(5));
            string msg = message.AsString;


            if (withTimeStamp)
            {
                msg = "[" + message.InsertionTime.ToString() + "] - " + msg;
            }

            urlQueue.DeleteMessage(message);

            return msg;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string RetrieveLogReports()
        {
            string[] arrayToReturn = {
            RetrieveLogFromQueue(MainLogQueueReference),
            RetrieveLogFromQueue(CNNLogQueueReference),
            RetrieveLogFromQueue(SILogQueueReference),
            RetrieveLogFromQueue(ErrorLogQueueReference),
            RetrieveLogFromQueue("trieerrorlogqueue")
            };

            return new JavaScriptSerializer().Serialize(arrayToReturn);
        }
        // Methods Used Only by Search Page below------------------------------------------------------------------------------------------------------------------------
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string RetrieveSearchResults(string keyword)
        {
            if (HttpContext.Current.Cache["(Results) " + keyword] != null)
            {
                return HttpContext.Current.Cache["(Results) " + keyword].ToString();
            }
            
            string[] words = keyword.Split(' ');            
            CloudTable table = GetCloudTable(HTMLDataMainTableReference);
            List<HTMLData> dataSet = new List<HTMLData>();

            foreach (string word in words)
            {
                if (word != "")
                {
                    TableQuery<HTMLData> query = new TableQuery<HTMLData>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, word.ToLower()));
                    dataSet.AddRange(table.ExecuteQuery(query));
                }                                
            }
            
            HttpContext.Current.Cache.Insert("(Results) " + keyword, new JavaScriptSerializer().Serialize(dataSet.Select(x => { x.RowKey = WebUtility.UrlDecode(x.RowKey); return x; }).GroupBy(x => x.RowKey).Select(x => new Tuple<int, HTMLData>(x.ToList().Count, x.ElementAt(0))).OrderByDescending(x => x.Item1).ThenByDescending(x => x.Item2.PopularityCount).Select(x => x.Item2).Take(50)), null, DateTime.Now.AddMinutes(1440), System.Web.Caching.Cache.NoSlidingExpiration);
            return new JavaScriptSerializer().Serialize(dataSet.Select(x => { x.RowKey = WebUtility.UrlDecode(x.RowKey); return x; }).GroupBy(x => x.RowKey).Select(x => new Tuple<int, HTMLData>(x.ToList().Count, x.ElementAt(0))).OrderByDescending(x => x.Item1).ThenByDescending(x => x.Item2.PublishDate.HasValue).ThenByDescending(x => x.Item2.PublishDate).ThenByDescending(x => x.Item2.PopularityCount).Select(x => x.Item2).Take(50));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetContentFromBlobFile(string blockBlobReference)
        {
            CloudBlockBlob blockBlob = GetCloudBlockBlob(HTMLBodyTextBlobContainerReference, blockBlobReference);

            using (var stream = blockBlob.OpenRead())
            {
                using (StreamReader reader = new StreamReader(stream))
                {                    
                    return new JavaScriptSerializer().Serialize(reader.ReadToEnd());
                }
            }
        }

        public void SaveToCache()
        {
            List<object> list = new List<Object>();

            HttpContext.Current.Cache["ObjectList"] = list;
        }
    }
}
