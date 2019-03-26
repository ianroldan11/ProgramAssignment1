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

namespace WorkerRole2
{
    class AzureStorageManager
    {
        private static readonly string UrlMainQueueReference = "bleachreporturlqueue";
        private static readonly string LogQueueReference = "logqueue";
        private static readonly string OwnLogQueueReference = "bleacherreportlogqueue";
        private static readonly string HTMLDataMainTableReference = "htmldatatable";
        private static readonly string MachineCounterTableReference = "machinecounterstable";
        private static readonly string ThreadSwitcherTableReference = "threadswitchertable";
        private static readonly string ThreadStatusTableReference = "threadstatustable";
        private static readonly string CurrentlyCrawledURLTableReference = "currentlycrawledurlstable";
        private static readonly string NumberOfUrlsCrawledTableReference = "numberurlscrawledtable";
        private static readonly string HTMLBodyTextBlobContainerReference = "htmlbodyblobcontainer";

        private static readonly string RestartFlag = "";
        private static readonly string TargetAll = "All";
        private static readonly string NumberOfBlobFilesKey = "numberofblobfiles";
        private static readonly string NumberOfDataInTableKey = "numberofdataintable";

        // Cloud Storage Instantiation------------------------------------------------------------------------------------------------------------------
        // Standard code block for getting a queue from azure storage
        private static CloudQueue GetCloudQueue(string queueReference)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient QueueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue Queue = QueueClient.GetQueueReference(queueReference);

            return Queue;
        }

        // Standard code block for getting a table from azure storage
        private static CloudTable GetCloudTable(string tableReference)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient TableClient = storageAccount.CreateCloudTableClient();
            CloudTable Table = TableClient.GetTableReference(tableReference);

