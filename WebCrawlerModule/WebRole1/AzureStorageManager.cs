using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace WebRole1
{
    public class AzureStorageManager
    {
        private static readonly string HtmlBodyBlobContainerReference = "htmlbodyblobcontainer";
        private static readonly string MachineCountersTableReference = "machinecounterstable";
        private static readonly string NumberUrlsCrawledTableReference = "numberurlscrawledtable";
        private static readonly string ThreadStatusTableReference = "threadstatustable";
        private static readonly string ThreadSwitcherTableReference = "threadswitchertable";        
        private static readonly string HTMLDataTableReference = "htmldatatable";

        private static readonly string PartitionKey = "PartitionKey";
        private static readonly string RowKey = "RowKey";
        private static readonly string RestartFlagKey = "RestartFlag";

        /// <summary>
        /// Generic method for getting a cloud table
        /// </summary>
        /// <param name="tableReference">reference name for table</param>
        /// <returns>target table</returns>
        private static CloudTable GetCloudTable(string tableReference)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient TableClient = storageAccount.CreateCloudTableClient();
            CloudTable Table = TableClient.GetTableReference(tableReference);

            return Table;
        }

        /// <summary>
        /// Generic method for getting cloud queue
        /// </summary>
        /// <param name="queueReference">reference name for queue</param>
        /// <returns>target cloud</returns>
        private static CloudQueue GetCloudQueue(string queueReference)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient QueueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue Queue = QueueClient.GetQueueReference(queueReference);

            return Queue;
        }

        /// <summary>
        /// Generic method for getting a block blob
        /// </summary>
        /// <param name="BlockBlobReference">reference name for the file</param>
        /// <returns>target file</returns>
        private static CloudBlockBlob GetCloudBlockBlob(string BlockBlobReference)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(HtmlBodyBlobContainerReference);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(BlockBlobReference);

            return blockBlob;
        }

        /// <summary>
        /// Gets a machine counter entity from the table
        /// </summary>
        /// <param name="partitionKey">Which web crawler</param>
        /// <param name="rowKey">What Counter to get i.e. CPU Usage, Available Ram, Reset Flag</param>
        /// <returns>target entity</returns>
        public static MachineCounterEntity RetrieveEntityFromMachineCountersTable(string partitionKey, string rowKey)
        {
            CloudTable table = GetCloudTable(MachineCountersTableReference);
            TableOperation retrieveOpertaion = TableOperation.Retrieve<MachineCounterEntity>(partitionKey, rowKey);
            TableResult retrievedResult = table.Execute(retrieveOpertaion);
            MachineCounterEntity updateEntity = (MachineCounterEntity)retrievedResult.Result;

            return updateEntity;
        }

        /// <summary>
        /// gets url count data from table
        /// </summary>
        /// <param name="partitionKey">which category i.e. queue, table, blob</param>
        /// <param name="rowKey">which web craw;er</param>
        /// <returns></returns>
        public static UrlCount GetURLCountInStorage(string partitionKey, string rowKey)
        {
            CloudTable table = GetCloudTable(NumberUrlsCrawledTableReference);
            TableOperation retrieveOpertaion = TableOperation.Retrieve<UrlCount>(partitionKey, rowKey);
            TableResult retrievedResult = table.Execute(retrieveOpertaion);
            UrlCount updateEntity = (UrlCount)retrievedResult.Result;

            return updateEntity;
        }

        /// <summary>
        /// Gets approximate count of a queue
        /// </summary>
        /// <param name="queueReference">target queue</param>
        /// <returns>number of messages in the target queue</returns>
        public static int GetQueueCount(string queueReference)
        {            
            CloudQueue queue = GetCloudQueue(queueReference);
            queue.FetchAttributes();

            int? count = queue.ApproximateMessageCount;

            if (count == null)
            {
                return 0;
            }

            else
            {
                return (int)count;
            }            
        }

        /// <summary>
        /// Gets the status of all threads in a web crawler ex. loading, crawling, idle, stopped
        /// </summary>
        /// <param name="partitionKey">which webcrawler</param>
        /// <returns>list of threads status</returns>
        public static List<ThreadStatus> GetAllThreadStatus(string partitionKey)
        {
            CloudTable table = GetCloudTable(ThreadStatusTableReference);
            TableQuery<ThreadStatus> query = new TableQuery<ThreadStatus>().Where(TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey));
                      
            return table.ExecuteQuery(query).ToList();
        }

        /// <summary>
        /// Gets a message from a queue containing report logs
        /// </summary>
        /// <param name="queueReference">target queue</param>
        /// <returns>string containing a report log</returns>
        public static string GetMessageFromLog(string queueReference)
        {
            CloudQueue urlQueue = GetCloudQueue(queueReference);
            urlQueue.FetchAttributes();
            if (urlQueue.ApproximateMessageCount > 0)
            {
                try
                {
                    CloudQueueMessage message = urlQueue.GetMessage(TimeSpan.FromMinutes(5));
                    string msg = message.AsString;
                    string dateMsg = message.InsertionTime.ToString();
                    urlQueue.DeleteMessage(message);

                    return "[" + dateMsg + "] || " + msg;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the switches for all thread switches across webcrawlers
        /// </summary>
        /// <param name="willStart">on or off</param>
        public static void SetAllSwitches(bool willStart)
        {            
            CloudTable table = GetCloudTable(ThreadSwitcherTableReference);
            TableQuery<ThreadSwitch> query = new TableQuery<ThreadSwitch>();

            foreach (ThreadSwitch switcher in table.ExecuteQuery(query)){
                switcher.IsSwitched = willStart;
                TableOperation updateOperation = TableOperation.Replace(switcher);                    
                table.Execute(updateOperation);
            }                        
        }

        /// <summary>
        /// updates the status of a thread after switching it  
        /// </summary>
        /// <param name="temporaryStatus">starting... or stopping...</param>
        public static void ChangeStatusAfterSwitching(string temporaryStatus)
        {
            CloudTable table = GetCloudTable(ThreadStatusTableReference);
            TableQuery<ThreadStatus> query = new TableQuery<ThreadStatus>();

            foreach (ThreadStatus status in table.ExecuteQuery(query))
            {
                try
                {
                    status.Status = temporaryStatus;
                    TableOperation updateOperation = TableOperation.Replace(status);
                    table.Execute(updateOperation);
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// gets all currently crawled urls by all threads to be displayed in the dashboard
        /// </summary>
        /// <param name="partitionKey">Which web crawler</param>
        /// <returns></returns>
        public static List<CurrentURLEntity> GetAllCurrentlyCrawledURL  (string partitionKey)
        {
            CloudTable table = GetCloudTable("currentlycrawledurlstable");
            TableQuery<CurrentURLEntity> query = new TableQuery<CurrentURLEntity>().Where(TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey));

            return table.ExecuteQuery(query).ToList();
        }

        /// <summary>
        /// gets all the states of switches
        /// </summary>
        /// <param name="partitionKey">which webcrawler</param>
        /// <returns>boolean depicting on/off states</returns>
        public static List<ThreadSwitch> GetAllSwitchStates(string partitionKey)
        {
            CloudTable table = GetCloudTable(ThreadSwitcherTableReference);
            TableQuery<ThreadSwitch> query = new TableQuery<ThreadSwitch>().Where(TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey));

            return table.ExecuteQuery(query).ToList();
        }

        /// <summary>
        /// switches a specific thread for a particular web crawler
        /// </summary>
        /// <param name="willStart">on/off</param>
        /// <param name="partitionKey">which webcrawler</param>
        /// <param name="rowKey">which thread</param>
        public static void SetSpecificSwitch(bool willStart, string partitionKey, string rowKey)
        {
            CloudTable table = GetCloudTable(ThreadSwitcherTableReference);
            TableQuery<ThreadSwitch> query = new TableQuery<ThreadSwitch>().Where(TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(RowKey, QueryComparisons.Equal, rowKey)));

            foreach (ThreadSwitch switcher in table.ExecuteQuery(query))
            {
                switcher.IsSwitched = willStart;
                TableOperation updateOperation = TableOperation.Replace(switcher);
                table.Execute(updateOperation);
            }
        }

        /// <summary>
        /// retieves a list of data crawled under the same webcrawler
        /// </summary>
        /// <param name="partitionKey">which webcrawler</param>
        /// <returns>list of data</returns>
        public static List<HTMLData> GetAllElementsUnderSamePartition(string partitionKey)
        {
            CloudTable table = GetCloudTable(HTMLDataTableReference);
            TableQuery<HTMLData> query = new TableQuery<HTMLData>().Where(TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey));

            try
            {
                return table.ExecuteQuery(query).ToList();
            }

            catch
            {
                return null;
            }            
        }

        /// <summary>
        /// Gets a single html data from the table
        /// </summary>
        /// <param name="partitionKey">which web crawler</param>
        /// <param name="url">which url stored</param>
        /// <returns></returns>
        public static HTMLData GetHTMLDataElement(string partitionKey, string url)
        {
            url = System.Net.WebUtility.UrlEncode(url);
            CloudTable table = GetCloudTable(HTMLDataTableReference);

            TableOperation retrieveOpertaion = TableOperation.Retrieve<HTMLData>(partitionKey, url);
            TableResult retrievedResult = table.Execute(retrieveOpertaion);
            HTMLData updateEntity = (HTMLData)retrievedResult.Result;

            return updateEntity;            
        }

        /// <summary>
        /// gets the body content inside the blob file
        /// </summary>
        /// <param name="blockBlobReference">what is the name of the file</param>
        /// <returns>string content of the body of the html</returns>
        public static string GetContentFromBlobFile(string blockBlobReference)
        {
            CloudBlockBlob blockBlob = GetCloudBlockBlob(blockBlobReference);

            using (var stream = blockBlob.OpenRead())
            {                
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }   
        }

        /// <summary>
        /// deletes all messages from the queue - required for restart
        /// </summary>
        /// <param name="queueReference"></param>
        public static void DeleteAllMessagesFromQueue(string queueReference)
        {
            CloudQueue urlQueue = GetCloudQueue(queueReference);
            urlQueue.Clear();
        }

        /// <summary>
        /// activates all reset flags for web crawlers
        /// </summary>
        public static void SwitchAllResetFlags()
        {
            CloudTable table = GetCloudTable(MachineCountersTableReference);
            TableQuery<MachineCounterEntity> query = new TableQuery<MachineCounterEntity>().Where(
                TableQuery.GenerateFilterCondition(RowKey, QueryComparisons.Equal, RestartFlagKey));

            foreach (MachineCounterEntity restartFlag in table.ExecuteQuery(query))
            {
                restartFlag.Value = 1;
                TableOperation updateOperation = TableOperation.Replace(restartFlag);
                table.Execute(updateOperation);
            }
        }

        /// <summary>
        /// deletes blob container - required for restart
        /// </summary>
        /// <param name="containerReferenceName"></param>
        public static void DeleteBlobContainer(string containerReferenceName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerReferenceName);
            
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

        /// <summary>
        /// recreates same blob container after deleting
        /// </summary>
        /// <param name="containerReferenceName"></param>
        public static void RecreateBlobContainer (string containerReferenceName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerReferenceName);

            CreateBlob(container);
        }

        /// <summary>
        /// waits until it is available to recreate the blob container
        /// </summary>
        /// <param name="container"></param>
        public static void CreateBlob(CloudBlobContainer container)
        {             
            try
            {
                container.CreateIfNotExists();
            }
            catch
            {
                Thread.Sleep(10000);
                CreateBlob(container);
            }
        }

        /// <summary>
        /// deletes the table - required for restart
        /// </summary>
        /// <param name="tableReferenceName"></param>
        public static void DeleteTable(string tableReferenceName)
        {
            CloudTable table = GetCloudTable(tableReferenceName);            
            table.DeleteIfExists();
        }

        /// <summary>
        /// recreates same table after deleting
        /// </summary>
        /// <param name="tableReferenceName"></param>
        public static void RecreateTable(string tableReferenceName)
        {
            CloudTable table = GetCloudTable(tableReferenceName);
            CreateTable(table);
        }

        /// <summary>
        /// waits until it is available to recreate the table
        /// </summary>
        /// <param name="table"></param>
        public static void CreateTable(CloudTable table)
        {
            try
            {
                table.CreateIfNotExists();
            }
            catch
            {
                Thread.Sleep(10000);
                CreateTable(table);
            }
        }

        /// <summary>
        /// resets all the values of every data containing the numbers of stored urls
        /// </summary>
        public static void ResetAllNumberOfUrls()
        {
            CloudTable table = GetCloudTable(NumberUrlsCrawledTableReference);
            TableQuery<UrlCount> query = new TableQuery<UrlCount>();

            foreach (UrlCount count in table.ExecuteQuery(query))
            {
                count.Urls = 0;
                TableOperation updateOperation = TableOperation.Replace(count);
                table.Execute(updateOperation);
            }
        }
    }
}