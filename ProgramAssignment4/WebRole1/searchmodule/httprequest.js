// JavaScript File Exclusively for fetching crawled urls from Azure Table Storage

var searchResultsAJAX;
// Retrieves data from table storage for urls
function getSearchResults() {

    if(searchResultsAJAX && searchResultsAJAX.readyState != 4){
        searchResultsAJAX.abort();
    }

    var textValue = $("#textinput").val();
    dataString = { "keyword": textValue };
    searchResultsAJAX = $.ajax({
        type: "POST",
        data: JSON.stringify(dataString),
        url: "http://localhost:56267/AzureStorageManager.asmx/RetrieveSearchResults",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {            
            var data = eval(msg);
            jQuery.each(data, function (rec) {
                var htmls = JSON.parse(this);
                displaySearchHeaderMessage(htmls.length); 
                displaySearchResults(htmls);                
            });
        },
        error: function (msg) {
        }
    });
}

// gets body content of a url from blob storage
function getBlobFileContents(blobFileName, x){
    dataString = { "blockBlobReference": blobFileName };
    $.ajax({
        type: "POST",
        data: JSON.stringify(dataString),
        url: "http://localhost:56267/AzureStorageManager.asmx/GetContentFromBlobFile",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);            
            jQuery.each(data, function (rec) {
                $("#bodyblobcontent" + x).html(boldLettersInContent(this));                         
            });
        },
        error: function (msg) {
            $("#bodyblobcontent" + x).text("No available text");  
        }
    });
}

// Indicates if the number of results returned if there are any, or displays 'no results found' when web service returns 0 results
function displaySearchHeaderMessage(arrayCount){
    if (arrayCount > 0){
    $("#searchheadertext").text("Search Results : " + arrayCount + " matches");
    }
    else{
        $("#searchheadertext").text("No Results Found");
    }
    if ($("#textinput").val() == ""){
        $("#searchheader").fadeOut('100');
    }    
    else{
        $("#searchheader").fadeIn('100');
    }
}

// Displays Urls and their body content
function displaySearchResults(htmlArray){
    var content = "";
    var index = 0;
    htmlArray.forEach(function (htmlData) {
        urlForMethod = "'" + htmlData.RowKey +"'";
        content += '<div class="col-12 my-4"><div class="card resultcard"><div class="card-header"><a href="#" class="text-success" data-toggle="collapse" data-target="#resultbody' + index + '"><strong>' + htmlData.Title + '</strong> <small>(Popularity Count: ' + htmlData.PopularityCount + ')</small></a></div><div class="card-body resultbody collapse" id="resultbody' + index + '">' + toJavaScriptDate(htmlData.PublishDate) + '<br><span id="bodyblobcontent' + index + '"></span></div><div class="card-footer"><a onclick="incrementPopularityOfWebSite(' + urlForMethod  + ')" href=' + htmlData.RowKey + ' target="_blank">' + htmlData.RowKey + '</a></div></div></div>';
        getBlobFileContents(htmlData.BlobFileForBody, index);
        index++;                          
    }); 
    $("#searchresultcontainer").html(content);
}

// Converts DateTime variable to a human readable String format
function toJavaScriptDate(value) {
    if (value != null){
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
    }
    else{
        return "No Avaiable Date."
    }    
}

// if a word in body content of url matches exactly the typed word in the search text, sets it to bold font
function boldLettersInContent(blobFileContent){
    var textValue = $("#textinput").val();
    var words = textValue.split(" ");    
    words.forEach(function (word) {        
        blobFileContent = blobFileContent.replace(new RegExp(word, "g"), "<b>" + word + "</b>"); 
    });

    return blobFileContent;
}

// increments popularity of the page when the url link is clicked
function incrementPopularityOfWebSite(url){    
    $.ajax({
        type: "POST",
        data: "url=" + url,        
        url: "http://localhost:56267/AzureStorageManager.asmx/IncrementPopularityCount",        
        success: function (msg) {
            getSearchResults();
        },
        error: function (msg) {   
        }
    });
}