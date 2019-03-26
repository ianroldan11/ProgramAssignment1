using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole3
{
    class AzureStorageManager
    {
        private static readonly string UrlMainQueueReference = "imdburlqueue";
        private static readonly string LogQueueReference = "logqueue";
        private static readonly string OwnLogQueueReference = "imdblogqueue";
        private static readonly string HTMLDataMainTableReference = "htmldatatable";
        private static readonly string MachineCounterTableReference = "machinecounterstable";
        private static readonly string ThreadSwitcherTableReference = "threadswitchertable";
        private static readonly string ThreadStatusTableReference = "threadstatustable";
        private static readonly string CurrentlyCrawledURLTableReference = "currentlycrawledurlstable";
        private static readonly string NumberOfUrlsCrawledTableReference = "numberurlscrawledtable";
        private static readonly string HTMLBodyTextBlobContainerReference = "htmlbodyblobcontainer";

        // Cloud Storage Instantiation------------------------------------------------------------------------------------------------------------------
        private static CloudQueue GetCloudQueue(string queueReference)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient QueueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue Queue = QueueClient.GetQueueReference(queueReference);

            return Queue;
        }

        private static CloudTable GetCloudTable(string tableReference)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient TableClient = storageAccount.CreateCloudTableClient();
            CloudTable Table = TableClient.GetTableReference(tableReference);

            return Table;
        }

        private static CloudBlockBlob GetCloudBlockBlob(string blobContainerReference, string blockBlobReference)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = BlobClient.GetContainerReference(blobContainerReference);
            CloudBlockBlob BlockBlob = container.GetBlockBlobReference(blockBlobReference);

            return BlockBlob;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------


        // Queue Methods------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------
        // Adding Messages--------        
        public static string AddUrlToQueue(string url)
        {
            try
            {                
                CloudQueue urlQueue = GetCloudQueue(UrlMainQueueReference);
                CloudQueueMessage message = new CloudQueueMessage(url);
                urlQueue.AddMessage(message);

                string stringLog = "URL: " + url + " has been successfully added to Queue Reference: " + UrlMainQueueReference;
                return stringLog;
            }

            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static void AddLogMessage(string stringLog)
        {
            CloudQueue logQueue = GetCloudQueue(LogQueueReference);
            CloudQueueMessage logMessage = new CloudQueueMessage(stringLog);
            logQueue.AddMessage(logMessage);
        }

        public static void AddIndividualLogMessage(string stringLog)
        {
            CloudQueue logQueue = GetCloudQueue(OwnLogQueueReference);
            CloudQueueMessage logMessage = new CloudQueueMessage(stringLog);
            logQueue.AddMessage(logMessage);
        }
        //--------------------------

        //Getting and Deleting Messages----------------
        private static string GetMessageFromQueue(string queueReference)
        {            

            CloudQueue urlQueue = GetCloudQueue(queueReference);

            urlQueue.FetchAttributes();

            if (urlQueue.ApproximateMessageCount > 0)
            {
                try
                {
                    CloudQueueMessage message = urlQueue.GetMessage(TimeSpan.FromMinutes(5));
                    string msg = message.AsString;
                    urlQueue.DeleteMessage(message);

                    return msg;
                }

                catch
                {
                    return null;
                }  
            }            

            return null;
        }        

        public static string GetUrlMessageFromQueue()
        {
            string returnMessage = GetMessageFromQueue(UrlMainQueueReference);
            if (returnMessage != null)
            {
                return returnMessage;
            }
            else
            {
                return null;
            }
        }
        //-----------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------

        // Table Methods------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------
        // Adding entities--------------------
        public static string AddHTMLDataEntityToTable(HTMLData dataObject)
        {
            try
            {
                CloudTable table = GetCloudTable(HTMLDataMainTableReference);
                TableOperation insertOperation = TableOperation.InsertOrReplace(dataObject);
                table.Execute(insertOperation);
                string urlString = System.Net.WebUtility.UrlDecode(dataObject.RowKey);
                // Increment number of data in table
                IncrementNumberOfDataInTable();
                return "URL: " + urlString + "has been successfully added in Table Reference: " + HTMLDataMainTableReference;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
        //-----------------------

        // Retrieving entities--------------------
        public static HTMLData RetrieveHTMLDataEntityFromTable(string rowKey, string partitionKey)
        {
            rowKey = System.Net.WebUtility.UrlEncode(rowKey);
            CloudTable table = GetCloudTable(HTMLDataMainTableReference);
            TableOperation retrieveOpertaion = TableOperation.Retrieve<HTMLData>(partitionKey, rowKey);
            TableResult retrievedResult = table.Execute(retrieveOpertaion);
            HTMLData updateEntity = (HTMLData)retrievedResult.Result;

            return updateEntity;
        }

        public static ThreadSwitch RetrieveThreadSwitchStatus(string rowKey, string partitionKey)
        {
            CloudTable table = GetCloudTable(ThreadSwitcherTableReference);
            TableOperation retrieveOpertaion = TableOperation.Retrieve<ThreadSwitch>(partitionKey, rowKey);
            TableResult retrievedResult = table.Execute(retrieveOpertaion);
            ThreadSwitch updateEntity = (ThreadSwitch)retrievedResult.Result;

            return updateEntity;
        }

        public static MachineCounterEntity RetrieveRestartFlag(string partitionKey)
        {
            CloudTable table = GetCloudTable(MachineCounterTableReference);
            TableOperation retrieveOpertaion = TableOperation.Retrieve<MachineCounterEntity>(partitionKey, "RestartFlag");
            TableResult retrievedResult = table.Execute(retrieveOpertaion);
            MachineCounterEntity updateEntity = (MachineCounterEntity)retrievedResult.Result;

            return updateEntity;
        }
        //-----------------------

        // Updating entities--------------------
        public static string UpdateMachineCounterValues(string partitionKey, string rowKey, double newValue)
        {
            try
            {
                CloudTable table = GetCloudTable(MachineCounterTableReference);
                TableOperation retrieveOpertaion = TableOperation.Retrieve<MachineCounterEntity>(partitionKey, rowKey);
                TableResult retrievedResult = table.Execute(retrieveOpertaion);
                MachineCounterEntity updateEntity = (MachineCounterEntity)retrievedResult.Result;
                if (updateEntity != null)
                {
                    updateEntity.Value = newValue;
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                }
                return "Memory Updated";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static string UpdateCurrentlyCrawledUrlTableEntity(string partitionKey, string rowKey, string newValue)
        {
            newValue = System.Net.WebUtility.UrlEncode(newValue);
            try
            {
                CloudTable table = GetCloudTable(CurrentlyCrawledURLTableReference);
                TableOperation retrieveOpertaion = TableOperation.Retrieve<CurrentURLEntity>(partitionKey, rowKey);
                TableResult retrievedResult = table.Execute(retrieveOpertaion);
                CurrentURLEntity updateEntity = (CurrentURLEntity)retrievedResult.Result;

                if (updateEntity != null)
                {
                    updateEntity.URL = newValue;
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                }

                return "URL Set to " + newValue;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static string UpdateThreadStatus(string partitionKey, string rowKey, string newValue)
        {
            try
            {
                CloudTable table = GetCloudTable(ThreadStatusTableReference);
                TableOperation retrieveOpertaion = TableOperation.Retrieve<ThreadStatus>(partitionKey, rowKey);
                TableResult retrievedResult = table.Execute(retrieveOpertaion);
                ThreadStatus updateEntity = (ThreadStatus)retrievedResult.Result;

                if (updateEntity != null)
                {
                    updateEntity.Status = newValue;
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                }

                return "Thread is " + newValue;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static string IncrementNumberOfUrls(string rowKey)
        {
            try
            {
                CloudTable table = GetCloudTable(NumberOfUrlsCrawledTableReference);
                TableOperation retrieveOpertaion = TableOperation.Retrieve<UrlCount>(NumberOfUrlsCrawledTableReference, rowKey);
                TableResult retrievedResult = table.Execute(retrieveOpertaion);
                UrlCount updateEntity = (UrlCount)retrievedResult.Result;

                if (updateEntity != null)
                {
                    updateEntity.Urls += 1;
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                }

                retrieveOpertaion = TableOperation.Retrieve<UrlCount>(NumberOfUrlsCrawledTableReference, "All");
                retrievedResult = table.Execute(retrieveOpertaion);
                updateEntity = (UrlCount)retrievedResult.Result;

                if (updateEntity != null)
                {
                    updateEntity.Urls += 1;
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                }

                return rowKey + " Urls Crawled has increased.";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static string UpdateRestartFlag(string partitionKey)
        {
            try
            {
                CloudTable table = GetCloudTable(MachineCounterTableReference);
                TableOperation retrieveOpertaion = TableOperation.Retrieve<MachineCounterEntity>(partitionKey, "RestartFlag");
                TableResult retrievedResult = table.Execute(retrieveOpertaion);
                MachineCounterEntity updateEntity = (MachineCounterEntity)retrievedResult.Result;

                if (updateEntity != null)
                {
                    updateEntity.Value = 0;
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                }

                return "Updated";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static string IncrementNumberOfBlobFiles()
        {
            try
            {
                CloudTable table = GetCloudTable(NumberOfUrlsCrawledTableReference);
                TableOperation retrieveOpertaion = TableOperation.Retrieve<UrlCount>("numberofblobfiles", "All");
                TableResult retrievedResult = table.Execute(retrieveOpertaion);
                UrlCount updateEntity = (UrlCount)retrievedResult.Result;

                if (updateEntity != null)
                {
                    updateEntity.Urls += 1;
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                }

                return "Number of blob files has increased.";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static string IncrementNumberOfDataInTable()
        {
            try
            {
                CloudTable table = GetCloudTable(NumberOfUrlsCrawledTableReference);
                TableOperation retrieveOpertaion = TableOperation.Retrieve<UrlCount>("numberofdataintable", "All");
                TableResult retrievedResult = table.Execute(retrieveOpertaion);
                UrlCount updateEntity = (UrlCount)retrievedResult.Result;

                if (updateEntity != null)
                {
                    updateEntity.Urls += 1;
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                }

                return "Number of data in table has increased.";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
        //-----------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------

        // Blob Methods------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------
        public static string SaveTextBodyToBlockBlob(string textBody, string blockBlobName)
        {
            CloudBlockBlob blockBlob = GetCloudBlockBlob(HTMLBodyTextBlobContainerReference, blockBlobName);

            try
            {
                using (StreamWriter writer = new StreamWriter(blockBlob.OpenWrite()))
                {
                    writer.WriteLine(textBody);                    
                }

                // increments number of blob files
                IncrementNumberOfBlobFiles();
                return "Writing body text for " + blockBlobName + " to " + HTMLBodyTextBlobContainerReference + " is successful";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            
        }
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
    }
}