            return Table;
        }

        // Standard code block for getting a block blob from azure storage
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
        /// <summary>
        /// adds a url to the queue to be crawled later on
        /// </summary>
        /// <param name="url"></param>
        /// <returns>returns a string report if successful or not</returns>
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

        /// <summary>
        /// adds a string containing a report of any important event in the log queue
        /// </summary>
        /// <param name="stringLog">the string to be added in the log queue</param>
        public static void AddLogMessage(string stringLog)
        {
            CloudQueue logQueue = GetCloudQueue(LogQueueReference);
            CloudQueueMessage logMessage = new CloudQueueMessage(stringLog);
            logQueue.AddMessage(logMessage);
        }

        /// <summary>
        /// aside from a general log queue, each webcrawler has its own log queue to contain reports coming from that webcrawler only
        /// </summary>
        /// <param name="stringLog">the string to be added in the log queue</param>
        public static void AddIndividualLogMessage(string stringLog)
        {
            CloudQueue logQueue = GetCloudQueue(OwnLogQueueReference);
            CloudQueueMessage logMessage = new CloudQueueMessage(stringLog);
            logQueue.AddMessage(logMessage);
        }
        //--------------------------

        //Getting and Deleting Messages----------------
        /// <summary>
        /// Obtains a message from the specified queue
        /// </summary>
        /// <param name="queueReference">queue to obtain the message from</param>
        /// <returns>returns a string obtaining the message</returns>
        private static string GetMessageFromQueue(string queueReference)
        {            
            // get queue based on the reference specified
            CloudQueue urlQueue = GetCloudQueue(queueReference);
            // fetch all content
            urlQueue.FetchAttributes();
            // check if queue has content
            if (urlQueue.ApproximateMessageCount > 0)
            {
                try
                {
                    // gets message
                    CloudQueueMessage message = urlQueue.GetMessage(TimeSpan.FromMinutes(5));
                    // gets the value of the message to return
                    string msg = message.AsString;
                    // delete the message from the queue
                    urlQueue.DeleteMessage(message);
                    return msg;
                }
                catch
                {
                    return null;
                }  
            } 
            // returns null if queue has no content
            return null;
        }
                
        /// <summary>
        /// obtains a url message
        /// </summary>
        /// <returns>url to be crawled next</returns>
        public static string GetUrlMessageFromQueue()
        {
            // use GetMessageFromQueue method using the reference for the queue containing the urls
            string returnMessage = GetMessageFromQueue(UrlMainQueueReference);
            if (returnMessage != null)
            {
                // returns url
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
        /// <summary>
        /// adds an element containing all the pertinent data retrieved from the url (url, title, publish date, body text)
        /// </summary>
        /// <param name="dataObject">object to be saved in the table</param>
        /// <returns>report to tell if successful or not</returns>
        public static string AddHTMLDataEntityToTable(HTMLData dataObject)
        {
            try
            {
                // get table containing all HTML Data
                CloudTable table = GetCloudTable(HTMLDataMainTableReference);
                // insert operation
                TableOperation insertOperation = TableOperation.InsertOrReplace(dataObject);
                table.Execute(insertOperation);
                // decodes string for readability
                string urlString = System.Net.WebUtility.UrlDecode(dataObject.RowKey);
                // Increment number of data in table
                IncrementNumberOfDataInTable();
                return "URL: " + urlString + "has been successfully added in Table Reference: " + HTMLDataMainTableReference;
            }
            catch(Exception e)
            {
                return e.ToString();
            }
        }
        //-----------------------

        // Retrieving entities--------------------
        /// <summary>
        /// Retrieves data from the table
        /// </summary>
        /// <param name="rowKey">rowkey to specify data to retrieve</param>
        /// <param name="partitionKey">partition key to specify which partition it is located</param>
        /// <returns>returns the object retrieved</returns>
        public static HTMLData RetrieveHTMLDataEntityFromTable(string rowKey, string partitionKey)
        {
            // encodes the url used to compare matches - this is because url stored in the table is URLencoded too
            rowKey = System.Net.WebUtility.UrlEncode(rowKey);
            // get table containing data
            CloudTable table = GetCloudTable(HTMLDataMainTableReference);            
            // retrieve operation
            TableOperation retrieveOpertaion = TableOperation.Retrieve<HTMLData>(partitionKey, rowKey);
            TableResult retrievedResult = table.Execute(retrieveOpertaion);
            HTMLData updateEntity = (HTMLData)retrievedResult.Result;
            // returns whatever was retrieved - null if no match
            return updateEntity;
        }

        /// <summary>
        /// Retrieves switch status to check if crawler would start or stop
        /// </summary>
        /// <param name="rowKey">name of thread</param>
        /// <param name="partitionKey">a key identifier unique to every webcrawler</param>
        /// <returns>thread switch that handles the decision</returns>
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
        /// <summary>
        /// updates or rewrites the cpu usage and available memory over time
        /// </summary>
        /// <param name="partitionKey">a key identifier unique to every webcrawler</param>
        /// <param name="rowKey">contains either "Available Memory" or "CPU Usage"</param>
        /// <param name="newValue">value used to rewrite the old value</param>
        /// <returns>report if successful or not</returns>
        public static string UpdateMachineCounterValues(string partitionKey, string rowKey, double newValue)
        {
            try
            {
                // get table containing the cpu usage and available memory data
                CloudTable table = GetCloudTable(MachineCounterTableReference);
                TableOperation retrieveOpertaion = TableOperation.Retrieve<MachineCounterEntity>(partitionKey, rowKey);
                TableResult retrievedResult = table.Execute(retrieveOpertaion);
                MachineCounterEntity updateEntity = (MachineCounterEntity)retrievedResult.Result;
                // makes sure retrieved value is not null
                if (updateEntity != null)
                {
                    // changes value then updates entity in the table
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

        /// <summary>
        /// updates the url currently being crawled
        /// </summary>
        /// <param name="partitionKey">a key identifier unique to every webcrawler</param>
        /// <param name="rowKey">thread name</param>
        /// <param name="newValue">url currently being crawled</param>
        /// <returns>report if successful or not</returns>
        public static string UpdateCurrentlyCrawledUrlTableEntity(string partitionKey, string rowKey, string newValue)
        {
            // encodes the url that will replace the old value
            newValue = System.Net.WebUtility.UrlEncode(newValue);
            try
            {
                // gets cloud table
                CloudTable table = GetCloudTable(CurrentlyCrawledURLTableReference);
                // retrieve operation
                TableOperation retrieveOpertaion = TableOperation.Retrieve<CurrentURLEntity>(partitionKey, rowKey);
                TableResult retrievedResult = table.Execute(retrieveOpertaion);
                CurrentURLEntity updateEntity = (CurrentURLEntity)retrievedResult.Result;
                //makes sure retrieved identity is not null
                if (updateEntity != null)
                {
                    //updates value in the table
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

        /// <summary>
        /// updates thread status (crawling, idle, stopped, loading)
        /// </summary>
        /// <param name="partitionKey">a key identifier unique to every webcrawler</param>
        /// <param name="rowKey">thread name</param>
        /// <param name="newValue">status</param>
        /// <returns>report if successful or not</returns>
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

        /// <summary>
        /// Saves the new value of the number of urls crawled after a thread crawls a url
        /// </summary>
        /// <param name="rowKey">Which Web Crawler crawled the url</param>
        /// <returns>report if successful</returns>
        public static string IncrementNumberOfUrls(string rowKey)
        {
            try
            {
                // get table
                CloudTable table = GetCloudTable(NumberOfUrlsCrawledTableReference);
                // get the target entity
                TableOperation retrieveOpertaion = TableOperation.Retrieve<UrlCount>(NumberOfUrlsCrawledTableReference, rowKey);
                TableResult retrievedResult = table.Execute(retrieveOpertaion);
                UrlCount updateEntity = (UrlCount)retrievedResult.Result;
                // increment its value
                if (updateEntity != null)
                {
                    updateEntity.Urls += 1;
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                }
                // get entity containing the total number of urls crawled throughout the web crawlers
                retrieveOpertaion = TableOperation.Retrieve<UrlCount>(NumberOfUrlsCrawledTableReference, TargetAll);
                retrievedResult = table.Execute(retrieveOpertaion);
                updateEntity = (UrlCount)retrievedResult.Result;
                // increment its value
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

        /// <summary>
        /// Method called set restart flag in table to 0
        /// </summary>
        /// <param name="partitionKey">which web crawler is restarting</param>
        /// <returns>report if successful</returns>
        public static string UpdateRestartFlag(string partitionKey)
        {
            try
            {
                CloudTable table = GetCloudTable(MachineCounterTableReference);
                TableOperation retrieveOpertaion = TableOperation.Retrieve<MachineCounterEntity>(partitionKey, RestartFlag);
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

        /// <summary>
        /// Saves the new value of the number of blob files stored after creating a new blob file
        /// </summary>
        /// <returns>report if successful</returns>
        public static string IncrementNumberOfBlobFiles()
        {
            try
            {
                CloudTable table = GetCloudTable(NumberOfUrlsCrawledTableReference);
                TableOperation retrieveOpertaion = TableOperation.Retrieve<UrlCount>(NumberOfBlobFilesKey, TargetAll);
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

        /// <summary>
        /// Saves the new value of number of data in the table after storing data from a crawled url
        /// </summary>
        /// <returns>report if successful</returns>
        public static string IncrementNumberOfDataInTable()
        {
            try
            {
                CloudTable table = GetCloudTable(NumberOfUrlsCrawledTableReference);
                TableOperation retrieveOpertaion = TableOperation.Retrieve<UrlCount>(NumberOfDataInTableKey, TargetAll);
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
        /// <summary>
        /// since the text body is large enough not to fit the table, this will be written and saved in a blob file instead
        /// </summary>
        /// <param name="textBody"></param>
        /// <param name="blockBlobName"></param>
        /// <returns>report if successful or not</returns>
        public static string SaveTextBodyToBlockBlob(string textBody, string blockBlobName)
        {
            // gets block blob file - creates new if nonexistent
            CloudBlockBlob blockBlob = GetCloudBlockBlob(HTMLBodyTextBlobContainerReference, blockBlobName);

            try
            {
                //creates a streamwriter using the text body and writes it to the block blob file
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
