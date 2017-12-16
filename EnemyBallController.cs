using UnityEngine;
using System.Collections;

public class EnemyBallController : MonoBehaviour {

	/// <summary>
	/// This class moves enemyballs in survival mode to the bottom of the screen.
	/// The game is over if any enemyball reach to the botttom.
	/// </summary>

	public GameObject redBlast;

	private float speed;							//movement speed (the faster, the harder)
	private float destroyThreshold = -6.5f;			//if position is passed this value, the game is over.
	
	void Start() {
		//set a random speed for each enemyball
		speed = Random.Range(0.6f, 2.0f);
	}

	void Update() {
		//move the enemyball down
		transform.position -= new Vector3(0, 0, Time.deltaTime * 
		                                 		GameController.moveSpeed * 
		                                 		speed);

		//check for possible gameover
		if (transform.position.z < destroyThreshold) {
			StartCoroutine (waitGameOver ());
		}
	}

	IEnumerator waitGameOver(){
		if(!GameController.gameOver){
			Instantiate(redBlast, new Vector3 (transform.position.x, transform.position.y, destroyThreshold), transform.rotation);
			yield return new WaitForSeconds(0.2f);
			GameController.gameOver = true;
			Destroy(gameObject);
		}
	}
}
