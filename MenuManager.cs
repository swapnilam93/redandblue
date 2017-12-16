using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	/// <summary>
	/// Main Menu Controller.
	/// This class handles user clicks on menu button, and also fetch and shows user saved scores on screen.
	/// </summary>
	
	private int bestScore;				//best saved score
	private int lastScore;				//score of the last play
	private int playMusic;				//store music preference
	
	//reference to gameObjects
	public GameObject bestScoreText;	
	public GameObject lastScoreText; 
	public GameObject helpPlane;

	public AudioClip menuTap;			//sfx for touch on menu buttons
	
	private bool canTap;						//are we allowed to click on buttons? (prevents double touch)
	private float buttonAnimationSpeed = 9.0f;	//button scale animation speed
	private float waitForSeconds = 0.2f;		//wait for button animation
	private bool showHelpPlane = false;			//button to toggle help plane

	void Awake () {
		
		canTap = true; //player can tap on buttons
		
		bestScore = PlayerPrefs.GetInt("bestScore");
		bestScoreText.GetComponent<TextMesh>().text = bestScore.ToString();
		
		lastScore = PlayerPrefs.GetInt("lastScore");
		lastScoreText.GetComponent<TextMesh>().text = lastScore.ToString();

		playMusic = PlayerPrefs.GetInt ("playMusic", 1);
		GameObject btnMusic = GameObject.Find ("btnMusic");
		if(playMusic == 0){
			btnMusic.GetComponent<MeshRenderer>().material = Resources.Load("btnNoSound") as Material;
			AudioListener.volume = 0;
		}
		else{
			btnMusic.GetComponent<MeshRenderer>().material = Resources.Load("btnSound") as Material;
			AudioListener.volume = 1.0f;
		}

	}


	void Start() {
		//prevent screenDim in handheld devices
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}


	void Update () {
		if(canTap)	
			StartCoroutine(tapManager());
	}
	
	
	///***********************************************************************
	/// Process user inputs
	///***********************************************************************
	private RaycastHit hitInfo;
	private Ray ray;
	IEnumerator tapManager() {
		
		//Mouse of touch?
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonUp(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			yield break;
		
		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			switch(objectHit.name) {
				case "Btn-Mode-01":
					canTap = false;
					playSfx(menuTap);
					PlayerPrefs.SetInt("GameMode", 0);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(waitForSeconds);
					Application.LoadLevel("Game");
					break;

				case "Btn-Mode-02":
					canTap = false;
					playSfx(menuTap);
					PlayerPrefs.SetInt("GameMode", 1);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(waitForSeconds);
					Application.LoadLevel("Game");
					break;

				case "Btn-Mode-03":
					canTap = false;
					playSfx(menuTap);
					PlayerPrefs.SetInt("GameMode", 2);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(waitForSeconds);
					Application.LoadLevel("Circles");
					break;

				case "Btn-Mode-04":
					canTap = false;
					playSfx(menuTap);
					PlayerPrefs.SetInt("GameMode", 3);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(waitForSeconds);
					Application.LoadLevel("Rectangles");
					break;

				case "Btn-Mode-05":
					canTap = false;
					playSfx(menuTap);
					PlayerPrefs.SetInt("GameMode", 4);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(waitForSeconds);
					Application.LoadLevel("Squares");
					break;
					
				case "btnExit":
					canTap = false;
					playSfx(menuTap);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(waitForSeconds);
					Application.Quit();
					break;

				case "btnHelp":
					canTap = false;
					playSfx(menuTap);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(waitForSeconds);
					if(showHelpPlane)
						showHelpPlane = false;
					else
				   		showHelpPlane = true;
				   	helpPlane.SetActive(showHelpPlane);
					break;

				case "btnMusic":
					canTap = false;
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(waitForSeconds);
					if(playMusic == 0){
						AudioListener.volume = 1.0f;
						objectHit.GetComponent<MeshRenderer>().material = Resources.Load("btnSound") as Material;
						PlayerPrefs.SetInt("playMusic", 1);
						playMusic = 1;
					}
					else{
						AudioListener.volume = 0;
						objectHit.GetComponent<MeshRenderer>().material = Resources.Load("btnNoSound") as Material;
						PlayerPrefs.SetInt("playMusic", 0);
						playMusic = 0;
					}
					break;

			}	
		}
	}
	

	///***********************************************************************
	/// Animate button by modifying it's scales
	///***********************************************************************
	IEnumerator animateButton(GameObject _btn) {

		Vector3 startingScale = _btn.transform.localScale;
		Vector3 destinationScale = startingScale * 1.1f;
		float t = 0.0f; 
		while (t <= 1.0f) {
			t += Time.deltaTime * buttonAnimationSpeed;
			_btn.transform.localScale = new Vector3(Mathf.SmoothStep(startingScale.x, destinationScale.x, t),
			                                        _btn.transform.localScale.y,
			                                        Mathf.SmoothStep(startingScale.z, destinationScale.z, t));
			yield return 0;
		}
		
		float r = 0.0f; 
		if(_btn.transform.localScale.x >= destinationScale.x) {
			while (r <= 1.0f) {
				r += Time.deltaTime * buttonAnimationSpeed;
				_btn.transform.localScale = new Vector3(Mathf.SmoothStep(destinationScale.x, startingScale.x, r),
				                                        _btn.transform.localScale.y,
				                                        Mathf.SmoothStep(destinationScale.z, startingScale.z, r));
				yield return 0;
			}
		}
		
		if(r >= 1)
			canTap = true;
	}
	
	
	///***********************************************************************
	/// play audio clip
	///***********************************************************************
	void playSfx(AudioClip _sfx) {
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}
}
