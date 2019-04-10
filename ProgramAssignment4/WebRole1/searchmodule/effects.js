// Used For Toggling Show/Hide Player's stats and tweets
var isTweetHidden = true;
var isStatHidden = true;

function toggleLinkName(element, name){
	var isHidden;	
	if (name == 'Stats'){
		isHidden = isStatHidden;
	}
	else if (name == 'Tweets') {
		isHidden = isTweetHidden;
	}	

	if (isHidden){
		$(element).text("Hide " + name + "...");
		isHidden = false;
	}
	else{
		$(element).text("Show " + name + "...");
		isHidden = true;
	}

	if (name == 'Stats'){
		isStatHidden = isHidden;
	}
	else if (name =='Tweets') {
		isTweetHidden = isHidden;
	}	
}

// Hides Every Panel On startup
$(document).ready(function(){

	$("#imagespanel").fadeOut(100);
    $("#videospanel").fadeOut(100);
    $("#twittercontainer").fadeOut(100);
    $("#playercontainer").fadeOut(100);
    $("#searchheader").fadeOut(100);

	twttr.ready(function (twttr) {
	    twttr.widgets.load();    
	});

}); 