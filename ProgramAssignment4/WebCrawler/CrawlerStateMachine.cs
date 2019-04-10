using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml.Linq;

namespace WebCrawler
{
    /// <summary>
    /// Possible states of a crawler thread
    /// </summary>
    public enum State
    {        
        Starting,
        Loading,
        Crawling,
        Idling,
        Stopped,
        Restarting,
        Debug
    }

    public class CrawlerStateMachine
    {
        // unique identifier for blob files
        public static int blobFileCounter = 0;
        // determines what procedures the crawler will use on the next iteration of DoWork
        public State CurrentState { get; set; }
        // determines if the thread crawler is the one analyzing robots.txt - only one thread crawler should analyze to avoid duplicates
        public bool IsStarter { get; }
        // url that is currently being crawled
        public string CurrentlyCrawledUrl { get; set; }
        // type of url being crawled (html or xml)
        public UrlType CurrentlyCrawledUrlType { get; set; }
        // general configurations and methods particular to a website crawler
        public WebCrawlerManager WebCrawlerManager { get; set; }

        /// <summary>
        /// Constructor for the crawler's machine
        /// </summary>
        /// <param name="willStartTheCrawl">Determines if this crawler will be the analyzer of the robots.txt</param>
        /// <param name="sharedWebCrawlerManager">which configuration would be used</param>
        public CrawlerStateMachine(bool willStartTheCrawl, WebCrawlerManager sharedWebCrawlerManager)
        {
            CurrentState = State.Stopped;
            IsStarter = willStartTheCrawl;
            WebCrawlerManager = sharedWebCrawlerManager;
        }

        /// <summary>
        /// main method for the crawler's activity
        /// </summary>
        public void DoWork()
        {
            // loops repeatedly
            while (true)
            {
                try
                {
                    WorkerRole.PerformLoggingOperationsOnSpecificQueue("State: " + CurrentState, WebCrawlerManager.LogQueueReference);
                    // Change state of thread saved in the azure table
                    WorkerRole.ASMClient.UpdateThreadStatus(Thread.CurrentThread.Name, CurrentState.ToString());
                    switch (CurrentState)
                    {
                        case State.Starting:
                            OnStartState();
                            break;
                        case State.Loading:
                            OnLoadState();
                            break;
                        case State.Crawling:
                            OnCrawlState();
                            break;
                        case State.Idling:
                            OnIdleState();
                            break;
                        case State.Stopped:
                            OnStopState();
                            break;
                        case State.Restarting:
                            OnRestartState();
                            break;
                        case State.Debug:
                            OnDebug();
                            break;
                    }
                }
                catch(Exception e)
                {
                    WorkerRole.PerformErrorLoggingOperations(e.ToString());
                } 
                // pause for 1 second
                Thread.Sleep(1000);
            }            
        }

        /// <summary>
        /// Includes getting sitemaps and disallowed directories from robots.text
        /// </summary>
        public void OnStartState()
        {
            // Robot.txt analysis will only be done on one thread, while the other will move on stand by
            if (IsStarter)
            {
                // Analyze Robots.txt
                try
                {                    
                    WebCrawlerManager.RobotsCrawler = new RobotsAnalyzer(WebCrawlerManager.RobotsUrl);
                    WorkerRole.PerformLoggingOperationsOnSpecificQueue("Finished Crawling Robots.txt: " + WebCrawlerManager.RobotsUrl, WebCrawlerManager.LogQueueReference);
                }
                catch (Exception e)
                {
                    WorkerRole.PerformErrorLoggingOperations(e.ToString());
                }                
                // Store SiteMaps To queue
                string logReport = "";
                try
                {
                    foreach (string siteMap in WebCrawlerManager.RobotsCrawler.SiteMaps)
                    {
                        WorkerRole.ASMClient.AddUrlToQueue(siteMap, WebCrawlerManager.UrlQueueReference);
                        logReport += siteMap + "\n";
                    }
                    WorkerRole.PerformLoggingOperationsOnSpecificQueue("Added the following Urls to " + WebCrawlerManager.UrlQueueReference + "queue:\n" + logReport, WebCrawlerManager.LogQueueReference);
                }
                catch (Exception e)
                {                    
                    WorkerRole.PerformErrorLoggingOperations(e.ToString());
                }
            }
            // Change State
            CurrentState = State.Stopped;   // have to manually start via dashboard after analyzing robots.txt
        }

