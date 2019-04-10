using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WebCrawler
{
    class HTMLReader
    {
        public Uri Uri { get; }
        public string Title { get; }
        public DateTime? PublishDate { get; }
        
        /// <summary>
        /// Gets both title and publish date from url
        /// </summary>
        /// <param name="htmlUrl">url to be crawled</param>
        public HTMLReader(string htmlUrl)
        {
            Uri = new Uri(htmlUrl);
            this.Title = GetTitleFromUrlContent(GetUrllContent());
            PublishDate = GetDateFromUrlContent();
            if (PublishDate == null)
            {
                PublishDate = GetDateFromUrlString();
            }
        }

        /// <summary>
        /// Gets html content from url
        /// </summary>
        /// <returns>string of html content</returns>
        private string GetUrllContent()
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(Uri);
            }
        }

        /// <summary>
        /// gets title of the url
        /// </summary>
        /// <param name="urlContent"></param>
        /// <returns>title</returns>
        private static string GetTitleFromUrlContent(string urlContent)
        {
            string title = Regex.Match(urlContent, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
            return title;
        }

        /// <summary>
        /// gets all url from href attributes
        /// </summary>
        /// <param name="domainSite">used when urls in href omits domain/host name</param>
        /// <returns>enumerable list of urls</returns>
        public IEnumerable<string> GetUrls(string domainSite)
        {
            var web = new HtmlWeb();
            var doc = web.Load(Uri);
            var hrefs = doc.DocumentNode.SelectNodes("//a[@href]").Select(x => x.Attributes["href"].Value);

            foreach (string href in hrefs)
            {
                if (WebCrawlerManager.StopIterations)
                {
                    break;
                }

                string hrefToReturn;
                bool hrefIsValidUrl;    

                try
                {
                    Uri tempUri = new Uri(href);
                    hrefIsValidUrl = tempUri.IsAbsoluteUri;
                }
                catch
                {
                    hrefIsValidUrl = false;
                }

                if (!hrefIsValidUrl)
                {
                    hrefToReturn = domainSite + href;
                }
                else
                {
                    hrefToReturn = href;
                }                

                yield return hrefToReturn;
            }            
        }

        /// <summary>
        /// gets datetime value from pubdate metadata of url
        /// </summary>
        /// <returns></returns>
        private DateTime? GetDateFromUrlContent()
        {
            var web = new HtmlWeb();
            var doc = web.Load(Uri);
            try
            {
                var dateString = doc.DocumentNode.SelectSingleNode("//meta[@name='pubdate']").Attributes["content"].Value;

                if (dateString != null)
                {
                    return GetDateTimeFromDateString(dateString);
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

        /// <summary>
        /// converts string to date
        /// </summary>
        /// <param name="dateString">string of date</param>
        /// <returns>datetime object</returns>
        private static DateTime GetDateTimeFromDateString(string dateString)
        {            
            DateTime dt = DateTime.ParseExact(dateString, "yyyy-MM-ddTHH:mm:ssZ", null);
            return dt;          
        }

        /// <summary>
        /// Gets date from the url itself i.e. https://www.si.com/soccer/2019/03/20/vinicius-junior-reveals-why-he-choose-real-madrid-despite-barcelona-offer-pay-more
        /// returns the 2019/03/20 as datetime
        /// </summary>
        /// <returns></returns>
        private DateTime? GetDateFromUrlString()
        {
            try
            {
                string[] UriComponent = Uri.AbsolutePath.Split('/');
                string dateString = UriComponent[2] + "/" + UriComponent[3] + "/" + UriComponent[4];
                DateTime dt = DateTime.ParseExact(dateString, "yyyy/MM/dd", null);

                return dt;
            }
            catch
            {
                return null;
            }                        
        }

        /// <summary>
        /// Gets all <p> tagged elements in body and return it as a list
        /// </summary>
        /// <returns>list of paragraphs in string</returns>
        public IEnumerable<string> GetBodyContent()
        {
            var web = new HtmlWeb();
            var doc = web.Load(Uri);
            var bodyContents = doc.DocumentNode.SelectNodes("//p").Select(x => RemoveTags(x.InnerHtml));
            var bodyContentsAppend = doc.DocumentNode.SelectNodes("//div").Where(x => x.HasClass("zn-body__paragraph")).Select(x => RemoveTags(x.InnerHtml));


            return bodyContents.Concat(bodyContentsAppend);
        }

        /// <summary>
        /// removes html tags from body content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string RemoveTags(string content)
        {
            Regex htmlTagRegex = new Regex("\\<[^\\>]*\\>");
            content = htmlTagRegex.Replace(content, String.Empty);

            return content;
        }
    }
}
