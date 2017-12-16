using UnityEngine;
using System.Collections;

public class GameoverManager : MonoBehaviour {

	/// <summary>
	/// Gameover manager class. 
	/// 
	/// </summary>
	
	public GameObject scoreText;					//gameobject which shows the score on screen
	public GameObject bestScoreText;				//gameobject which shows the best saved score on screen

	public AudioClip menuTap;						//touch sound

	private bool canTap;							//prevents double click on buttons
	private float buttonAnimationSpeed = 9.0f;		//button scale animation speed
	

	void Awake () {
		canTap = true;
		//Set the new score on the screen
		scoreText.GetComponent<TextMesh>().text = PlayerManager.playerScore.ToString();
		bestScoreText.GetComponent<TextMesh>().text = PlayerPrefs.GetInt("bestScore").ToString();
	}


	void Update () {	
		if(canTap)
			StartCoroutine(tapManager());
	}
	
	
	///***********************************************************************
	/// Manage user taps on gameover buttons
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
				case "retryButton":
					playSfx(menuTap);									//play audioclip
					saveScore();										//save players best and last score
					StartCoroutine(animateButton(objectHit));			//animate the button
					yield return new WaitForSeconds(0.4f);				//Wait
					Application.LoadLevel(Application.loadedLevelName); //Load next level
					break;
				case "menuButton":
					playSfx(menuTap);
					saveScore();
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(1.0f);
					Application.LoadLevel("Menu");
					break;
				}	
		}
	}
	
	
	///***********************************************************************
	/// Save player score
	///***********************************************************************
	void saveScore() {
		//immediately save the last score
		PlayerPrefs.SetInt("lastScore", PlayerManager.playerScore);
		//check if this new score is higher than saved bestScore.
		//if so, save this new score into playerPrefs. otherwise keep the last bestScore intact.
		int lastBestScore;
		lastBestScore = PlayerPrefs.GetInt("bestScore");
		if(PlayerManager.playerScore > lastBestScore)
			PlayerPrefs.SetInt("bestScore", PlayerManager.playerScore);
	}
	
	
	///***********************************************************************
	/// Animate buttons on touch
	///***********************************************************************
	IEnumerator animateButton(GameObject _btn) {
		canTap = false;
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
	/// IPlay audioclip
	///***********************************************************************
	void playSfx(AudioClip _sfx) {
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}
}