        /// <summary>
        /// Checks the validity of the url grabbed from the queue
        /// </summary>
        public void OnLoadState()
        {            
            try
            {
                WorkerRole.ASMClient.UpdateCurrentlyCrawledUrlValue(Thread.CurrentThread.Name, "none");
                int? numberOfMessagesInUrlQueue = WorkerRole.ASMClient.CountMessagesInQueue(WebCrawlerManager.UrlQueueReference);
                if (numberOfMessagesInUrlQueue == null || numberOfMessagesInUrlQueue < 1)
                {
                    // switch to idle if no message from queue is available
                    WorkerRole.PerformLoggingOperationsOnSpecificQueue("There are no more Urls To Crawl. Crawler will proceed to IDLE state.", WebCrawlerManager.LogQueueReference);
                    CurrentState = State.Idling;
                }
                else
                {
                    string reportLog = "";
                    // Grab Url From Queue                    
                    string urlToBeCrawled = WorkerRole.ASMClient.RetrieveMessageFromQueue(WebCrawlerManager.UrlQueueReference);
                    if (urlToBeCrawled == null)
                    {
                        return;
                    }
                    CurrentlyCrawledUrl = urlToBeCrawled;
                    reportLog += "URL: " + urlToBeCrawled + " has been grabbed from the queue.\n";                    
                    // Check if url is xml, html or invalid
                    CurrentlyCrawledUrlType = WebCrawlerManager.UrlTypeChecker(urlToBeCrawled);
                    reportLog += "URL: " + urlToBeCrawled + " type is " + CurrentlyCrawledUrlType + ".\n";                    
                    // Restart loading state if url is invalid
                    if (CurrentlyCrawledUrlType == UrlType.Invalid)
                    {
                        reportLog += "URL: " + urlToBeCrawled + " will be ignored.\n";
                        WorkerRole.PerformLoggingOperationsOnSpecificQueue(reportLog, WebCrawlerManager.LogQueueReference);
                        return;
                    }
                    // Check if state has changed in the middle of the loading process
                    if (CurrentState == State.Stopped || CurrentState == State.Restarting)
                    {
                        return;
                    }
                    // Check if url belongs to the domain
                    if (!WebCrawlerManager.UrlIsIncludedInDomain(urlToBeCrawled))
                    {
                        reportLog += "URL: " + urlToBeCrawled + "is not a part of the domain being crawled so it will be ignored.\n";
                        WorkerRole.PerformLoggingOperationsOnSpecificQueue(reportLog, WebCrawlerManager.LogQueueReference);
                        return;
                    }
                    // Check if state has changed in the middle of the loading process
                    if (CurrentState == State.Stopped || CurrentState == State.Restarting)
                    {
                        return;
                    }
                    // Check if url is disallowed by robots.txt
                    if (WebCrawlerManager.UrlIsDisallowed(urlToBeCrawled))
                    {
                        reportLog += "URL: " + urlToBeCrawled + " is disallowed by Robots.txt and will be ignored.\n";
                        WorkerRole.PerformLoggingOperationsOnSpecificQueue(reportLog, WebCrawlerManager.LogQueueReference);
                        return;
                    }
                    // Check if state has changed in the middle of the loading process
                    if (CurrentState == State.Stopped || CurrentState == State.Restarting)
                    {
                        return;
                    }
                    // Check if url is already visited
                    if (WorkerRole.ASMClient.GetEntityCount("RowKey", System.Net.WebUtility.UrlEncode(urlToBeCrawled)) > 0)
                    {
                        reportLog += "URL: " + urlToBeCrawled + " has already been visited before.\n";
                        // Add one popularity count if visited already
                        WorkerRole.ASMClient.IncrementPopularityCount(CurrentlyCrawledUrl);
                        WorkerRole.PerformLoggingOperationsOnSpecificQueue(reportLog, WebCrawlerManager.LogQueueReference);
                        return;
                    }
                    // Check if state has changed in the middle of the loading process
                    if (CurrentState == State.Stopped || CurrentState == State.Restarting)
                    {
                        return;
                    }                    
                    WorkerRole.PerformLoggingOperationsOnSpecificQueue(reportLog, WebCrawlerManager.LogQueueReference);
                    // Check if state has changed in the middle of the loading process
                    if (CurrentState == State.Stopped || CurrentState == State.Restarting)
                    {
                        return;
                    }
                    // Switch to Crawling
                    CurrentState = State.Crawling;
                }                
            }
            catch (Exception e)
            {
                WorkerRole.PerformErrorLoggingOperations(e.ToString());
            }            
            
            
        }

