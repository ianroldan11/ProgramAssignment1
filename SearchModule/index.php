<!DOCTYPE html>
<html lang="en-US">
  <head>
    <!-- Javascript for AJAX -->
    <script src="searchmodule.js"></script>
    <meta charset="UTF-8">
    <!-- Dynamic Scaling -->
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <!-- Bootstrap CDN -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.2.1/css/bootstrap.min.css">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.6/umd/popper.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.2.1/js/bootstrap.min.js"></script>
    <!-- Custom CSS -->
    <link rel="stylesheet" type="text/css" href="searchmodule.css">
    <link rel="stylesheet" type="text/css" href="parallax.css">
    <!-- Title -->
    <title>NBA Players Stats Viewer</title>
  </head>

  <body>
    <!--  -->

    <!-- Container for NBA logo -->
    <div class="thincontainer my-5"><img id="nbalogo" src="images/nbalogo.png" alt="NBA Logo"></div>
    <!-- Heading -->
    <div class="thincontainer mt-5 ">
      <h1 class="display-4 text-light">NBA Player Searcher</h1>
    </div>
    <!-- Subheading -->
    <div class="thincontainer">
      <h2 class="text-white-50">
        <small>A comprehensive statistics viewer of all 2015-2016 NBA players</small>
      </h2>
    </div>
    <!-- Spacing -->
    <div class="container whitespace"></div>
    <!-- Form group for search bar -->
    <div class="container">
      <form class="form-group">
        <label for="searchbar" class="text-white-50"><strong>Search for a player by typing his name below:</strong></label>
        <input type="text" class="form-control" onkeyup="startServerConnection(this.value)" id="searchbar" name="playername" placeholder="e.g. Stephen Curry, James, Irving, durant, etc.">
      </form>
      <div class="searchguide"><p class="text-info">* Start typing now! page will load results as you type 4 or more characters.</p></div>
    </div>
    <!-- Spacing -->
    <div class="container whitespace"></div>

    <!-- Search Results -->
    <div id="output"></div>

  </body>
</html>