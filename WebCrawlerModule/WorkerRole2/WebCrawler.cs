using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WorkerRole2
{
    class WebCrawler
    {
        // starting point for all web crawlers
        public static readonly string seedUrl = "https://bleacherreport.com/robots.txt";
        // identifying string to check if url belongs to the domain to be crawled to
        public static readonly string rootDomainIdentifier = "bleacherreport.com";
        // array containing all disallowed directories
        public static string[] disallowedDirectories;

        /// <summary>
        /// analyzes robots.txt to save disallowed directoried and get all urls/sitemaps
        /// </summary>
        /// <returns>array of urls/sitemaps</returns>
        public static string[] StartCollectingFromSeed()
        {
            string urlContent = GetContentsFromUrl(seedUrl);
            // sets disallowed directories contents
            disallowedDirectories = ScanDisallowedDirectoriesFromSeed(urlContent);
            // gets all urls/sitemaps
            return GetAllUrlsFromString(urlContent);
        }

        /// <summary>
        /// gets all urls from a url webpage then removes all disallowed ones
        /// </summary>
        /// <param name="url">url to be crawled</param>
        /// <returns>array of valid urls</returns>
        public static string[] GetAllValidUrls(string url)
        {
            //gets the web content from a url
            string urlContent = GetContentsFromUrl(url);
            // gets all the urls from the web content
            string[] retrievedUrls = GetAllUrlsFromString(urlContent);
            // list that will contain every url that passes validation
            List<string> validUrls = new List<string>();
            // segregates allowed urls from disallowed urls
            foreach (string retrievedUrl in retrievedUrls)
            {
                if (UrlIsAllowed(retrievedUrl))
                {
                    validUrls.Add(retrievedUrl);
                }
                validUrls.Add(retrievedUrl);
            }
            // returns an array verson of the list of valid urls
            return validUrls.ToArray();
        }

        /// <summary>
        /// filters urls based on the give condtion
        /// </summary>
        /// <param name="unfliteredUrls">array of urls</param>
        /// <returns>filtered urls</returns>
        public static string[] FilterResults(string[] unfliteredUrls)
        {
            // list that will contain all filtered urls
            List<string> filteredList = new List<string>();
            // contains keywords that are related to nba
            string[] nbaKeywords = { "nets", "celtics", "knicks", "76ers", "raptors", "bulls", "pistons", "pacers", "bucks", "cavaliers", "hornets", "grizzlies", "heat", "magic", "wizards",
                "hawks", "rockets", "spurs", "pelicans", "mavericks", "timberwolves", "blazers", "thunder", "jazz", "nuggets", "warriors", "clippers", "lakers", "suns", "kings" };

            foreach (string url in unfliteredUrls)
            {
                // check if url contains the word "nba"
                if (url.Contains("nba"))
                {
                    filteredList.Add(url);
                }

                else
                {
                    // check if url contains at least one of the valid keywords
                    foreach (string keyword in nbaKeywords)
                    {
                        if (url.Contains(keyword))
                        {
                            filteredList.Add(url);
                            break;
                        }
                    }
                }
            }
            // returns an array verson of the list of filtered urls
            return filteredList.ToArray();
        }

        /// <summary>
        /// Generates an HTML Data object out of a given url
        /// </summary>
        /// <param name="url">url to analyze</param>
        /// <param name="blockBlobReferenceNumber">unique number to differetiate between blob block files</param>
        /// <returns>generated HTML Data</returns>
        public static HTMLData CreateDataFromUrl(string url, string blockBlobReferenceNumber)
        {
            string urlContent = GetContentsFromUrl(url);
            HTMLData data = new HTMLData(GetTitleFromUrlContent(urlContent), GetDateTimeFromUrlContent(urlContent), rootDomainIdentifier + blockBlobReferenceNumber, rootDomainIdentifier, url);

            return data;
        }        

        /// <summary>
        /// used to check if url is an xml file by checking if it contains .xml as file type
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsXML(string url)
        {
            if (url.Contains(".xml"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// checks if a url is disallowed by the robots.txt
        /// </summary>
        /// <param name="url">url to be checked</param>
        /// <returns>boolean telling if it should be filtered or not</returns>
        private static bool UrlIsAllowed (string url)
        {
            foreach (string disallowedDirectory in disallowedDirectories)
            {
                if (url.Contains(disallowedDirectory))
                {
                    return false;
                }
            }
            return true;
        }        

        /// <summary>
        /// Used to gather the list of disallowed directories and store to be used later on as a filter for urls to be crawled
        /// </summary>
        /// <param name="urlContent">robots.txt url content</param>
        /// <returns>array of string containing all the directories that are not allowed to be crawled</returns>
        private static string[] ScanDisallowedDirectoriesFromSeed(string urlContent)
        {
            // replaces every 'Disallow: ' string to '!' for easier splitting
            urlContent = urlContent.Replace("Disallow: ", "!");
            // splits by '!'
            string[] scannedStrings = urlContent.Split('!');
            List<string> disallowedDirectoriesList = new List<string>();
            // gets all element that were divided
            for (int i = 1; i < scannedStrings.Length; i++)
            {
                disallowedDirectoriesList.Add(scannedStrings[i].Trim());
            }                             

            return disallowedDirectoriesList.ToArray();
        }

        /// <summary>
        /// gets the html content of the url
        /// </summary>
        /// <param name="url">target url</param>
        /// <returns>string that contains the html content</returns>
        private static string GetContentsFromUrl(string url)
        {
            string urlContent = "";           
            using (WebClient client = new WebClient())
            {
                urlContent = client.DownloadString(url);
            }            
            return urlContent;
        }
        
        /// <summary>
        /// Gets all urls from a content of a url
        /// </summary>
        /// <param name="urlContent">html content of a url</param>
        /// <returns>array of urls</returns>
        private static string[] GetAllUrlsFromString(string urlContent)
        {
            // splits the html content by spaces
            urlContent = urlContent.Replace(">", "> ");
            urlContent = urlContent.Replace("<", " <");
            urlContent = urlContent.Replace("\"", " ");
            
            string[] urlStrings = urlContent.Split(' ');

            List<string> urlList = new List<string>();                      
            // gets every string result from splitting
            foreach (string url in urlStrings)
            {
                // string to add to the list
                string urlToAdd = null;
                // checks if the string contains a url that belongs to the domain of the site being crawled
                if (url.Contains(rootDomainIdentifier))
                {
                    // checks if a string contains https:// prefix
                    if (url.Contains("https://"))
                    {
                        // checks if the url retrieved is an xml
                        if (url.Contains(".xml"))
                        {
                            int firstStringPosition = url.IndexOf("https://");
                            int secondStringPosition = url.IndexOf(".xml");
                            string stringBetweenTwoStrings = url.Substring(firstStringPosition, secondStringPosition - firstStringPosition + 4);
                            urlToAdd = stringBetweenTwoStrings;
                        }
                        else
                        {
                            urlToAdd = url;
                        }                        
                    }
                }                
                // adds the url to the list to be returned
                if (urlToAdd != null)
                {
                    urlList.Add(urlToAdd);
                }
            }
            // removes duplicate items
            urlList = urlList.Distinct().ToList();

            return urlList.ToArray();
        }        
        
        /// <summary>
        /// gets the title of an web page
        /// </summary>
        /// <param name="urlContent">html content of the url</param>
        /// <returns>returns a string containing the title</returns>
        private static string GetTitleFromUrlContent(string urlContent)
        {
            string title = Regex.Match(urlContent, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
            return title;
        }

        /// <summary>
        /// gets the publication date of an article (only works for article pages; returns null if it is not an article)
        /// </summary>
        /// <param name="urlContent"></param>
        /// <returns></returns>
        private static DateTime? GetDateTimeFromUrlContent(string urlContent)
        {
            // find the key word for the publish date
            int firstStringPosition = urlContent.IndexOf("=\"pubdate\"");
            // makes sure that the pubdate keyword is present in the entire html content
            if (firstStringPosition >= 0)
            {
                // searches for the ending HTML tag where the pubdate is located
                int secondStringPosition = firstStringPosition;
                while (true)
                {
                    secondStringPosition++;
                    if (urlContent[secondStringPosition].Equals('>'))
                    {
                        break;
                    }
                }
                // gets the distance between '="pubdate"' and the ending of the html tag
                string stringBetweenTwoStrings = urlContent.Substring(firstStringPosition, secondStringPosition - firstStringPosition);
                // gets the exact part of the string where the complete datetime is written within the HTML tag
                string date = stringBetweenTwoStrings.Split('"')[3].Trim();
                // gets only the parts relevant to datetime to parse it
                date = date.Substring(0, 19);
                // parses the datetime string to a datetime datatype
                DateTime dt = DateTime.ParseExact(date, "yyyy-MM-ddTHH:mm:ss", null);
                return dt;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// gets the content within the <body> tag of the html of a url
        /// </summary>
        /// <param name="url">target url</param>
        /// <returns>entire body content from an html</returns>
        public static string GetBodyTextFromUrl(string url)
        {
            // gets all the contents of a url
            string urlContent = GetContentsFromUrl(url);
            // finds the contents of the body tag
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            Regex bodyTagRegex = new Regex("<body>(?<theBody>.*)</body>", options);
            Match match = bodyTagRegex.Match(urlContent);
            // makes sure there is body
            if (match.Success)
            {
                // gets the body content
                urlContent = match.Groups["theBody"].Value;
            }
            // removes html tags
            Regex htmlTagRegex = new Regex("\\<[^\\>]*\\>");
            urlContent = htmlTagRegex.Replace(urlContent, String.Empty);
            
            return urlContent;
        }
    }
}
