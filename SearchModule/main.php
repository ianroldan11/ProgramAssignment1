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
	$result = BasketballStatsQuerry::selectAllWithCondition($connection, 'PlayersStatsTable', $nameOfSearchedPlayer);
	}

	if ($result){
		$playersArray = array();
		// create object model of players
		foreach ($result as $row){
			$player = new Player($row);
			array_push($playersArray, $player);
		}
		// echo result
		foreach ($playersArray as $player) {
			$resultName = $player->getLastName() . ' ' . $player->getFirstName();
			echo $player->echoDetails();
		}

	} else{
		echo "<div id='resultDiv' style='text-align: center'><h1 class='text-warning'>NO RESULTS FOR $nameOfSearchedPlayer</h1></div>";
	}
}

?>