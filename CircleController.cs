using UnityEngine;
using System.Collections;

public class CircleController : MonoBehaviour {

	public GameObject blueBlast;		//animation to show a blue blast
	public GameObject redBlast;			//animation to show a red blast

	private float startTime;			//note start time to determine life of object

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (!PauseManager.isPaused) {
			StartCoroutine (tapManager ());
			//shrink the circles gradually
			if(this.transform.localScale.x > 0.5f){
				this.transform.localScale -= Vector3.one * 0.005f;
			}
			//game over if blue circle is not hit in time
			if (this.gameObject.name == "blueCircle") {
				if (Time.time - startTime > 2f) {
					if(!GameController.gameOver)
						StartCoroutine (waitGameOver ());
				}
			}
		}
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

		//destroy blue circles on touch and game over on touching red circles
		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			switch(objectHit.name) {
			case "blueCircle":
				Instantiate(blueBlast, hitInfo.transform.position, hitInfo.transform.rotation);
				Destroy(objectHit);
				break;
				
			case "redCircle":
				Instantiate(redBlast, hitInfo.transform.position, hitInfo.transform.rotation);
				yield return new WaitForSeconds(0.2f);
				GameController.gameOver = true;
				break;
			}	
		}
	}

	//time to wait before showing the game over screen
	IEnumerator waitGameOver(){
		Instantiate (blueBlast, transform.position, transform.rotation);
		yield return new WaitForSeconds(0.2f);
		GameController.gameOver = true;
	}

}
