function insertResults(name){
	document.getElementById("output").innerHTML = name;
}

// AJAX function for search bar
function startServerConnection(searchedName){
  document.getElementById("output").innerHTML = "<h3 align='center' class='text-light'>Searching Results for "
   + searchedName + "...</h3>";
	if (searchedName.length == 0) { 
      document.getElementById("output").innerHTML = "";
      return;
  	} else {
      var xmlhttp = new XMLHttpRequest();
      xmlhttp.onreadystatechange = function() {
        if (this.readyState == 4 && this.status == 200) {
       	  //inserts results based on the text that main.php echoed
          insertResults(this.responseText);
          twttr.widgets.load();
        }
    }
    // passes name to be querried to main php to be executed
    xmlhttp.open("GET", "main.php?playername=" + searchedName, true);
    xmlhttp.send();
  }
}

function alertWindow(message){
  window.alert(message);
}

function setTwitterRefLink(refLink){
  document.getElementById('twitterModalRef').href = refLink; 
}