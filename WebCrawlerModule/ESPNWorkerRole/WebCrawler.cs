using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESPNWorkerRole
{
    class WebCrawler
    {
        public static readonly string seedUrl = "http://www.espn.com/robots.txt";
        public static readonly string rootDomainIdentifier = "espn.com";
        public static string[] disallowedDirectories;

        public static string[] StartCollectingFromSeed()
        {
            string urlContent = GetContentsFromUrl(seedUrl);
            disallowedDirectories = ScanDisallowedDirectoriesFromSeed(urlContent);
            return GetAllUrlsFromString(urlContent);
        }

        public static string[] GetAllValidUrls(string url)
        {
            string urlContent = GetContentsFromUrl(url);
            string[] retrievedUrls = GetAllUrlsFromString(urlContent);
            List<string> validUrls = new List<string>();

            foreach (string retrievedUrl in retrievedUrls)
            {
                if (UrlIsAllowed(retrievedUrl))
                {
                    validUrls.Add(retrievedUrl);
                }
                validUrls.Add(retrievedUrl);
            }

            return validUrls.ToArray();
        }

        // override
        public static string[] FilterResults(string[] unfliteredUrls)
        {
            List<string> filteredList = new List<string>();

            string[] nbaKeywords = { "nets", "celtics", "knicks", "76ers", "raptors", "bulls", "pistons", "pacers", "bucks", "cavaliers", "hornets", "grizzlies", "heat", "magic", "wizards",
                "hawks", "rockets", "spurs", "pelicans", "mavericks", "timberwolves", "blazers", "thunder", "jazz", "nuggets", "warriors", "clippers", "lakers", "suns", "kings" };

            foreach (string url in unfliteredUrls)
            {
                if (url.Contains("nba"))
                {
                    filteredList.Add(url);
                }

                else
                {
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

            return filteredList.ToArray();
        }

        public static HTMLData CreateDataFromUrl(string url, string blockBlobReferenceNumber)
        {
            string urlContent = GetContentsFromUrl(url);
            HTMLData data = new HTMLData(GetTitleFromUrlContent(urlContent), GetDateTimeFromUrlContent(urlContent), rootDomainIdentifier + blockBlobReferenceNumber, rootDomainIdentifier, url);

            return data;
        }        

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

        private static string[] ScanDisallowedDirectoriesFromSeed(string urlContent)
        {
            
            urlContent = urlContent.Replace("Disallow: ", "!");
            string[] scannedStrings = urlContent.Split('!');
            List<string> disallowedDirectoriesList = new List<string>();
            for (int i = 1; i < scannedStrings.Length; i++)
            {
                disallowedDirectoriesList.Add(scannedStrings[i].Trim());
            }                             

            return disallowedDirectoriesList.ToArray();
        }

        private static string GetContentsFromUrl(string url)
        {
            string urlContent = "";           
            using (WebClient client = new WebClient())
            {
                urlContent = client.DownloadString(url);
            }            
            return urlContent;
        }
        
        // override
        private static string[] GetAllUrlsFromString(string urlContent)
        {
            urlContent = urlContent.Replace(">", "> ");
            urlContent = urlContent.Replace("<", " <");
            urlContent = urlContent.Replace("\"", " ");

            string[] urlStrings = urlContent.Split(' ');

            List<string> urlList = new List<string>();                      

            foreach (string url in urlStrings)
            {
                string urlToAdd = null;

                if (url.Contains(rootDomainIdentifier))
                {
                    if (url.Contains("/nba/"))
                    {
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

                if (urlToAdd != null)
                {
                    urlList.Add(urlToAdd);
                }
            }

            urlList = urlList.Distinct().ToList();

            return urlList.ToArray();
        }

        // override?
        /*private static string CheckForURLSuffix(string prefix, string url)
        {
            string[] validSuffixes = { ".html", ".htm", ".xml" };

            foreach (string suffix in validSuffixes)
            {
                if (url.Contains(suffix))
                {
                    int firstStringPosition = url.IndexOf(prefix);
                    int secondStringPosition = url.IndexOf(suffix);
                    string stringBetweenTwoStrings = url.Substring(firstStringPosition, secondStringPosition - firstStringPosition + suffix.Length);

                    if (stringBetweenTwoStrings.Contains(rootDomainIdentifier))
                    {                       
                        return stringBetweenTwoStrings;
                    }
                }
            }
            return null;
        }*/
        
        private static string GetTitleFromUrlContent(string urlContent)
        {
            string title = Regex.Match(urlContent, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
            return title;
        }

        // override
        private static DateTime? GetDateTimeFromUrlContent(string urlContent)
        {
            int firstStringPosition = urlContent.IndexOf("=\"DC.date.issued\"");

            if (firstStringPosition >= 0)
            {
                int secondStringPosition = firstStringPosition;
                while (true)
                {
                    secondStringPosition++;
                    if (urlContent[secondStringPosition].Equals('>'))
                    {
                        break;
                    }
                }
                string stringBetweenTwoStrings = urlContent.Substring(firstStringPosition, secondStringPosition - firstStringPosition);
                string date = stringBetweenTwoStrings.Split('"')[2].Trim();                
                DateTime dt = DateTime.ParseExact(date, "yyyy-MM-ddTHH:mm:ssZ", null);
                return dt;
            }
            else
            {
                return null;
            }
        }
        
        public static string GetBodyTextFromUrl(string url)
        {
            string urlContent = GetContentsFromUrl(url);
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            Regex bodyTagRegex = new Regex("<body>(?<theBody>.*)</body>", options);
            Match match = bodyTagRegex.Match(urlContent);

            if (match.Success)
            {
                urlContent = match.Groups["theBody"].Value;
            }

            Regex htmlTagRegex = new Regex("\\<[^\\>]*\\>");
            urlContent = htmlTagRegex.Replace(urlContent, String.Empty);
            
            return urlContent;
        }
    }
}
