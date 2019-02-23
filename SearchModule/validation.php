<?php

$temporaryGuide = "";
$nameOfSearchedPlayer = "";

if ($_SERVER["REQUEST_METHOD"] == "GET"){
  if (empty($_GET["playername"])) {
    $temporaryGuide = "* Start typing now! page will load results as you type.";
  } else{
  	$temporaryGuide = "";
    $nameOfSearchedPlayer = testInput($_GET["playername"]);
  }
}

function testInput($data) {
  $data = trim($data);
  $data = stripslashes($data);
  $data = htmlspecialchars($data);
  $data = str_replace("'","''",$data);
  $data = str_replace("\\","",$data);
  return $data;
}
?>