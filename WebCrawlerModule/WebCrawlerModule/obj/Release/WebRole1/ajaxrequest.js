var machineCountersAJAX;
var indexedDataCountAJAX;
var urlQueueSizesAJAX;
var stoppedThreadCountAJAX;

var NumberOfUrlsCrawledForSpecificWebCrawlerAJAX;
var ThreadStatusOfWebCrawlerAJAX;
var CurrentlyCrawledUrlsOfWebCrawlerAJAX;
var SwitchStatesOfWebCrawler;

$(document).ready(function () {
    startService();
    setInterval(function () {
       /* if (machineCountersAJAX && machineCountersAJAX.readyState != 4) {
            machineCountersAJAX.abort();
        }

        if (indexedDataCountAJAX && indexedDataCountAJAX.readyState != 4) {
            indexedDataCountAJAX.abort();
        }

        if (urlQueueSizesAJAX && urlQueueSizesAJAX.readyState != 4) {
            urlQueueSizesAJAX.abort();
        }

        if (stoppedThreadCountAJAX && stoppedThreadCountAJAX.readyState != 4) {
            stoppedThreadCountAJAX.abort();
        }  */   
        
        getSingleWebCrawlerData();

    }, 5000);

    setInterval(function () {
        getMachineCounters();
        getIndexedDataCount();
        getUrlQueueSizes();
        getStoppedThreadCount();

    }, 3000);

    setInterval(function () {        
        getWebLogs();

    }, 600);

    $("#generallogclear").click(function () {
        $("#generallogreports").text("");
    });

    $("#bleacherreportclear").click(function () {
        $("#bleacherreportlog").text("");
    });

    $("#cnnclear").click(function () {
        $("#cnnlog").text("");
    });

    $("#espnclear").click(function () {
        $("#espnlog").text("");
    });

    $("#forbesclear").click(function () {
        $("#forbeslog").text("");
    });

    $("#imdbclear").click(function () {
        $("#imdblog").text("");
    });

    $("#startallcrawlers").click(function () {
        setAllSwitches("StartAll");
    });

    $("#stopallcrawlers").click(function () {
        setAllSwitches("StopAll");
    });

    $("#resetallcrawlers").click(function () {
        resetAllCrawlers();
    });   

    $("#startthreads").click(function () {
        setAllSwitchesInWebCrawler(true);
    });

    $("#stopthreads").click(function () {
        setAllSwitchesInWebCrawler(false);
    });
});

function setAllSwitchesInWebCrawler(willStart) {
    $('#dogswitch').prop('checked', willStart);
    $('#dogswitch').trigger('change');

    $('#catswitch').prop('checked', willStart);
    $('#catswitch').trigger('change');

    $('#horseswitch').prop('checked', willStart);
    $('#horseswitch').trigger('change');

    $('#rabbitswitch').prop('checked', willStart);
    $('#rabbitswitch').trigger('change');
}

function getSingleWebCrawlerData() {
    /*if (NumberOfUrlsCrawledForSpecificWebCrawlerAJAX && NumberOfUrlsCrawledForSpecificWebCrawlerAJAX.readyState != 4) {
        NumberOfUrlsCrawledForSpecificWebCrawlerAJAX.abort();
    }

    if (ThreadStatusOfWebCrawlerAJAX && ThreadStatusOfWebCrawlerAJAX.readyState != 4) {
        ThreadStatusOfWebCrawlerAJAX.abort();
    }

    if (CurrentlyCrawledUrlsOfWebCrawlerAJAX && CurrentlyCrawledUrlsOfWebCrawlerAJAX.readyState != 4) {
        CurrentlyCrawledUrlsOfWebCrawlerAJAX.abort();
    }

    if (SwitchStatesOfWebCrawler && SwitchStatesOfWebCrawler.readyState != 4) {
        SwitchStatesOfWebCrawler.abort();
    }
    */

    getNumberOfUrlsCrawledForSpecificWebCrawler(currentCarouselIndex);
    getThreadStatusOfWebCrawler(currentCarouselIndex);
    getCurrentlyCrawledUrlsOfWebCrawler(currentCarouselIndex);
    getSwitchStatesOfWebCrawler(currentCarouselIndex);

    getCurrentlyCrawledUrls(currentCarouselIndex);
}

