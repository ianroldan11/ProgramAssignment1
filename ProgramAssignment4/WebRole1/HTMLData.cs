using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class HTMLData : TableEntity
    {
        public DateTime? PublishDate { get; set; }
        public string BlobFileForBody { get; set; }
        public int PopularityCount { get; set; }
        public string Title { get; set; }

        public HTMLData()
        {

        }

        public HTMLData(string title, string url, DateTime? dateTime, string blobFile, string fullTitle)
        {
            PartitionKey = title;
            RowKey = System.Net.WebUtility.UrlEncode(url);
            PublishDate = dateTime;
            BlobFileForBody = blobFile;
            Title = fullTitle;
            PopularityCount = 1;
        }
    }
}