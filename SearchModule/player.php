<?php
class Player{
	private $id;
	private $firstName;
	private $lastName;

	private $team;
	private $gp;
	private $min;

	private $mFG;
	private $aFG;
	private $pctFG;

	private $m3PT;
	private $a3PT;
	private $pct3PT;

	private $mFT;
	private $aFT;
	private $pctFT;

	private $offReb;
	private $defReb;
	private $totReb;

	private $ast;
	private $to;
	private $stl;
	private $blk;
	private $pf;
	private $ppg;

  private $espnID;
  private $backgroundPicture;
  private $twitterUrl;
  private $youtubeVideo1;
  private $youtubeVideo2;
  private $picture1;
  private $picture2;

	public function __construct($playerInfoArray){
		$this->id = $playerInfoArray['id'];
		$this->firstName = $playerInfoArray['FirstName'];
		$this->lastName = $playerInfoArray['LastName'];

		$this->team = $playerInfoArray['Team'];
		$this->gp = $playerInfoArray['GP'];
		$this->min = $playerInfoArray['Min'];

		$this->mFG = $playerInfoArray['MFG'];
		$this->aFG = $playerInfoArray['AFG'];
		$this->pctFG = $playerInfoArray['PctFG'];

		$this->m3PT = $playerInfoArray['M3PT'];
		$this->a3PT = $playerInfoArray['A3PT'];
		$this->pct3PT = $playerInfoArray['Pct3PT'];

		$this->mFT = $playerInfoArray['MFT'];
		$this->aFT = $playerInfoArray['AFT'];
		$this->pctFT = $playerInfoArray['PctFT'];

		$this->offReb = $playerInfoArray['OffRebounds'];
		$this->defReb = $playerInfoArray['DefRebounds'];
		$this->totReb = $playerInfoArray['TotRebounds'];

		$this->ast = $playerInfoArray['Ast'];
		$this->to = $playerInfoArray['TO'];
		$this->stl = $playerInfoArray['Stl'];

		$this->blk = $playerInfoArray['Blk'];
		$this->pf = $playerInfoArray['PF'];
		$this->ppg = $playerInfoArray['PPG'];

    $this->espnID = $playerInfoArray['espnID'];
    $this->backgroundPicture = $playerInfoArray['backgroundPicture'];
    $this->twitterUrl = $playerInfoArray['twitter'];
    $this->youtubeVideo1 = $playerInfoArray['youtube1'];
    $this->youtubeVideo2 = $playerInfoArray['youtube2'];
    $this->picture1 = $playerInfoArray['picture1'];
    $this->picture2 = $playerInfoArray['picture2'];
	}

	public function getID(){
		return $this->id;
	}
	public function getFirstName(){
		return $this->firstName;
	}
	public function getLastName(){
		return $this->lastName;
	}

	public function getTeam(){
		return $this->team;
	}
	public function getGP(){
		return $this->gp;
	}
	public function getMin(){
		return $this->min;
	}

	public function getMFG(){
		return $this->mFG;
	}
	public function getAFG(){
		return $this->aFG;
	}
	public function getPctFG(){
		return $this->pctFG;
	}

	public function getM3PT(){
		return $this->m3PT;
	}
	public function getA3PT(){
		return $this->a3PT;
	}
	public function getPct3PT(){
		return $this->pct3PT;
	}

	public function getMFT(){
		return $this->mFT;
	}
	public function getAFT(){
		return $this->aFT;
	}
	public function getPctFT(){
		return $this->pctFT;
	}

	public function getOffRebounds(){
		return $this->offReb;
	}
	public function getDefRebounds(){
		return $this->defReb;
	}
	public function getTotRebounds(){
		return $this->totReb;
	}

	public function getAst(){
		return $this->ast;
	}
	public function getTO(){
		return $this->to;
	}
	public function getStl(){
		return $this->stl;
	}

	public function getBlk(){
		return $this->blk;
	}
	public function getPF(){
		return $this->pf;
	}
	public function getPPG(){
		return $this->ppg;
	}

  public function getespnID(){
    return $this->espnID;
  }
  public function getBackgroundPicture(){
    return $this->backgroundPicture;
  }
  public function getTwitterUrl(){
    return $this->twitterUrl;
  }
  public function getYoutubeVideo1(){
    return $this->youtube1;
  }
  public function getYoutubeVideo2(){
    return $this->youtube2;
  }
  public function getPicture1(){
    return $this->picture1;
  }
  public function getPicture2(){
    return $this->picture2;
  }

