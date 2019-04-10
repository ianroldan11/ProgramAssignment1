using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebCrawler
{
    public enum UrlType
    {
        HTML,
        XML,
        Invalid
    }

    public class WebCrawlerManager
    {
        // Flag to indicate breaking of all for-loops when STOP command is initiated
        public static bool StopIterations = false;
        // robots url to be usedto specific web crawler
        public string RobotsUrl { get; }
        // Queue Reference containing Urls
        public string UrlQueueReference { get; }
        // Queue Reference containing Logs
        public string LogQueueReference { get; }
        // Base Uri to be appended to urls retrieved that have omitted domain/host name
        public string BaseUri { get; }
        // Required domain name for urls to determine if they ar part of the crawled domain
        public string RequiredDomain { get; }
        // Robots crawler specific for a single website
        public RobotsAnalyzer RobotsCrawler { get; set; }

        public WebCrawlerManager(string robotsUrl, string urlQueueReference, string logQueueReference, string baseUri, string requiredDomain)
        {
            RobotsUrl = robotsUrl;
            UrlQueueReference = urlQueueReference;
            LogQueueReference = logQueueReference;
            RequiredDomain = requiredDomain;
            BaseUri = baseUri;
        }

        /// <summary>
        /// Checks if url grabbed from queue is xml, html or uncrawlable url
        /// </summary>
        /// <param name="urlToCheck"></param>
        /// <returns></returns>
        public static UrlType UrlTypeChecker(string urlToCheck)
        {
            Uri checkedUri = new Uri(urlToCheck);

            if (checkedUri.AbsolutePath.EndsWith(".xml"))
            {
                return UrlType.XML;
            }

            else if (checkedUri.AbsolutePath.EndsWith(".json") || checkedUri.AbsolutePath.EndsWith(".rtf") || checkedUri.AbsolutePath.EndsWith(".js") || checkedUri.AbsolutePath.EndsWith(".jpg"))
            {
                return UrlType.Invalid;
            }

            else
            {
                return UrlType.HTML;
            }
        }

        /// <summary>
        /// Uses Robots Crawler Object to check if url being crawled is disallowed
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool UrlIsDisallowed(string url)
        {
            Uri checkedUri = new Uri(url);

            if (RobotsCrawler != null)
            {
                foreach (string directory in RobotsCrawler.DisallowedDirectories)
                {
                    if (checkedUri.AbsolutePath.Contains(directory))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets all valid urls based on its date if it is within 3 months ago until now
        /// </summary>
        /// <param name="listOfUrlElements"></param>
        /// <param name="listOfDateElements"></param>
        /// <returns></returns>
        public IEnumerable<string> GetUrlsFromListOfElements(IEnumerable<XElement> listOfUrlElements, IEnumerable<XElement> listOfDateElements)
        {            
            int index = 0;
            foreach (DateTime dateTime in listOfDateElements.Select(x => ConvertDateStringToDate(x.Value)))
            {
                if (StopIterations)
                {
                    break;
                }

                index++;
                if (DateTime.Compare(dateTime, DateTime.Today.AddMonths(-3)) >= 0)
                {                    
                    yield return listOfUrlElements.Select(x => x.Value).ToArray()[index - 1];
                }               
            }
            
        }

        /// <summary>
        /// Conversts string to DateTime object
        /// </summary>
        /// <param name="dateTimeString"></param>
        /// <returns></returns>
        public DateTime ConvertDateStringToDate(string dateTimeString)
        {
            // get only date, trim time
            dateTimeString = dateTimeString.Substring(0, 10);
            
            DateTime dt = DateTime.ParseExact(dateTimeString, "yyyy-MM-dd", null);
            return dt;    
        }

        /// <summary>
        /// Stores pertinent data to table
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="date">nullable publish date of url</param>
        /// <param name="blobFile">name reference for the blob file containing this url's body content</param>
        public void StoreDataToTable(string title, string url, DateTime? date, string blobFile)
        {
            string[] keyWords = title.Split(' ');
            foreach (string keyWord in keyWords)
            {
                Regex rgx = new Regex("[^a-zA-Z0-9]");
                string newKeyWord = rgx.Replace(keyWord, "");
                WorkerRole.ASMClient.AddHTMLDataEntityToTable(newKeyWord.ToLower(), url, date, blobFile, title);
            }
        }

        /// <summary>
        /// Checks if url is part of the domain being crawled
        /// </summary>
        /// <param name="url">url to be checked</param>
        /// <returns></returns>
        public bool UrlIsIncludedInDomain(string url)
        {
            Uri uri = new Uri(url);
            if (uri.Host.Contains(RequiredDomain))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Create file containing url body content in blob container
        /// </summary>
        /// <param name="bodyContents">list of paragraphs from body html tag</param>
        /// <param name="blobFileName">name for the file to be created</param>
        public void CreateFileInBlob(IEnumerable<string> bodyContents, string blobFileName)
        {
            string wholeContent = string.Join("\n", bodyContents);
            WorkerRole.ASMClient.CreateBlobFile(blobFileName, wholeContent);
        }
    }    
}
