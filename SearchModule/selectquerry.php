<?php

class BasketballStatsQuerry{
    // generic function to retrieve all values from a specified table
    public static function selectAll($connection, $tableName){
        
        $stmt = $connection->prepare("SELECT * FROM $tableName;");
        $stmt->execute();
        // fetches data from the executed querry
        $resultArray = $stmt->fetchAll();
        // return the retrieved data
        return $resultArray;        
    }

    public static function selectAllWithCondition($connection, $columnNamesString, $searchedString){
    	$stmt = $connection->prepare("SELECT $columnNamesString FROM PlayersStatsTable INNER JOIN PlayersMediaTable ON PlayersStatsTable.id = PlayersMediaTable.id WHERE CONCAT(FirstName, ' ', LastName) LIKE '%$searchedString%' OR CONCAT(LastName, ' ', FirstName) LIKE '%$searchedString%';");
    	$stmt->execute();
        // fetches data from the executed querry
        $resultArray = $stmt->fetchAll();
        // return the retrieved data

        // if no result through accurate searching, search by misspelled names instead
        if (!$resultArray){
            $stmt = $connection->prepare("SELECT $columnNamesString FROM PlayersStatsTable INNER JOIN PlayersMediaTable ON PlayersStatsTable.id = PlayersMediaTable.id  WHERE levenshtein(LastName, '$searchedString') < 3 OR levenshtein(FirstName, '$searchedString') < 3 OR levenshtein(CONCAT(LastName, ' ', FirstName), '$searchedString') < 4 OR  levenshtein(CONCAT(FirstName, ' ', LastName), '$searchedString') < 4;");
            $stmt->execute();
            $resultArray = $stmt->fetchAll();
        }
        return $resultArray;        
    }
}