  public function lowerCaseTeam(){
    return strtolower($this->team);
  }

  // echos html code for the results showing the stats of this player object
  public function printPictureAndLogo(){
    $teamLogoIdentifier =  $this->lowerCaseTeam();
    return "<!-- container for picture -->      
        <div class='container mb-3' style='margin: auto;'>
          <div class='row'>
            <!-- col for protrait -->
            <div class='col-sm-6'>
              <img title='http://a.espncdn.com/combiner/i?img=/i/headshots/nba/players/full/$this->espnID.png&w=350&h=254' src='http://a.espncdn.com/combiner/i?img=/i/headshots/nba/players/full/$this->espnID.png&w=350&h=254' class='img-fluid rounded-circle shadow-lg' alt='portrait' style='height: 100%; width: auto; object-fit: cover;'>
            </div>
            <!-- col for logo -->
            <div class='col-sm-6' >
              <img title='https://a.espncdn.com/i/teamlogos/nba/500/$teamLogoIdentifier.png' src='https://a.espncdn.com/i/teamlogos/nba/500/$teamLogoIdentifier.png' class='img-fluid shadow-lg' alt='logo' style='height: 100%; width: 100%'>
            </div>
          </div>
        </div>";
  }

  public function printDetails(){
    return "<!-- container for name -->
        <div class='container mb-3 shadowed border border-dark' style='text-align: center;'>
          <h5 id='lebron' class='display-4 text-white' style='margin: auto'><strong>$this->firstName $this->lastName</strong></h5>
        </div>

        <!-- container for top subdetails -->
        <div class='container mb-3' style='text-align: center;'>
          <div class='row'>
            <div class='col-sm-6 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-light'><small>GP: $this->gp</small></h3>
            </div>
            <div class='col-sm-6 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-light'><small>Min: $this->min</small></h3>
            </div>
          </div>
        </div>";

  }

  public function printFGAnd3PT(){
    return "<!-- container for top FG & 3PT -->
        <div class='container mb-3'>
          <div class='row'><h3 class='text-white'><strong>Field Goal Stats</strong></h3></div>
          <div class='row'>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-danger'><small>M: $this->mFG</small></h3>
            </div>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-info'><small>A: $this->aFG</small></h3>
            </div>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-primary'><small>Pct: $this->pctFG</small></h3>
            </div>
          </div>
        </div>

        <div class='container mb-3'>
          <div class='row'>
            <div class='container' style='text-align: right'>
            <h3 class='text-white'><strong>Three Point Stats</strong></h3>
            </div>
          </div>
          <div class='row'>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-primary'><small>M: $this->m3PT</small></h3>
            </div>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-danger'><small>A: $this->a3PT</small></h3>
            </div>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-info'><small>Pct: $this->pct3PT</small></h3>
            </div>
          </div>
        </div>";
  }

  public function printFTAndReb(){
    return "<!-- container for top FT & Reb -->
        <div class='container mb-3'>
          <div class='row'><h3 class='text-white'><strong>Free Throw Stats</strong></h3></div>
          <div class='row'>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-danger'><small>M: $this->mFT</small></h3>
            </div>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-info'><small>A: $this->aFT</small></h3>
            </div>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-primary'><small>Pct: $this->pctFT</small></h3>
            </div>
          </div>
        </div>

        <div class='container mb-3'>
          <div class='row'>
            <div class='container' style='text-align: right'>
            <h3 class='text-white'><strong>Rebounds</strong></h3>
            </div>
          </div>
          <div class='row'>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-primary'><small>Offensive: $this->offReb</small></h3>
            </div>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-danger'><small>Defensive: $this->defReb</small></h3>
            </div>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-info'><small>Total: $this->totReb</small></h3>
            </div>
          </div>
        </div>";
  }

  public function printOtherStats(){
    return "<!-- container for top Other Stats -->
        <div class='container mb-3'>
          <div class='row'><h3 class='text-white'><strong>Other Info</strong></h3></div>
          <div class='row'>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-danger'><small>Ast: $this->ast</small></h3>
            </div>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-info'><small>TO: $this->to</small></h3>
            </div>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-primary'><small>Stl: $this->stl</small></h3>
            </div>
          </div>
          <div class='row'>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-primary'><small>Blk: $this->blk</small></h3>
            </div>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-danger'><small>PF: $this->pf</small></h3>
            </div>
            <div class='col-sm-4 shadowed rounded border border-top-0 border-light' style=''>
              <h3 class='text-info'><small>PPG: $this->ppg</small></h3>
            </div>
          </div>
        </div>";
  }

