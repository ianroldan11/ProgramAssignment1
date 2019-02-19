<?php
//Settings for SQL Connection
$servername = "ianroldandbidentifier.ctsiyx6wkl2q.us-east-2.rds.amazonaws.com";
$username = "dbianroldan";
$password = "dbianroldan";
$dbname = "dbbasketballstats";
//----------------------------

//Connection string to setup connection to server and its specified database
$connectionString = "mysql:host=$servername;dbname=$dbname";


class Connector{
    
    // static function that returns a PDO connection instance if successful or a boolean false flag if failed
    public static function pdoConnect(){
        
        // call the global variables to be used in this function
        global $connectionString, $username, $password;
        
        // always put connection to try-catch code block
        try {
            // instance for PDO connection
            $conn = new PDO($connectionString, $username, $password);
            // set the PDO error mode to exception
            $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
            // return PDO connection instance
            return $conn;
        }
        catch(PDOException $e)
        {
            // Print failure
            echo "Connection failed: " . $e->getMessage();
            // Return false
            return FALSE;
        }
    }
    
}
 
