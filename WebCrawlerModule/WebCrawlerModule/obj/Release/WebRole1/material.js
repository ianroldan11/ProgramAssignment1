// Materialize CSS Compononents Initializers--------------------------------------------------------------------
// SIDENAV
document.addEventListener('DOMContentLoaded', function () {
    var elems = document.querySelectorAll('.sidenav');
    var instances = M.Sidenav.init(elems, {});
});

// TOOLTIP
  document.addEventListener('DOMContentLoaded', function() {
      var elems = document.querySelectorAll('.tooltipped');
      var instances = M.Tooltip.init(elems, { position: 'left' });
});

// FIXED ACTION BUTTON
document.addEventListener('DOMContentLoaded', function () {
    var elems = document.querySelectorAll('.fixed-action-btn');
    var instances = M.FloatingActionButton.init(elems, {});
});

// CAROUSEL
document.addEventListener('DOMContentLoaded', function () {
    var elems = document.querySelectorAll('.carousel');
    var instances = M.Carousel.init(elems, {
        onCycleTo: function (slide) {            
            afterCarouselSelect(slide);
        }
    });
});

// MODAL
document.addEventListener('DOMContentLoaded', function () {
    var elems = document.querySelectorAll('.modal');
    var instances = M.Modal.init(elems, {});
});

// PARALLAX
document.addEventListener('DOMContentLoaded', function () {
    var elems = document.querySelectorAll('.parallax');
    var instances = M.Parallax.init(elems, {});
});
//------------------------------------------------------------------------------------------------------------

//Navigation-------------------------------------------------------------------------------------------------
// Opens side navigation bar - being used by menu button (top left)
function openSideNav() {
    var elem = document.querySelector('.sidenav');
    var instance = M.Sidenav.getInstance(elem);
    instance.open();
}

// Makes sure that all present components are hidden to prepare for transition
function hideAll() {
    $("#generaloverview").fadeOut();
    $("#generalsettings").fadeOut();
    $("#generallog").fadeOut();
    $("#webcrawlerexplorer").fadeOut();

    $("#webcrawlerexplorerheader").removeClass('scale-out');
    $("#webcrawlerexplorerheader").addClass('scale-out');
    $("#webcrawlerexplorerheader").fadeOut();
}

// Used when navigating to General Overview "page"
function showGeneralOverView() {
    $("#header").text("General Overview");
    $("#subheader").text("This part shows machine counters per crawlers, number of indexed data in table, size of each queues, and the general overview on the state of each crawlers.");
    hideAll();
    $("#generaloverview").fadeIn();
    M.toast({ html: 'Switched to General Settings', displayLength: 1000, classes: 'brown pulse' });
}

// Used when navigating to Web Crawler Explorer "page"
function showWebCrawlerExplorer() {    
    $("#header").text("Web Crawler Explorer");
    $("#subheader").text("Views and manages all of the occurences within the web crawlers as well as their individual threads.");
    hideAll();
    $("#webcrawlerexplorer").fadeIn();
    $("#webcrawlerexplorerheader").fadeIn();
    $("#webcrawlerexplorerheader").removeClass('scale-out');    
    M.toast({ html: 'Switched to Web Crawler Explorer', displayLength: 1000, classes: 'brown pulse' });
   
}

// Used when navigating to General Log "page"
function showGeneralLog() {
    $("#header").text("General Log");
    $("#subheader").text("This part shows logs regarding the activity of every thread from all domain crawlers.");    
    hideAll();
    $("#generallog").fadeIn();
    M.toast({ html: 'Switched to General Log', displayLength: 1000, classes: 'brown pulse' });
}

// Used when navigating to General Settings "page"
function showGeneralSettings() {
    $("#header").text("General Settings");
    $("#subheader").text("This part allows you to start or stop all crawlers. It also allows you to reset all functionality of all worker roles");
    hideAll();
    $("#generalsettings").fadeIn();
    M.toast({ html: 'Switched to General Settings', displayLength: 1000, classes: 'brown pulse' });
   
}
//------------------------------------------------------------------------------------------------------------

// General Settings animation effect--------------------------------------------------------------------------
// Hides all options except the one clicked
function hideOthers(elementClicked) {    

    $('#start').addClass('scale-out');
    $('#stop').addClass('scale-out');
    $('#reset').addClass('scale-out');

    $(elementClicked).parent().removeClass('scale-out');
}

// Shows all options after doing something from the option clicked
function showOthers(elementClicked) {
    $('#start').removeClass('scale-out');
    $('#stop').removeClass('scale-out');
    $('#reset').removeClass('scale-out');
}
//------------------------------------------------------------------------------------------------------------

//Web Crawler Explorer animation effect-----------------------------------------------------------------------
// function to perform everytime carousel is slided
function afterCarouselSelect(slide) {   
    if (flag > 0) {
        $(webcrawlerexplorer).fadeOut();
        $(webcrawlerexplorer).fadeIn();
    }
    identifySlideSelected(slide);        
}