  public function printCarouselForYoutubeVideos(){
    return "<!-- Start Carousel -->
                <div id='videoCarou$this->id' class='carousel slide' data-ride='carousel' data-interval='false'>                
                  
                  <!-- The slideshow -->
                  <div class='carousel-inner'>
                    <div class='carousel-item active'>

                      <div class='embed-responsive embed-responsive-16by9'>
                        <iframe class='embed-responsive-item' src='https://www.youtube.com/embed/$this->youtubeVideo1'></iframe>
                      </div>
                      
                    </div>
                    <div class='carousel-item'>

                      <div class='embed-responsive embed-responsive-16by9'>
                        <iframe class='embed-responsive-item' src='https://www.youtube.com/embed/$this->youtubeVideo2'></iframe>
                      </div>
                     
                    </div>                    
                  </div>
                  
                  <!-- Left and right controls -->
                  <a class='carousel-control-prev' href='#videoCarou$this->id' data-slide='prev'>
                    <span class='carousel-control-prev-icon'></span>
                  </a>
                  <a class='carousel-control-next' href='#videoCarou$this->id' data-slide='next'>
                    <span class='carousel-control-next-icon'></span>
                  </a>
                </div>
                <!-- End Carousel -->";
  }

  public function printTwitterWidget(){
    return "<a class='twitter-timeline' data-height='800' data-theme='dark' href='https://twitter.com/$this->twitterUrl?ref_src=twsrc%5Etfw'>Tweets by $this->lastName</a>";
  }

  public function printCarouselForPictures(){
    return "<!-- Start Carousel -->
                <div id='pictureCarou$this->id' class='carousel slide' data-ride='carousel'>                
                  
                  <!-- The slideshow -->
                  <div class='carousel-inner'>
                    <div class='carousel-item active'>

                      <img src='$this->picture1' alt='picture 1' width='1100' height='500'>
                      
                    </div>
                    <div class='carousel-item'>

                       <img src='$this->picture2' alt='picture 2' width='1100' height='500'>
                     
                    </div>                    
                  </div>
                  
                  <!-- Left and right controls -->
                  <a class='carousel-control-prev' href='#pictureCarou$this->id' data-slide='prev'>
                    <span class='carousel-control-prev-icon'></span>
                  </a>
                  <a class='carousel-control-next' href='#pictureCarou$this->id' data-slide='next'>
                    <span class='carousel-control-next-icon'></span>
                  </a>
                </div>
                <!-- End Carousel -->";
  }

  public function printCollapseForMediaObjects(){
    return "<!-- Start collapse -->
        <div id='accordion$this->id'>
          <div class='card'>
            <div class='card-header' style='background-color: black'>
              <a class='card-link' data-toggle='collapse' href='#collapseOne$this->id'>
                Youtube Videos of $this->lastName
              </a>
            </div>
            <div id='collapseOne$this->id' class='collapse show' data-parent='#accordion$this->id'>
              <div class='card-body'>
                {$this->printCarouselForYoutubeVideos()}
              </div>
            </div>
          </div>

          <div class='card'>
            <div class='card-header' style='background-color: black'>
              <a class='collapsed card-link' data-toggle='collapse' href='#collapseTwo$this->id'>
                Tweets about $this->lastName
              </a>
            </div>
            <div id='collapseTwo$this->id' class='collapse' data-parent='#accordion$this->id'>
              <div class='card-body'>
                {$this->printTwitterWidget()}                              
              </div>
            </div>
          </div>

          <div class='card'>
            <div class='card-header' style='background-color: black'>
              <a class='collapsed card-link' data-toggle='collapse' href='#collapseThree$this->id'>
                Pictures of $this->lastName
              </a>
            </div>
            <div id='collapseThree$this->id' class='collapse' data-parent='#accordion$this->id'>
              <div class='card-body'>
                {$this->printCarouselForPictures()}
              </div>
            </div>
          </div>
        </div>
        <!-- End Collapses -->";
  }

	public function echoDetails(){
		return "<!-- 1 thin container per player -->
    <div class='container-fluid parallax' style='background-image:url(\"images/backgrounds/$this->backgroundPicture\");'>
      <div class='thincontainer' style='height: 80%;'>

        {$this->printPictureAndLogo()}

        {$this->printDetails()}

        {$this->printFGAnd3PT()}

        {$this->printFTAndReb()}

        {$this->printOtherStats()}

        {$this->printCollapseForMediaObjects()}          

      </div>
    </div>
    <div class='container whitespace'></div>";
	}
}

?>