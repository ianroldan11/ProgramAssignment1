// METHODS BELOW ARE FOR DISPLAYING COLLECTED DATA FROM AJAX METHODS, THESE ARE USUALLY PERFORMED AFTER SUCCESFUL AJAX REQUESTS
var machineCountersIDPrefix = ["#bleacherreport", "#cnn", "#espn", "#forbes", "#imdb"];

function displayMachineCounters(siteCountersArray, machineCountersIDPrefixIndex) {
    var cpuID = machineCountersIDPrefix[machineCountersIDPrefixIndex] + "cpu";
    var ramID = machineCountersIDPrefix[machineCountersIDPrefixIndex] + "ram";
    var machBarID = machineCountersIDPrefix[machineCountersIDPrefixIndex] + "machbar";

    $(cpuID).text(siteCountersArray[0]);
    $(ramID).text(siteCountersArray[1]);
    $(machBarID).css("width", siteCountersArray[0] + "%");
}

var numIndexDataStored = ["#numdatastored", "#numblobfiles", "#numurlscrawled"];

function displayIndexedDataCounters(indexedDataCounter, numIndexDataStoredIndex) {
    $(numIndexDataStored[numIndexDataStoredIndex]).text(indexedDataCounter);
}

var queueSizeID = ["#bleacherreportqueuesize", "#cnnqueuesize", "#espnqueuesize", "#forbesqueuesize", "#imdbqueuesize"];

function displayQueueSizes(queueSize, queueSizeIDIndex) {
    $(queueSizeID[queueSizeIDIndex]).text(queueSize);
}

var threadStatusID = ["#bleacherreport", "#cnn", "#espn", "#forbes", "#imdb"];

function displayThreadCountStatuses(stoppedThreadsCount, threadStatusIDIndex) {
    var numRunningID = threadStatusID[threadStatusIDIndex] + "numrunning";
    var numStoppedID = threadStatusID[threadStatusIDIndex] + "numstopped";
    var threadsBarID = threadStatusID[threadStatusIDIndex] + "threadsbar";    
    var runningThreadCount = 4 - Number(stoppedThreadsCount);
    var threadCountRatio = (runningThreadCount * 100) / 4

    $(numRunningID).text(runningThreadCount);
    $(numStoppedID).text(stoppedThreadsCount);
    $(threadsBarID).css("width", threadCountRatio + "%");
}

var textareaID = ["#generallogreports", "#bleacherreportlog", "#cnnlog", "#espnlog", "#forbeslog", "#imdblog"];

function displayLogs(logMessage, textareaIDIndex) {
    $(textareaID[textareaIDIndex]).text($(textareaID[textareaIDIndex]).text() + logMessage + "\n");
    var textarea = $(textareaID[textareaIDIndex]);
    textarea.scrollTop(textarea[0].scrollHeight - textarea.height());
}

function displayNumberOfUrlsCrawledInWebCrawler(numberToDisplay) {    
    $("#webcrawlerurlscrawled").text(numberToDisplay);
}

var webCrawlerThreadStatusID = ["#catstatus", "#dogstatus", "#horsestatus", "#rabbitstatus"];

function displayThreadStatusOfWebCrawler(webCrawlerThreadStatus, webCrawlerThreadStatuIDIndex) {
    $(webCrawlerThreadStatusID[webCrawlerThreadStatuIDIndex]).text(webCrawlerThreadStatus);
}

var currentlyCrawledID = ["#caturl", "#dogurl", "#horseurl", "#rabbiturl"];

function displayCurrentlyCrawledUrlsOfWebCrawler(crawledUrl, currentlyCrawledIDIndex) {
    $(currentlyCrawledID[currentlyCrawledIDIndex]).text(crawledUrl);
}

var switchID = ["#catswitch", "#dogswitch", "#horseswitch", "#rabbitswitch"];

function displaySwitchStatesOfWebCrawler(switchState, switchIDIndex) {
    $(switchID[switchIDIndex]).prop('checked', !!Number(switchState));    
}

function printCurrentlyCrawledUrls(url, indexCount) {
    var recentUrlID = "#recentUrl" + indexCount;
    $(recentUrlID).text(url);
}

function printHTMLContentData(title, publishDate, blobFileName) {
    if (title == null || title == "") {
        $("#modaltitle").text("Title Not Available");
    }
    else {
        $("#modaltitle").text(title);        
    }    

    if (publishDate == null) {
        $("#modaldate").text("Date not Available");
    }
    else {
        $("#modaldate").text(ToJavaScriptDate(publishDate));
    }
    getBodyContentFromBlobFile(blobFileName);   
}

function ToJavaScriptDate(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
}