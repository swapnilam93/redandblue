using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	///***********************************************************************
	/// Main GameController Class.
	/// It supports two different modes: "Escape" & "Survival"
	/// in escape mode, user must escape through various mazes.
	/// in survival mode, user must avoid enemyballs reaching the bottom of the screen.
	/// 
	/// This class clones the maze and enemyball objects in the game.
	/// It also manages the difficulty steep of the game, by increasing the movement speed of the elements.
	///***********************************************************************0

	static int gameMode = 0; 				//escape by default
											//index[0] = escape
											//index[1] = survival

	//Difficulty variables
	public static float moveSpeed; 			//Global speed of moving items (mazes)
	public static float cloneInterval; 		//clone maze and enenmyball every N seconds
	
	//leveling vars
	public static int currentLevel = 1;		//Start from easy settings (1 = very easy ---> 10 = very hard)	
	private float levelJump = 10.0f; 		//increase the level every N seconds

	private Vector3 startPoint;				//starting point of the clones object
	private float levelPassedTime;			//passed time since we started the game
	private float levelStartTime;			//time of starting the game
	
	//Gamevver state
	public static bool gameOver;			//Gameover plane object
	private bool gameOverFlag;				//Run the gameover sequence just once
	private float destroyInterval = 2f;		//Time before destroying harmful objects
	
	//AudioClips
	public AudioClip levelAdvanceSfx;
	public AudioClip gameoverSfx;
	
	//maze & enemyball creation flag
	private bool createMaze;				//can we clone a new maze?
	private bool createEnemyBall;			//can we clone a new enemyball?
	private bool createCircle;				//can we clone a new circle?
	private bool createRectangle;			//can we clone a new rectangle?
	private bool createSquare;				//can we clone a new square?

	//maze types
	public GameObject[] maze;				//Array of all available mazes
	public GameObject enemyBall;			//reference to the enemyball object
	public GameObject blueCircle;			//reference to the blueCircle object
	public GameObject redCircle;			//reference to the redCircle object
	public GameObject blueRectangle;		//reference to the blueRectangle object
	public GameObject redRectangle;			//reference to the redRectangle object
	public GameObject blueSquare;			//reference to the blueSquare object
	public GameObject redSquare;			//reference to the redSquare object
	
	//Game finish variables
	public GameObject gameOverPlane;		//reference to gameover plane (activates when we hit a maze)
	public GameObject mainBackground;		//reference to the main background object (to modify its color)
	public GameObject player;				//Reference to main ball object	
	
	
	///***********************************************************************
	/// Init everything here
	///***********************************************************************
	void Awake() {

		gameOverPlane.SetActive(false);				//hide the gameover plane
		mainBackground.GetComponent<Renderer>().material.color = new Color(1, 1, 1);	//set the background color to default
		gameMode = PlayerPrefs.GetInt("GameMode");	//check game mode

		createMaze = true;			//allow maze creation
		createEnemyBall = true;		//allow enemyball creation
		createCircle = true;		//allow circle creation
		createRectangle = true;		//allow rectangle creation
		createSquare = true;		//allow square creation

		currentLevel = 1;				
		levelPassedTime = 0;
		levelStartTime = 0;
		moveSpeed = 1.2f;
		cloneInterval = 1.0f;
		gameOver = false;
		gameOverFlag = false;
	}
	
	
	///***********************************************************************
	/// Main FSM
	///***********************************************************************
	void Update() {

		//If we have lost the set
		if(gameOver) {

			if(!gameOverFlag) {
				gameOverFlag = true;
				playSfx(gameoverSfx);
				//show gameover menu
				processGameover();
			}

			return;
		}

		//Escape or Survival modes ?
		if (gameMode == 0) {
			//if we are allowed to spawn a maze
			if (createMaze) 
				cloneMaze (); 
		} else if (gameMode == 1) {
			//if we are allowed to spawn enemyBall
			if (createEnemyBall) 
				cloneEnemyBall ();
		} else if (gameMode == 2) {
			//if we are allowed to spawn circle
			if (createCircle)
				cloneCircle ();
		} else if (gameMode == 3) {
			//if we are allowed to spawn rectangles
			if (createRectangle)
				cloneRectangle();
		} else if (gameMode == 4) {
			//if we are allowed to spawn rectangles
			if (createSquare)
				cloneSquare();
		}

		//if the game is not yet finished, make it harder and harder by increasing the objects movement speed
		modifyLevelDifficulty();
		
	}
	
	///***********************************************************************
	/// Clone Maze item based on a simple chance factor
	///***********************************************************************
	void cloneMaze() {
		createMaze = false;
		startPoint = new Vector3( Random.Range(-1.0f, 1.0f) , 0.5f, 7);
		Instantiate(maze[Random.Range(0, maze.Length)], startPoint, Quaternion.Euler( new Vector3(0, 0, 0)));	
		StartCoroutine(reactiveMazeCreation());
	}


	///***********************************************************************
	/// Clone a new enemyball object and let them have random sizes
	///***********************************************************************
	void cloneEnemyBall() {
		createEnemyBall = false;
		startPoint = new Vector3( Random.Range(-3.2f, 3.2f) , 0.5f, 7);
		GameObject eb = Instantiate(enemyBall, startPoint, Quaternion.Euler( new Vector3(0, 0, 0))) as GameObject;	
		eb.name = "enemyBall";
		float scaleModifier = Random.Range(-0.4f, 0.1f);
		eb.transform.localScale = new Vector3(eb.transform.localScale.x + scaleModifier,
		                                      eb.transform.localScale.y,
		                                      eb.transform.localScale.z + scaleModifier);
		StartCoroutine(reactiveEnemyBallCreation());
	}

	///***********************************************************************
	/// Clone a new circle object and let them have random sizes
	///***********************************************************************
	void cloneCircle(){
		createCircle = false;
		GameObject clone; 
		float size = Random.Range(0.5f, 1.5f);
		if(Random.value >= 0.4f){
			clone = Instantiate(blueCircle, new Vector3(Random.Range(-3f, 3f), 2f, Random.Range(5f, -5f)), Quaternion.Euler( new Vector3(90, 0, 0))) as GameObject;
			clone.transform.localScale = Vector3.one * size;
			clone.name = "blueCircle";
		}else{
			clone = Instantiate(redCircle, new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(5f, -5f)), Quaternion.Euler( new Vector3(90, 0, 0))) as GameObject;
			clone.transform.localScale = Vector3.one * size;
			clone.name = "redCircle";
		}
		if (clone.name == "redCircle")
			Destroy (clone, destroyInterval);
		StartCoroutine (reactiveCircleCreation ());
	}

	///***********************************************************************
	/// Clone a new grid of rectangles based on level
	///***********************************************************************
	void cloneRectangle(){
		createRectangle = false;
		GameObject clone;
		Vector3[] position = getRectangleGrid ();
		float size = getRectangleSize ();
		for(int i = 0; i < position.Length; i++){
			int slot = GameController.currentLevel % 2;
			float chance;
			if(slot == 0 || GameController.currentLevel >= 7)
				chance = 0.4f;
			else
				chance = 0.2f;
			if(Random.value >= chance){
				clone = Instantiate(redRectangle, position[i], Quaternion.Euler( new Vector3(90, 0, 0))) as GameObject;
				clone.transform.localScale = Vector3.one * size;
				clone.name = "redRectangle";
			}else{
				clone = Instantiate(blueRectangle, position[i], Quaternion.Euler( new Vector3(90, 0, 0))) as GameObject;
				clone.transform.localScale = Vector3.one * size;
				clone.name = "blueRectangle";
			}
			if (clone.name == "redRectangle")
				Destroy (clone, 2f);
		}
		StartCoroutine (reactiveRectangleCreation ());
	}

	///***********************************************************************
	/// Clone a new square object and let fall from random locations
	///***********************************************************************
	void cloneSquare() {
		createSquare = false;
		GameObject clone;
		if(Random.value >= 0.4f){
			clone = Instantiate(blueSquare, new Vector3( Random.Range(-3.2f, 3.2f) , 2f, 7), Quaternion.Euler( new Vector3(90, 0, 0))) as GameObject;
			clone.name = "blueSquare";
		}else{
			clone = Instantiate(redSquare, new Vector3( Random.Range(-3.2f, 3.2f) , 1f, 7), Quaternion.Euler( new Vector3(90, 0, 0))) as GameObject;
			clone.name = "redSquare";
		}
		StartCoroutine(reactiveSquareCreation());
	}
	
	//enable this controller to be able to clone maze objects again
	IEnumerator reactiveMazeCreation() {
		yield return new WaitForSeconds(cloneInterval);
		createMaze = true;
	}


	//enable this controller to be able to clone enemyball objects again
	IEnumerator reactiveEnemyBallCreation() {
		yield return new WaitForSeconds(0.35f);
		createEnemyBall = true;
	}

	//enable this controller to be able to clone circle objects again
	IEnumerator reactiveCircleCreation() {
		yield return new WaitForSeconds(cloneInterval);
		createCircle = true;
	}

	//enable this controller to be able to clone rectangle objects again
	IEnumerator reactiveRectangleCreation() {
		yield return new WaitForSeconds(2f);
		createRectangle = true;
	}

	//enable this controller to be able to clone square objects again
	IEnumerator reactiveSquareCreation() {
		yield return new WaitForSeconds(cloneInterval);
		createSquare = true;
	}
	
	///***********************************************************************
	/// Here can increase gameSpeed and decrease itemCloneInterval values to 
	/// make the game harder.
	///***********************************************************************
	void modifyLevelDifficulty() {

		levelPassedTime = Time.timeSinceLevelLoad;
		if(levelPassedTime > levelStartTime + levelJump) {

			//increase level difficulty (but limit it to a maximum level of 10)
			if(currentLevel < 10) {

				currentLevel += 1;

				//let the player know what happened to him/her
				playSfx(levelAdvanceSfx);

				//increase difficulty by increasing movement speed
				moveSpeed += 0.6f;

				//clone items faster
				cloneInterval -= 0.18f; //very important!!!
				print ("cloneInterval: " + cloneInterval);
				if(cloneInterval < 0.3f) cloneInterval = 0.3f;

				levelStartTime += levelJump;

				//Background color correction (fade to red)
				float colorCorrection = currentLevel / 10.0f;
				//print("colorCorrection: " + colorCorrection);
				mainBackground.GetComponent<Renderer>().material.color = new Color(1, 
								                                                   1 - colorCorrection, 
								                                                   1 - colorCorrection);
			}
		}
	}
	
	
	///***********************************************************************
	/// Game Over routine
	///***********************************************************************
	void processGameover() {
		gameOverPlane.SetActive(true);
	}


	///***********************************************************************
	/// Play audioclips
	///***********************************************************************
	void playSfx(AudioClip _sfx) {
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}	

	private Vector3[] position2 = new [] {new Vector3(-1.64f, 1f, 2.91f), new Vector3(1.64f, 1f, 2.91f), 
		new Vector3(-1.64f, 1f, -2.91f), new Vector3(1.64f, 1f, -2.91f)};
	private Vector3[] position3 = new [] {new Vector3(-2.19f, 1f, 3.89f), new Vector3(0, 1f, 3.89f), new Vector3(2.19f, 1f, 3.89f),
		new Vector3(-2.19f, 1f, 0), new Vector3(0, 1f, 0), new Vector3(2.19f, 1f, 0),
		new Vector3(-2.19f, 1f, -3.89f), new Vector3(0, 1f, -3.89f), new Vector3(2.19f, 1f, -3.89f)};
	private Vector3[] position4 = new [] {new Vector3(-2.46f, 1f, 4.38f), new Vector3(-.82f, 1f, 4.38f), new Vector3( .82f, 1f, 4.38f),	new Vector3(2.46f, 1f, 4.38f),
		new Vector3(-2.46f, 1f, 1.46f), new Vector3(-.82f, 1f, 1.46f), new Vector3(.82f, 1f, 1.46f), new Vector3(2.46f, 1f, 1.46f),
		new Vector3(-2.46f, 1f, -1.46f), new Vector3(-.82f, 1f, -1.46f), new Vector3(.82f, 1f, -1.46f), new Vector3(2.46f, 1f, -1.46f),
		new Vector3(-2.46f, 1f, -4.38f), new Vector3(-.82f, 1f, -4.38f), new Vector3( .82f, 1f, -4.38f), new Vector3(2.46f, 1f, -4.38f)};
	//Vector3[] position5 = new [] {new Vector3(-2.63f, 1f, 4.66f), new Vector3(-1.32f, 1f,  4.66f), new Vector3(0, 1f,   4.66f),	new Vector3(1.32f, 1f,  4.66f), new Vector3(2.63f, 1f,  4.66f), 
	//	new Vector3(-2.63f, 1f,  2.32f), new Vector3(-1.32f, 1f,  2.32f), new Vector3(0, 1f,   2.32f),	new Vector3(1.32f, 1f,  2.32f), new Vector3(2.63f, 1f,  2.32f), 
	//	new Vector3(-2.63f, 1f,  0), new Vector3(-1.32f, 1f,  0), new Vector3(0, 1f,   0),	new Vector3(1.32f, 1f,  0), new Vector3(2.63f, 1f,  0), 
	//	new Vector3(-2.63f, 1f,  -2.32f), new Vector3(-1.32f, 1f,  -2.32f), new Vector3(0, 1f,   -2.32f),	new Vector3(1.32f, 1f,  -2.32f), new Vector3(2.63f, 1f,  -2.32f), 
	//	new Vector3(-2.63f, 1f,  -4.66f), new Vector3(-1.32f, 1f,  -4.66f), new Vector3(0, 1f,   -4.66f),	new Vector3(1.32f, 1f,  -4.66f), new Vector3(2.63f, 1f,  -4.66f)};

	//other game play logics
	private Vector3[] getRectangleGrid(){
		int gridSize = (currentLevel - 1) / 2;
		switch (gridSize) {
		case 0: return position2;
		case 1: return position3;
		default: return position4;
		//default: return position5;
		}
	}

	private float getRectangleSize(){
		int gridSize = (currentLevel - 1) / 2;
		switch (gridSize) {
		case 0: return 2.43f;
		case 1: return 1.62f;
		default: return 1.215f;
		//default: return 0.972f;
		}
	}

}