function startService() {
    $.ajax({
        type: "POST",
        url: "WebService1.asmx/StartAutoRetrieve",        
        success: function (msg) {            
        },
        error: function (msg) {
        }
    });
}

function getMachineCounters() {
    machineCountersAJAX = $.ajax({
        type: "POST",
        url: "WebService1.asmx/GetAllMachineCounterValues",       
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {            
            var data = eval(msg);            
            jQuery.each(data, function (rec) {                
                var sites = JSON.parse(this);       
                var x = 0;
                sites.forEach(function (siteCounters) {                    
                    displayMachineCounters(siteCounters, x);  
                    x++;
                });                
            });
        },
        error: function (msg) {
        }
    });
}

function getIndexedDataCount() {
    indexedDataCountAJAX = $.ajax({
        type: "POST",
        url: "WebService1.asmx/GetAllIndexedDataCount",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var counters = JSON.parse(this);
                var x = 0;
                counters.forEach(function (counter) {
                    displayIndexedDataCounters(counter, x);
                    x++;
                });             
            });
        },
        error: function (msg) {
        }
    });
}

function getUrlQueueSizes() {
    urlQueueSizesAJAX = $.ajax({
        type: "POST",
        url: "WebService1.asmx/GetAllURLQueueSize",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var queues = JSON.parse(this);
                var x = 0;
                queues.forEach(function (queueSize) {
                    displayQueueSizes(queueSize, x);
                    x++;
                });
            });
        },
        error: function (msg) {
        }
    });
}

function getStoppedThreadCount() {
    stoppedThreadCountAJAX = $.ajax({
        type: "POST",
        url: "WebService1.asmx/GetAllStoppedThreadCount",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var domains = JSON.parse(this);
                var x = 0;
                domains.forEach(function (thread) {                    
                    displayThreadCountStatuses(thread, x);
                    x++;
                });
            });
        },
        error: function (msg) {
        }
    });
}

function getWebLogs() {
    $.ajax({
        type: "POST",
        url: "WebService1.asmx/GetWebLogs",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var queues = JSON.parse(this);
                var x = 0;
                queues.forEach(function (log) {
                    displayLogs(log, x);
                    x++;
                });
            });
        },
        error: function (msg) {
        }
    });
}

function setAllSwitches(methodName) {
    $.ajax({
        type: "POST",
        url: "WebService1.asmx/" + methodName,        
        success: function (msg) {
            
        },
        error: function (msg) {
        }
    });
}

function getNumberOfUrlsCrawledForSpecificWebCrawler(index) {
    NumberOfUrlsCrawledForSpecificWebCrawlerAJAX = $.ajax({
        type: "POST",
        data: "webCrawlerIndex=" + index,
        url: "WebService1.asmx/GetNumberOfUrlsCrawledForWebCrawler",
        success: function (xml) {
            var data = $(xml).find('int').text();
            displayNumberOfUrlsCrawledInWebCrawler(data);
        },
        error: function (msg) {
        }
    });
}

function getThreadStatusOfWebCrawler(index) {
    dataString = { "webCrawlerIndex": index };
    ThreadStatusOfWebCrawlerAJAX = $.ajax({
        type: "POST",
        data: JSON.stringify(dataString),
        url: "WebService1.asmx/GetThreadStatusOfWebCrawler",
        contentType: "application/json; charset=utf-8",        
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var sites = JSON.parse(this);
                var x = 0;
                sites.forEach(function (thread) {
                    displayThreadStatusOfWebCrawler(thread, x);
                    x++;
                });
            });
        },
        error: function (msg) {
        }
    });
}

