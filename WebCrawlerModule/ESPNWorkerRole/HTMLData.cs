using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESPNWorkerRole
{
    class HTMLData : TableEntity
    {
        public string Title { get; set; }
        public DateTime? PublishDate { get; set; }
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
