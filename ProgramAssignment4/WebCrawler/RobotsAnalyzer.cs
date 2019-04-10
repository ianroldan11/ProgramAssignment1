using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    public class RobotsAnalyzer
    {
        // directories with Disallow from robots.txt
        public string[] DisallowedDirectories { get; }
        // gets all site map xml urls from robots.txt
        public string[] SiteMaps { get; }
        // full content of robots.txt
        public string RobotsContent { get; }
        
        public RobotsAnalyzer(string robotsUrl)
        {          
            using (WebClient client = new WebClient())
            {
                this.RobotsContent = client.DownloadString(robotsUrl);
                this.DisallowedDirectories = ScanDisalllowedDirectories().ToArray();
                this.SiteMaps = ScanSiteMaps().ToArray();
            }
        }

        /// <summary>
        /// checks if url given is allowed by robots.txt
        /// </summary>
        /// <param name="url">url to be checked</param>
        /// <returns>boolean decision</returns>
        public bool UrlIsAllowed(string url)
        {
            foreach (string directory in this.DisallowedDirectories)
            {
                if (url.Contains(directory))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets all disallowed directories
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> ScanDisalllowedDirectories()
        {
            string[] lines = RobotsContent.Split('\n');

            foreach (string line in lines)
            {
                if (line.StartsWith("Disallow: "))
                {                    
                    yield return line.Replace("Disallow: ", string.Empty);
                }
            }
        }

        /// <summary>
        /// gets all xml sitemaps
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> ScanSiteMaps()
        {
            string[] lines = RobotsContent.Split('\n');
            
            foreach (string line in lines)
            {
                if (line.StartsWith("Sitemap: "))
                {                    
                    yield return line.Replace("Sitemap: ", string.Empty);
                }
            }
        }        
    }
}