var flag = 0;
var currentCarouselIndex = 0;

// Checks which slide has been selected and then perform functions depending on its content
function identifySlideSelected(slide) {
    var index = Number($(slide).index());
    $("#carousellabel").text();    

    switch (index) {
        case 0:
            M.toast({ html: 'Scanning: Bleacher Report Data', displayLength: 1000, classes: 'brown' });
            changeCarouselDisplay("Bleacher Report", "https://bleacherreport.com/", "https://officesnapshots.com/wp-content/uploads/2018/04/bleacher-report-offices-new-york-city-12-700x525.jpg");
            showReportLog("#bleacherreportlog", "#bleacherreportclear");
            flag = 1;
            break;
        case 1:
            M.toast({ html: 'Scanning: CNN Data', displayLength: 1000, classes: 'brown' });
            changeCarouselDisplay("CNN", "https://edition.cnn.com/", "https://thehill.com/sites/default/files/cnn_062717getty_lead.jpg");
            showReportLog("#cnnlog", "#cnnclear");
            break;
        case 2:
            M.toast({ html: 'Scanning: ESPN Data', displayLength: 1000, classes: 'brown' });
            changeCarouselDisplay("ESPN", "http://www.espn.com/", "https://www.newscaststudio.com/wp-content/uploads/2018/03/ncs_espn-sportsnation_0002.jpg");
            showReportLog("#espnlog", "#espnclear");
            break;
        case 3:
            M.toast({ html: 'Scanning: Forbes Data', displayLength: 1000, classes: 'brown' });
            changeCarouselDisplay("Forbes", "https://www.forbes.com/", "https://www.mediamavenandmore.com/wp-content/uploads/2019/01/how-to-write-for-forbes-1200x800.jpg"); 
            showReportLog("#forbeslog", "#forbesclear");
            break;
        case 4:
            M.toast({ html: 'Scanning: IMDB Data', displayLength: 1000, classes: 'brown' });
            changeCarouselDisplay("IMDB", "https://www.imdb.com/", "https://storage.googleapis.com/kaggle-datasets-images/1474/2639/cb50fd3b236d01184fd23ad59af50b4f/dataset-original.jpg");
            showReportLog("#imdblog", "#imdbclear");
            break;
    }

    currentCarouselIndex = index;
    getNumberOfUrlsCrawledForSpecificWebCrawler(index);
    getThreadStatusOfWebCrawler(index);
    getCurrentlyCrawledUrlsOfWebCrawler(index);
    getSwitchStatesOfWebCrawler(index);

    getCurrentlyCrawledUrls(index);
    
}
// changes banner image of web crawler explorer header
function changeCarouselDisplay(siteName, url, imageUrl) {
    $("#carousellabel").text(siteName);
    $("#carouselsublabel").text(url);
    if (flag > 0) {
        $("#carouselbanner").fadeOut(200);
    }
    $("#carouselbanner").attr("src", imageUrl);
    $("#carouselbanner").fadeIn(200);
}

// shows corresponding report log text area depending on the selected web crawler
function showReportLog(logToReveal, btnToReveal) {
    hideAllReportLogs();
    $(logToReveal).fadeIn();
    $(btnToReveal).fadeIn();
}

// hides all report logs text area to prepare for showing the appropriate text area
function hideAllReportLogs() {
    $("#bleacherreportlog").fadeOut();
    $("#cnnlog").fadeOut();
    $("#espnlog").fadeOut();
    $("#forbeslog").fadeOut();
    $("#imdblog").fadeOut();

    $("#bleacherreportclear").fadeOut();
    $("#cnnclear").fadeOut();
    $("#espnclear").fadeOut();
    $("#forbesclear").fadeOut();
    $("#imdbclear").fadeOut();
}

$(document).ready(function () {

    $("#dogswitch").change(function () {
        setSwitch("#dogswitch", $("#carousellabel").text(), "THR-DOG");
    });

    $("#catswitch").change(function () {
        setSwitch("#catswitch", $("#carousellabel").text(), "THR-CAT");
    });

    $("#horseswitch").change(function () {
        setSwitch("#horseswitch", $("#carousellabel").text(), "THR-HRS");
    });

    $("#rabbitswitch").change(function () {
        setSwitch("#rabbitswitch", $("#carousellabel").text(), "THR-RBT");
    });

});

//------------------------------------------FOR DEBUGGING ONLY----------------------------------------------------------------------
// sample only


// sample only
function getSwitch() {
    $('#dogswitch').prop('checked', true);
    $('#dogswitch').trigger('change');
    //alert($('#dogswitch').prop('checked'));
}
//---------------------------------------------------------------------------------------------------------------------------------------------


