using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	/// <summary>
	/// Player Controller class
	/// This class handles player movement. 
	/// It always follows player's input (Mouse or Touch)
	/// </summary>

	[Range(0, 0.5f)]
	public float followSpeedDelay = 0.1f;		//we want the ball to follow player's input with a small delay.
												//delay of 0 leads to immediate follow and delay of 0.5 leads to a lazy follow
	
	private int fingerOffset = 100;				//Distance between ball and user's input (Mouse or Touch)
												//this is required to let player see the ball above the finger
	
	//Private internal variables
	private float xVelocity = 0.0f;
	private float zVelocity = 0.0f;
	private Vector3 screenToWorldVector;

	
	void Start() {
		//Set Y offset for ball
		transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
	}
	
	
	void Update() {
		if(!GameController.gameOver) {

			//let the player to move the ball
			touchControl();
			
			//this is just for debug and play in EDITOR and SHOULD be commented at final build
			//this can also be used to override control type for WebPlayer and Standalone...
			//if (Application.isEditor || Application.isWebPlayer) {
			//	screenToWorldVector = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y + fingerOffset, 10));
			//	float editorX = Mathf.SmoothDamp(transform.position.x, screenToWorldVector.x, ref xVelocity, followSpeedDelay);
			//	float editorZ = Mathf.SmoothDamp(transform.position.z, screenToWorldVector.z, ref zVelocity, followSpeedDelay);
			//	transform.position = new Vector3(editorX, transform.position.y, editorZ);
			//}
			
			//set offset for ball
			transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
			
			//prevent ball from exiting the view (downside)
			if(transform.position.z < -5) 
				transform.position = new Vector3(transform.position.x, transform.position.y, -5.0f);
			
			//prevent ball from exiting the view (Upside)
			if(transform.position.z > 2.9f) 
				transform.position = new Vector3(transform.position.x, transform.position.y, 2.9f);
			
			//left/right movement limiters
			if(transform.position.x > 3.1f) 
				transform.position = new Vector3(3.1f, transform.position.y, transform.position.z);
			if(transform.position.x < -3.1) 
				transform.position = new Vector3(-3.1f, transform.position.y, transform.position.z);
		}
	}
	
	
	///***********************************************************************
	/// Control ball's position with touch position parameters
	///***********************************************************************
	void touchControl() {
		if (Input.touchCount > 0 || Application.isEditor || Application.isWebPlayer) {
			screenToWorldVector = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y + fingerOffset, 10));
			float touchX = Mathf.SmoothDamp(transform.position.x, screenToWorldVector.x, ref xVelocity, followSpeedDelay);
			float touchZ = Mathf.SmoothDamp(transform.position.z, screenToWorldVector.z, ref zVelocity, followSpeedDelay);
			transform.position = new Vector3(touchX, transform.position.y, touchZ);
		}
	}
	

	void playSfx(AudioClip _sfx) {
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}
}