        /// <summary>
        /// gets urls from the content, if url is an html - also gets the title, date and body snippets
        /// </summary>
        public void OnCrawlState()
        {
            try
            {
                IEnumerable<string> retrievedUrls = null;
                WorkerRole.ASMClient.UpdateCurrentlyCrawledUrlValue(Thread.CurrentThread.Name, CurrentlyCrawledUrl);
                // for htmls only
                if (CurrentlyCrawledUrlType == UrlType.HTML)
                {
                    string reportLogRetrieve = "Retrieved Data From URL: " + CurrentlyCrawledUrl + "\n";
                    HTMLReader hTMLReader = new HTMLReader(CurrentlyCrawledUrl);
                    //get title
                    string retrievedTitle = hTMLReader.Title;
                    reportLogRetrieve += "Title : " + retrievedTitle + "\n";
                    if (CurrentState == State.Stopped || CurrentState == State.Restarting)
                    {
                        return;
                    }
                    //get date
                    DateTime ? publishDate = hTMLReader.PublishDate;
                    reportLogRetrieve += "Date : " + publishDate.ToString() + "\n";
                    if (CurrentState == State.Stopped || CurrentState == State.Restarting)
                    {
                        return;
                    }
                    //get body
                    IEnumerable<string> bodyContent = hTMLReader.GetBodyContent();
                    reportLogRetrieve += "Retrieved Body Content";
                    if (CurrentState == State.Stopped || CurrentState == State.Restarting)
                    {
                        return;
                    }
                    WorkerRole.PerformLoggingOperationsOnSpecificQueue(reportLogRetrieve, WebCrawlerManager.LogQueueReference);
                    //store the 3 data
                    string blobName = WebCrawlerManager.RequiredDomain + blobFileCounter + Guid.NewGuid();
                    WebCrawlerManager.StoreDataToTable(retrievedTitle, CurrentlyCrawledUrl, publishDate, blobName);
                    WebCrawlerManager.CreateFileInBlob(bodyContent, blobName);
                    blobFileCounter++;
                    if (CurrentState == State.Stopped || CurrentState == State.Restarting)
                    {
                        return;
                    }
                    //get urls
                    retrievedUrls = hTMLReader.GetUrls(WebCrawlerManager.BaseUri);
                    if (CurrentState == State.Stopped || CurrentState == State.Restarting)
                    {
                        return;
                    }
                    WorkerRole.PerformLoggingOperationsOnSpecificQueue("Retrieved " + retrievedUrls.Count() + " from URL: " + CurrentlyCrawledUrl, WebCrawlerManager.LogQueueReference);
                }

                // for xml only
                else if (CurrentlyCrawledUrlType == UrlType.XML)
                {
                    //get urls and remove 3 months old urls
                    SiteMapReader siteMapReader = new SiteMapReader(CurrentlyCrawledUrl);
                    List<XElement> allUrlsInPage = siteMapReader.GetUrls().ToList();
                    List<XElement> allDatesInPage = siteMapReader.GetDates().ToList();
                    retrievedUrls = WebCrawlerManager.GetUrlsFromListOfElements(allUrlsInPage, allDatesInPage);
                    if (CurrentState == State.Stopped || CurrentState == State.Restarting)
                    {
                        return;
                    }
                    WorkerRole.PerformLoggingOperationsOnSpecificQueue("Retrieved " + retrievedUrls.Count() +" from URL: " + CurrentlyCrawledUrl, WebCrawlerManager.LogQueueReference);                    
                }

                if (CurrentState == State.Stopped || CurrentState == State.Restarting)
                {
                    return;
                }
                //store urls to queue
                string reportLogUrls = "URLs Stored to queue: " + WebCrawlerManager.UrlQueueReference + "\n";
                foreach (string retrievedUrl in retrievedUrls)
                {
                    if (CurrentState == State.Stopped || CurrentState == State.Restarting)
                    {
                        return;
                    }
                    WorkerRole.ASMClient.AddUrlToQueue(retrievedUrl, WebCrawlerManager.UrlQueueReference);
                    reportLogUrls += retrievedUrl + "\n";
                }
                WorkerRole.PerformLoggingOperationsOnSpecificQueue(reportLogUrls, WebCrawlerManager.LogQueueReference);

            }
            catch(Exception e)
            {
                WorkerRole.PerformErrorLoggingOperations(e.ToString());
            }

            WorkerRole.ASMClient.IncrementNumberOfCrawledUrls("Total");
            CurrentState = State.Loading;
        }

        /// <summary>
        /// Waits for an available url to crawl or waits for a change in the crawler's state
        /// </summary>
        public void OnIdleState()
        {
            WorkerRole.ASMClient.UpdateCurrentlyCrawledUrlValue(Thread.CurrentThread.Name, "none");
            while (WorkerRole.ASMClient.CountMessagesInQueue(WebCrawlerManager.UrlQueueReference) < 1)
            {
                // if stop procede to stop state
                if (CurrentState == State.Stopped || CurrentState == State.Restarting || WebCrawlerManager.StopIterations)
                {
                    return;
                }                
            }
            CurrentState = State.Loading;
        }

        /// <summary>
        /// No event happens during stop
        /// </summary>
        public void OnStopState()
        {
            WorkerRole.ASMClient.UpdateCurrentlyCrawledUrlValue(Thread.CurrentThread.Name, "none");
            while (CurrentState == State.Stopped)
            {

            }
        }

        /// <summary>
        /// makes sure crawler threads are not doing anything during restart sequence
        /// </summary>
        public void OnRestartState()
        {
            while (CurrentState == State.Restarting)
            {

            }
        }

        /// <summary>
        /// Nothing
        /// </summary>
        public void OnDebug()
        {
            //CurrentlyCrawledUrl = "https://www.si.com/college-basketball/2019/03/29/gonzaga-florida-state-march-madness-ncaa-tournament";
            CurrentState = State.Loading;
        }
    }


}