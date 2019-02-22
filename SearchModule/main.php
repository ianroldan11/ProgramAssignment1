<?php 
// validate entry for security
include 'validation.php';
include 'pdoconnection.php';
include 'selectquerry.php';
include 'player.php';

$result;

// make pdo connection
if (strlen($nameOfSearchedPlayer) > 3){
	$connection = Connector::pdoConnect();

	if ($connection){
	global $result;
	// perform select querry
	$columnsToBeQuerried = "PlayersStatsTable.*, PlayersMediaTable.espnID, PlayersMediaTable.backgroundPicture, PlayersMediaTable.twitter, PlayersMediaTable.youtube1, PlayersMediaTable.youtube2, PlayersMediaTable.picture1, PlayersMediaTable.picture2";
	$result = BasketballStatsQuerry::selectAllWithCondition($connection, $columnsToBeQuerried, $nameOfSearchedPlayer);
	}

	if ($result){
		$playersArray = array();
		// create object model of players
		foreach ($result as $row){
			$player = new Player($row);
			array_push($playersArray, $player);
		}
		// echo result
		echo "<div style='text-align: center'><h4 class='text-info'>Showing results for <span class='text-white'>$nameOfSearchedPlayer</span>...</h4></div>";
		foreach ($playersArray as $player) {
			$resultName = $player->getLastName() . ' ' . $player->getFirstName();
			echo $player->echoDetails();
		}

	} else{
		echo "<div id='resultDiv' style='text-align: center'><h1 class='text-warning'>NO RESULTS FOR $nameOfSearchedPlayer</h1></div>";
	}
}

?>