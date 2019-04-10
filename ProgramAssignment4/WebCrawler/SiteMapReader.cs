using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WebCrawler
{
    public enum XMLType {
        SiteMapIndex,
        UrlSet,
        NotValidSiteMap
    }

    public class SiteMapReader
    {
        // all elements in xml file
        public IEnumerable<XElement> RetrievedElements { get; } 
        // category if xml contains xmls, htmls or is an invalid xml file
        public XMLType XMLType { get; } 
        // url of the current xml being crawled
        private string Url { get; }

        public SiteMapReader(string siteMapToRead)
        {
            this.Url = siteMapToRead;
            this.RetrievedElements = SimpleStreamAxis(siteMapToRead);
            this.XMLType = CheckXMLtype();
        }       

        /// <summary>
        /// gets all elements in the xml content
        /// </summary>
        /// <param name="inputUrl"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> SimpleStreamAxis(string inputUrl)
        {            
            using (XmlReader reader = XmlReader.Create(inputUrl))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {         
                        XElement el = XNode.ReadFrom(reader) as XElement;
                        if (el != null)
                        {
                            yield return el;
                        }                        
                    }
                }
            }
        }

        /// <summary>
        /// checks what type of urls the xml holds
        /// </summary>
        /// <returns></returns>
        private XMLType CheckXMLtype()
        {
            foreach (XElement element in RetrievedElements)
            {
                if (element.Name.LocalName == "sitemap")
                {
                    return XMLType.SiteMapIndex;
                }

                else if (element.Name.LocalName == "url")
                {
                    return XMLType.UrlSet;
                }
            }

            return XMLType.NotValidSiteMap;
        }

        /// <summary>
        /// gets all urls in the xml file
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XElement> GetUrls()
        {            
            return GetAllElementsWithSpecificTag("loc");  
        }

        /// <summary>
        /// gets all the date of each url retrieved
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XElement> GetDates()
        {
            IEnumerable<XElement> dates = GetAllElementsWithSpecificTag("lastmod");
            if (dates.Count() < 1)
            {
                return GetAllElementsWithSpecificTag("news:publication_date");
            }
            else
            {
                return dates;
            }           
        }
        
        /// <summary>
        /// gets elements with the given html tag
        /// </summary>
        /// <param name="htmlTag"></param>
        /// <returns></returns>
        public IEnumerable<XElement> GetAllElementsWithSpecificTag(string htmlTag)
        {
            using (XmlReader reader = XmlReader.Create(this.Url))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (WebCrawlerManager.StopIterations)
                    {
                        break;
                    }
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == htmlTag)
                        {
                            XElement el = XNode.ReadFrom(reader) as XElement;
                            if (el != null)
                            {
                                yield return el;
                            }
                        }
                    }
                }
            }
        }
    }
}
