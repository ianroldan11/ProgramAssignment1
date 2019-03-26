using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole2
{
    /// <summary>
    /// class for table object: HTML Data containing all pertinent data
    /// </summary>
    class HTMLData : TableEntity
    {
        // title of webpage
        public string Title { get; set; }
        // publish date of webpage
        public DateTime? PublishDate { get; set; }
        // blob block reference containg the text body of the webpage
        public string BodyContent { get; set; }

        public HTMLData(string htmlTitle, DateTime? htmlPubDate, string bodyContent, string parKey, string url)
        {
            this.Title = htmlTitle;
            this.PublishDate = htmlPubDate;
            this.BodyContent = bodyContent;

            this.PartitionKey = parKey;            
            this.RowKey = System.Net.WebUtility.UrlEncode(url);
        }

        public HTMLData()
        {

        }        
    }
}
