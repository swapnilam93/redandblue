using UnityEngine;
using System.Collections;

public class GlobalObjectMover : MonoBehaviour {

	/// <summary>
	/// This class moves the mazes down and destroys them when they pass a certain threshold
	/// </summary>

	[Range(1.5f, 2.5f)]
	public float speed = 2.0f;					//movement speed
	private float destroyThreshold = -10.0f;	//destory passed mazes to free up the memory


	void Update() {
		//Scroll down the maze objects
		transform.position -= new Vector3(0, 0, Time.deltaTime * GameController.moveSpeed * speed);
		//Destroy it if it's out of screen view
		if (transform.position.z < destroyThreshold) 
			Destroy(gameObject);
	}
}
