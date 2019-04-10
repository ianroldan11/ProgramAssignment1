// JavaScript File used exclusively for NBA Players stats

var twitterscript;

// Retrieves data from a php file using cross domain
function retrieveJSONPResult(data) {
    if (data.result > 0) {       
        $("#playerimage").attr('src', 'http://a.espncdn.com/combiner/i?img=/i/headshots/nba/players/full/' + data.espnID + '.png&w=350&h=254');
        $("#playerportrait").css('background-image', "url('https://a.espncdn.com/i/teamlogos/nba/500/" + data.Team + ".png'), linear-gradient(to bottom right, #424242, #616161, #757575)");
        $("#playername").text(data.LastName + ", " + data.FirstName);

        $("#playermin").text(data.Min);
        $("#playergp").text(data.GP);
        $("#playerteam").text(data.Team);

        $("#playermfg").text(data.MFG);
        $("#playerafg").text(data.AFG);
        $("#playerpctfg").text(data.PctFG);

        $("#playerm3pt").text(data.M3PT);
        $("#playera3pt").text(data.A3PT);
        $("#playerpct3pt").text(data.Pct3PT);

        $("#playermft").text(data.MFT);
        $("#playeraft").text(data.AFT);
        $("#playerpctft").text(data.PctFT);

        $("#playeroffrebounds").text(data.OffRebounds);
        $("#playerdefrebounds").text(data.DefRebounds);
        $("#playertotrebounds").text(data.TotRebounds);

        $("#playerast").text(data.Ast);
        $("#playerto").text(data.TO);
        $("#playerstl").text(data.Stl);

        $("#playerpf").text(data.PF);
        $("#playerblk").text(data.Blk);
        $("#playerppg").text(data.PPG);

        $("#playerpicture1").attr('src', data.picture1);
        $("#playerpicture2").attr('src', data.picture2);

        $("#playervideo1").attr('src', 'https://www.youtube.com/embed/' + data.youtube1);
        $("#playervideo2").attr('src', 'https://www.youtube.com/embed/' + data.youtube2); 

        $("#twittercontainer").html('<a id="twitterpanel" class="twitter-timeline" data-height="640" data-theme="light" href="https://twitter.com/' + data.twitter +'?ref_src=twsrc%5Etfw">Tweets</a>');
        twttr.widgets.load();

        $("#imagespanel").fadeIn();
        $("#videospanel").fadeIn();
        $("#twittercontainer").fadeIn();
        $("#playercontainer").fadeIn();        
    }
    else {
        // dont show portrait panel
        $("#imagespanel").fadeOut();
        $("#videospanel").fadeOut();
        $("#twittercontainer").fadeOut();
        $("#twittercontainer").html("");
        $("#playercontainer").fadeOut();        
    }
    
}

var s;
// creates a script element that calls the php script that fetches player's stats every keyupevent on text field
function keyUpEventListener() {
    $("#customscript").remove();
    s = document.createElement("script");
    s.id = "customscript";
    s.src = "http://3.0.146.127/SearchModule/SearchModule/statsfetch.php?playername=" + $("#textinput").val();
    document.body.appendChild(s);
    getQuerySuggestion();
    getSearchResults();
    displayRecentQuerry();
} 