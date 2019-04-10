$(document).ready(function () {
    startMachineCounter();
    setInterval(function () {
        getTrieMachineCounters();
        getTrieDataStates();
        getTrieDataCount();
        getViewsDataCount();
        getRecentlySearchedData();
    }, 5000);

    setInterval(function () {
       
    }, 700);
});

function startMachineCounter() {   
    $.ajax({
        type: "POST",        
        url: "http://localhost:56267/trie/TrieManager.asmx/StartMachineCounter",
        contentType: "application/json; charset=utf-8",
        success: function (msg) {           
        },
        error: function (msg) {
        }
    });
}

function getTrieMachineCounters() {
    $.ajax({
        type: "POST",
        url: "http://localhost:56267/trie/TrieManager.asmx/GetMachineCounterValues",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var counters = JSON.parse(this);  
                displayTrieMachineCounters(counters);                
            });
        },
        error: function (msg) {
        }
    });
}

function getTrieDataStates() {
    $.ajax({
        type: "POST",
        url: "http://localhost:56267/trie/TrieManager.asmx/GetDataStates",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var dataStates = JSON.parse(this);  
                displayTrieDataStates(dataStates);                
            });
        },
        error: function (msg) {
        }
    });
}

function getTrieDataCount() {
    $.ajax({
        type: "POST",
        url: "http://localhost:56267/trie/TrieManager.asmx/GetWikiDataCount",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var dataCount = JSON.parse(this);  
                displayTrieDataCount(dataCount);                
            });
        },
        error: function (msg) {
        }
    });
}

function getViewsDataCount() {
    $.ajax({
        type: "POST",
        url: "http://localhost:56267/trie/TrieManager.asmx/GetViewsDataCount",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var dataCount = JSON.parse(this);  
                displayViewsDataCount(dataCount);                
            });
        },
        error: function (msg) {
        }
    });
}

function startTrieStoring() {   
    $.ajax({
        type: "POST",        
        url: "http://localhost:56267/trie/TrieManager.asmx/StartupTrieStructure",
        contentType: "application/json; charset=utf-8",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                M.toast({ html: this, displayLength: 1000, classes: 'blue-text white pulse' });
            });
        },
        error: function (msg) {
        }
    });
}

function configurePopularityCount() {   
    $.ajax({
        type: "POST",        
        url: "http://localhost:56267/trie/TrieManager.asmx/ConfigurePopularityCount",
        contentType: "application/json; charset=utf-8",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                M.toast({ html: this, displayLength: 1000, classes: 'blue-text white pulse' });
            });
        },
        error: function (msg) {
        }
    });
}

function getRecentlySearchedData() {
    $.ajax({
        type: "POST",
        url: "http://localhost:56267/trie/TrieManager.asmx/GetRecentlySearchedData",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var keywords = JSON.parse(this);                
                displayRecentlySearchedData(keywords);
            });
        },
        error: function (msg) {
        }
    });
}

function displayRecentlySearchedData(entries){
    var listedData = "";
    if (entries.length > 0){        
        entries.forEach(function (entry) {   
            listedData += "<li class='collection-item'>" + entry.Item1 + " - " + entry.Item2 + "</li>";            
        });
    }
    else{
        listedData = "<li class='collection-item'>No Data Has Been Searched</li>"
    }      

    $("#recentlysearchdata").html(listedData);
}

function displayTrieMachineCounters(counters){
    $("#trieram").text(counters[0]);
    $("#triecpu").text(counters[1]);
    $("#triecpubar").css("width", counters[1] + "%");
}

function displayTrieDataStates(dataStates){
    $("#triedatastate").text(dataStates[0]);
    $("#popcountdatastate").text(dataStates[1]);
}

function displayTrieDataCount(dataCount){
    $("#triecollecteddata").text(dataCount[0]);    
    $("#triestoreddata").text(dataCount[1]);
    $("#triestoreddatabar").css("width", Number(dataCount[1])/Number(dataCount[0])*100 + "%");
}

function displayViewsDataCount(dataCount){
    $("#popcountlist").text(dataCount[0]);    
    $("#popcountdone").text(dataCount[1]);
}