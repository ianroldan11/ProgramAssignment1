$(document).ready(function () {
    setInterval(function () {
        getMachineCounters();
        countUrlsInQueue();
        countCrawledUrls();
        countEntitiesInHTMLDataTable();
        getThreadNames();
        getThreadStatus();
        getCurrentlyCrawledUrls();        
    }, 3000);

    setInterval(function () {
        getLogReports();
    }, 700);
});

function reset() {
    sendCommand('restart');
    $("#genlogtext").text("");    
    $("#cnnlogtext").text("");
    $("#silogtext").text("");
    $("#errlogtext").text("");
}

function sendCommand(command) {    
    dataString = { "command": command };
    $.ajax({
        type: "POST",
        data: JSON.stringify(dataString),
        url: "http://localhost:56267/AzureStorageManager.asmx/AddControlCommandToQueue",
        contentType: "application/json; charset=utf-8",
        success: function (msg) {
            //M.toast({ html: 'Switched ' + threadName + ' in ' + partitionKey, displayLength: 1000, classes: 'brown pulse' });
        },
        error: function (msg) {
        }
    });
}

function getMachineCounters() {
    $.ajax({
        type: "POST",
        url: "http://localhost:56267/AzureStorageManager.asmx/RetrieveMachineCounters",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var counters = JSON.parse(this);  
                displayMachineCounters(counters);                
            });
        },
        error: function (msg) {
        }
    });
}

function countUrlsInQueue() {
    $.ajax({
        type: "POST",
        url: "http://localhost:56267/AzureStorageManager.asmx/CountUrlInQueues",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var urlCount = JSON.parse(this);
                displayUrlsInQueue(urlCount);
            });
        },
        error: function (msg) {
        }
    }); 
}

function countCrawledUrls() {
    $.ajax({
        type: "POST",
        url: "http://localhost:56267/AzureStorageManager.asmx/CountCrawledUrls",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var count = JSON.parse(this);
                displayCountCrawledUrls(count);
            });
        },
        error: function (msg) {
        }
    });
}

function countEntitiesInHTMLDataTable() {
    $.ajax({        
        type: "POST",        
        url: "http://localhost:56267/AzureStorageManager.asmx/CountEntitiesInHTMLDataTable", 
        success: function (xml) {
            var data = $(xml).find('int').text();
            displayCountEntitiesInHTMLDataTable(data);
        },
        error: function (msg) {
        }
    });
}

function getThreadNames() {
    $.ajax({
        type: "POST",
        url: "http://localhost:56267/AzureStorageManager.asmx/GetThreadName",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var names = JSON.parse(this);
                displayThreadNames(names);
            });
        },
        error: function (msg) {
        }
    });
}

function getThreadStatus() {
    $.ajax({
        type: "POST",
        url: "http://localhost:56267/AzureStorageManager.asmx/GetThreadStatus",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var status = JSON.parse(this);
                displayThreadStatus(status);
            });
        },
        error: function (msg) {
        }
    });
}

function getCurrentlyCrawledUrls() {
    $.ajax({
        type: "POST",
        url: "http://localhost:56267/AzureStorageManager.asmx/GetCurrentlyCrawledUrl",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var urls = JSON.parse(this);
                displayCurrentlyCrawledUrls(urls);
            });
        },
        error: function (msg) {
        }
    });
}

function getLogReports() {
    $.ajax({
        type: "POST",
        url: "http://localhost:56267/AzureStorageManager.asmx/RetrieveLogReports",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var logs = JSON.parse(this);
                displayLogReports(logs);
            });
        },
        error: function (msg) {
        }
    });
}


function displayMachineCounters(counters) {
    $("#ram").text(counters[0]);
    $("#cpu").text(counters[1]);
    $("#cpubar").css("width", counters[1] + "%");
    cpubar
    
}

function displayUrlsInQueue(urlCount) {
    $("#cnnurlqueue").text(urlCount[0]);
    $("#siurlqueue").text(urlCount[1]);
    $("#totalurlqueue").text(Number(urlCount[0]) + Number(urlCount[1]));
}

function displayCountCrawledUrls(count) {
    $("#crawledurls").text(count[0]);    
}

function displayCountEntitiesInHTMLDataTable(dataCount) {
    $("#tableentities").text(dataCount);      
}

function displayThreadNames(names) {
    $("#cnnthread1name").text(names[0]);
    $("#cnnthread2name").text(names[1]);
    $("#sithread1name").text(names[2]);
    $("#sithread2name").text(names[3]);
}

function displayThreadStatus(status) {
    $("#cnnthread1state").text(status[0]);
    $("#cnnthread2state").text(status[1]);
    $("#sithread1state").text(status[2]);
    $("#sithread2state").text(status[3]);    
}

function displayCurrentlyCrawledUrls(urls) {
    $("#cnnthread1urlcrawled").text(urls[0]);
    $("#cnnthread2urlcrawled").text(urls[1]);
    $("#sithread1urlcrawled").text(urls[2]);
    $("#sithread2urlcrawled").text(urls[3]);      
}

function displayLogReports(logs) {
    writeToTextArea("#genlogtext", logs[0]);
    writeToTextArea("#cnnlogtext", logs[1]);
    writeToTextArea("#silogtext", logs[2]);
    writeToTextArea("#errlogtext", logs[3]); 
    writeToTextArea("#trielogreport", logs[4]);   
}

function writeToTextArea(id, logMessage) {
    if (logMessage != null) {
        $(id).text($(id).text() + logMessage + "\n");
        var textarea = $(id);
        textarea.scrollTop(textarea[0].scrollHeight - textarea.height());
    }
}

