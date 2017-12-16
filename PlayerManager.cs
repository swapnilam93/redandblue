using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	
	/// <summary>
	/// Main Player manager class.
	/// We check all player collisions here.
	/// We also calculate the score in this class. 
	/// </summary>
	
	public static int playerScore;			//player score
	public GameObject scoreTextDynamic;		//gameobject which shows the score on screen
	public GameObject blueblast;			//animation to show a blue blast
	public GameObject redblast;				//animation to show a red blast

	private int timeDiff;

	void Awake() {
		playerScore = 0;		
		timeDiff = 0;
		//Disable screen dimming on mobile devices
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		//Playe the game with a fixed framerate in all platforms
		Application.targetFrameRate = 60;
	}
	
	
	void Update() {
		if (!GameController.gameOver)
			calculateScore ();
	}

	///***********************************************************************
	/// calculate player's score
	/// Score is a combination of gameplay duration (while player is still alive) 
	/// and a multiplier for the current level.
	///***********************************************************************
	void calculateScore() {
		if(!PauseManager.isPaused) {
			timeDiff++;
			if(timeDiff == 10){
				timeDiff = 0;
				playerScore += (int)( GameController.currentLevel * Mathf.Log(GameController.currentLevel + 1, 2) );
			}
			scoreTextDynamic.GetComponent<TextMesh>().text = playerScore.ToString();
		}
	}


	///***********************************************************************
	/// Collision detection and management
	///***********************************************************************
	void OnCollisionEnter(Collision c) {

		//collision with mazes and enemyballs leads to a sudden gameover

		if(c.gameObject.tag == "Maze") {
			print ("Game Over");
			StartCoroutine(waitGameOver());
		}

		if(c.gameObject.tag == "enemyBall") {
			Instantiate(blueblast, transform.position, transform.rotation);
			Destroy(c.gameObject);
		}
	}
	
	void playSfx(AudioClip _sfx) {
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}

	//time to wait before showing the game over screen
	IEnumerator waitGameOver(){
		if(!GameController.gameOver){
			Instantiate(redblast, transform.position, transform.rotation);
			yield return new WaitForSeconds(0.2f);
			GameController.gameOver = true;
			Destroy(gameObject);
		}
	}
	
}
