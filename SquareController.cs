using UnityEngine;
using System.Collections;

public class SquareController : MonoBehaviour {

	public GameObject blueBlast;			//animation to show a blue blast
	public GameObject redBlast;				//animation to show a red blast

	private float speed;							//movement speed (the faster, the harder)
	private float destroyThreshold = -7f;			//if position is passed this value, the game is over.

	// Use this for initialization
	void Start () {
		//set a random speed for each enemyball
		speed = Random.Range(0.6f, 2.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (!PauseManager.isPaused) {
			StartCoroutine (tapManager ());
			//move the square down
			transform.position -= new Vector3(0, 0, Time.deltaTime * 
			                                  GameController.moveSpeed * 
			                                  speed);

			if (transform.position.z < destroyThreshold) {
				//game over if blue square is not hit in time
				if (this.gameObject.name == "blueSquare") {
					//check for possible gameover
					StartCoroutine (waitGameOver ());
				} else if (this.gameObject.name == "redSquare") {
					Destroy(this.gameObject);
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

		//destroy blue square on touch and game over on touching red square
		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			switch(objectHit.name) {
			case "blueSquare":
				Instantiate(blueBlast, hitInfo.transform.position, hitInfo.transform.rotation);
				Destroy(objectHit);
				break;
				
			case "redSquare":
				Instantiate(redBlast, hitInfo.transform.position, hitInfo.transform.rotation);
				yield return new WaitForSeconds(0.2f);
				GameController.gameOver = true;
				break;
			}	
		}
	}

	//time to wait before showing the game over screen
	IEnumerator waitGameOver(){
		if (!GameController.gameOver) {
			Instantiate (redBlast, new Vector3 (transform.position.x, transform.position.y, destroyThreshold + 1f), transform.rotation);
			yield return new WaitForSeconds (0.2f);
			GameController.gameOver = true;
		}
	}

}
