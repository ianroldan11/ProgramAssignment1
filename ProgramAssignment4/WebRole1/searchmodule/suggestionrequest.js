$(document).ready(function(){
  $("#textinput").focus(function(){
    $("#querysuggestionbox").fadeIn(200);     
  });
   $("#textinput").blur(function(){
    $("#querysuggestionbox").fadeOut(200);     
  });
});

var topSuggestion = null;

$(document).on('keypress',function(e) {
    var keyWord = $("#textinput").val();

    if(e.which == 13) {
        // finds exact match of what the user typed
        if (topSuggestion != null){
            searchProcedure(topSuggestion);            
        }                
    }
}); 

var querySuggestionAJAX;

function getQuerySuggestion(){    
    $("#suggestionbox").html("");
    if(querySuggestionAJAX && querySuggestionAJAX.readyState != 4){
        querySuggestionAJAX.abort();
        $("#suggestionloadingspinner").hide();
    }    
    var keyWord = $("#textinput").val();
    topSuggestion = null;
    if (keyWord.length > 0){
        $("#suggestionloadingspinner").show();
        dataString = { "searchWord": keyWord};
        querySuggestionAJAX = $.ajax({
            type: "POST",
            data: JSON.stringify(dataString),
            url: "http://localhost:56267/trie/TrieManager.asmx/GetSuggestionResults",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var data = eval(msg);            
                jQuery.each(data, function (rec) {
                    var words = JSON.parse(this);
                    topSuggestion = getTopSuggestion(words);                    
                    displayQuerySuggestion(words);
                    $("#suggestionloadingspinner").hide();
                });
            },
            error: function (msg) {            
            }
        });
    }
    else{
        $("#suggestionbox").html("");
    }
}

function displayQuerySuggestion(words){
    var querysuggestiontable ="";
    if (words.length > 0){
        // Write 'Try These Suggestions' in heading
        querysuggestiontable += "<thead><tr><th colspan='2'>Try these suggestions:</th></tr></thead>";
        // write <tbody>
        querysuggestiontable += "<tbody>"    
        words.forEach(function (word) {   
            querysuggestiontable+= "<tr onclick='tableRowClickListener(this)'><td>" + word.Item1 + "</td><td>PopularityCount: " + word.Item2 + "</td></tr>";            
        });
        // write </tbody>
        querysuggestiontable += "</tbody>"
        $("#suggestionbox").html(querysuggestiontable);
    }
    else{
        querysuggestiontable += "<thead><tr><th>No other suggestions available...</th></tr></thead>";
    }
    $("#suggestionbox").html(querysuggestiontable);
}

function displayRecentQuerry(){
    $("#recentsuggestionbox").html("");   
    var recentQuery = "";
    var wordList = "";

    if ($("#textinput").val() != ""){
        recentQuery += "<thead><tr><th>Your recent searches:</th></tr></thead><tbody>";
        for (var i = 0; i < localStorage.length; i++){
            if (localStorage.getItem(localStorage.key(i)).toLowerCase().startsWith($("#textinput").val().toLowerCase())){
                wordList += "<tr onclick='tableRowClickListener(this)'><td>" + localStorage.getItem(localStorage.key(i)) + "</td></tr>";     
            }  
        }
        if (wordList != ""){
            recentQuery += wordList;
        }
        else{
            recentQuery += "<tr><td>None Available</td></tr>"
        }
        recentQuery += "</tbody>";    
    }    
    $("#recentsuggestionbox").html(recentQuery);
}

function searchProcedure(searchedText){    
    $("#textinput").val(searchedText);
    addPopularityCount(searchedText);
    keyUpEventListener();
    getSearchResults();       
    getQuerySuggestion();
    addDataToRecentlySearchedTable(searchedText);
    showAlertOnSearch(searchedText);
    localStorage.setItem(searchedText, searchedText);
}

function tableRowClickListener(element){
    var searchedText = $(element).children('td').eq(0).text();
    searchProcedure(searchedText);
}

function addPopularityCount(keyWord){  
    $.ajax({
        type: "POST",
        data: "stringToTraverse=" + keyWord + "&isCaseSensitive=" + 1,        
        url: "http://localhost:56267/trie/TrieManager.asmx/AddPopularityCountToEntry",        
        success: function (msg) {
            
        },
        error: function (msg) {   
        }
    });
}

function getTopSuggestion(words){
    var wordToReturn;
    if (words.length > 0){
        words.forEach(function (word) {        
        if (word.Item1.toLowerCase() == $("#textinput").val().toLowerCase()){            
            wordToReturn = word.Item1;            
        }
        });

        if (wordToReturn == null){
            wordToReturn = words[0].Item1;
        }
    }   

    return wordToReturn;
}

function addDataToRecentlySearchedTable(keyWord){  
    $.ajax({
        type: "POST",
        data: "stringToTraverse=" + keyWord,        
        url: "http://localhost:56267/trie/TrieManager.asmx/AddDataToRecentlySearchedTable",        
        success: function (msg) {
            
        },
        error: function (msg) {   
        }
    });
}

function showAlertOnSearch(keyword){
    var alertMessage = "<div class='alert alert-success alert-dismissible'><button type='button' class='close' data-dismiss='alert'>&times;</button><strong>Success!</strong> Showing Results for " + keyword + ".</div>"
      $("#alertmessage").html(alertMessage);
}