function getCurrentlyCrawledUrlsOfWebCrawler(index) {   
    dataString = { "webCrawlerIndex": index };
    CurrentlyCrawledUrlsOfWebCrawlerAJAX = $.ajax({
        type: "POST",
        data: JSON.stringify(dataString),
        url: "WebService1.asmx/GetCurrentURLCrawledOfWebCrawler",
        contentType: "application/json; charset=utf-8",        
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var sites = JSON.parse(this);
                var x = 0;
                sites.forEach(function (thread) {
                    displayCurrentlyCrawledUrlsOfWebCrawler(thread, x);
                    x++;
                });
            });
        },
        error: function (msg) {
        }
    });
}

function getSwitchStatesOfWebCrawler(index) {   
    dataString = { "webCrawlerIndex": index };
    SwitchStatesOfWebCrawler = $.ajax({
        type: "POST",
        data: JSON.stringify(dataString),
        url: "WebService1.asmx/GetSwitchStates",
        contentType: "application/json; charset=utf-8",        
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var sites = JSON.parse(this);
                var x = 0;
                sites.forEach(function (thread) {
                    displaySwitchStatesOfWebCrawler(thread, x);
                    x++;
                });
            });
        },
        error: function (msg) {
        }
    });
}

function setSwitch(switchID, partitionKey, threadName) {
    var methodName;
    if ($(switchID).prop('checked')) {
        methodName = "StartSpecificSwitch";
    }
    else {
        methodName = "StopSpecificSwitch";
    }    
    dataString = { "partitionKey": partitionKey, "rowKey": threadName };    
    $.ajax({
        type: "POST",
        data: JSON.stringify(dataString),
        url: "WebService1.asmx/" + methodName,
        contentType: "application/json; charset=utf-8",
        success: function (msg) {
            M.toast({ html: 'Switched ' + threadName + ' in ' + partitionKey, displayLength: 1000, classes: 'brown pulse' });
        },
        error: function (msg) {
        }
    });
}

function getCurrentlyCrawledUrls(index) {
    dataString = { "webCrawlerIndex": index };
    $.ajax({
        type: "POST",
        data: JSON.stringify(dataString),
        url: "WebService1.asmx/GetRecentlyCrawledUrls",
        contentType: "application/json; charset=utf-8",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var sites = JSON.parse(this);
                var x = 0;
                sites.forEach(function (thread) {
                    printCurrentlyCrawledUrls(thread, x);
                    x++;
                });
            });
        },
        error: function (msg) {
        }
    });
}

function getHTMLContentData(element, index) {
    var url = $(element).text();

    if (url != "-") {
        dataString = { "webCrawlerIndex": index, "rowKey": url };
        $.ajax({
            type: "POST",
            data: JSON.stringify(dataString),
            url: "WebService1.asmx/GetExtractedData",
            contentType: "application/json; charset=utf-8",
            success: function (msg) {                
                var data = eval(msg) ;
                jQuery.each(data, function (rec) {
                    var content = JSON.parse(this);
                    printHTMLContentData(content.Title, content.PublishDate, content.BodyContent);                           
                });
            },
            error: function (msg) {
            }
        });
    }    
}

function getBodyContentFromBlobFile(blobFileName) {
    dataString = { "blobFileReference": blobFileName};
    $.ajax({
        type: "POST",
        data: JSON.stringify(dataString),
        url: "WebService1.asmx/GetHTMLBodyContent",
        contentType: "application/json; charset=utf-8",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                $("#modalbody").text(this);
            });
        },
        error: function (msg) {
        }
    });
}

function resetAllCrawlers() {
    $.ajax({
        type: "POST",
        url: "WebService1.asmx/RequestForReset",        
        success: function (msg) {
            alert("Finish Resetting");
        },
        error: function (msg) {
            alert(msg);
        }
    });
}


