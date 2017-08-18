using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_Script : MonoBehaviour {
	//UI object
	private GameObject menuUI;
	private GameObject playUI;

	//Head dimension
	private GameObject head;
	private Sprite headSprite;
	private Vector2 headSize;
	private Vector2 headNextPosition;
	private Vector3 headRotate;

	//tail
	public GameObject tailPrefab;
	private List<Transform> tailTransform;
	public int tailInitialCount;

	//tail spawn (Start spawn and digest spawn)
	private Vector2 tailSpawnPosition;
	private GameObject tailSpawnObject;


	//digest
	public GameObject digestPrefab;
	private Queue<GameObject> digestObject;
	private GameObject tempDigestObject;


	//speed and delay
	public float snakeSpeed;
	private float lastMoveTime;

	private bool isPlaying;

	private Vector2 direction;
	private Vector2 tempDirection; //to Avoid multiple direction change on delay


	// Use this for initialization
	void Start () {
		//UI
		menuUI = GameObject.Find ("MainMenu");
		playUI = GameObject.Find ("GamePlay");
		playUI.SetActive (false);

		//Head dimension
		head = GameObject.Find ("Head");
		headSprite = head.GetComponent<SpriteRenderer> ().sprite;

		//sprite width = bound x 2 x scale
		headSize = new Vector2 (headSprite.bounds.extents.x * 2 * head.transform.localScale.x, headSprite.bounds.extents.y * 2 * head.transform.localScale.y);

		headNextPosition = new Vector2 (0, 0); // initialize only
		headRotate = new Vector3(0,0,0); //initialize only

		lastMoveTime = 0f; //initialize only

		isPlaying = false;
		direction = Vector2.up;
		tempDirection = Vector2.up;

		//tail initial spawn
		tailTransform = new List<Transform> ();
		tailSpawnPosition = new Vector2(head.transform.position.x,head.transform.position.y - headSize.y);
		for (int i = 0; i < tailInitialCount; i++) {
			tailSpawnObject = (GameObject)Instantiate (tailPrefab, tailSpawnPosition, Quaternion.identity);
			tailTransform.Add (tailSpawnObject.transform);
			tailSpawnPosition.y = tailSpawnPosition.y - headSize.y;
		}

		//digest
		digestObject = new Queue<GameObject>();

	}
	
	// Update is called once per frame
	void Update () {

		//call every snakeSpeed seconds
		if (isPlaying) {
			if (Time.time >= lastMoveTime) {
				
				lastMoveTime = Time.time + snakeSpeed; //refresh rapid fire
				direction = tempDirection; //assign direction
				Digesting ();
				Move ();

			}

		}
	}

	private void Move(){
		//rotate head sprite
		head.transform.Rotate(headRotate);
		headRotate.z = 0;

		//move last tail to head position
		tailTransform[tailTransform.Count - 1].position = head.transform.position;
		tailTransform.Insert (0, tailTransform[tailTransform.Count - 1]);
		tailTransform.RemoveAt (tailTransform.Count - 1);

		//move head
		//new position = current position + width * direction value
		headNextPosition.x = head.transform.position.x + (direction.x * headSize.x);
		headNextPosition.y = head.transform.position.y + (direction.y * headSize.y);
		head.transform.position = headNextPosition;
	}

	public void eat(){
		digestObject.Enqueue((GameObject)Instantiate (digestPrefab, head.transform.position, Quaternion.identity));
	}

	private void Digesting(){
		if (digestObject.Count>0 &&
			Vector2.Distance(digestObject.Peek().transform.position,
				tailTransform[tailTransform.Count-1].position) <= 0.1f) {
			print ("Digested");
			tempDigestObject = digestObject.Dequeue ();


			//spawn tail
			tailSpawnPosition = tempDigestObject.transform.position;
			tailSpawnObject = (GameObject)Instantiate (tailPrefab, tailSpawnPosition, Quaternion.identity);
			tailTransform.Add (tailSpawnObject.transform);

			GameObject.Destroy (tempDigestObject);
		}
	}


	public void clickStart(){
		menuUI.SetActive (false);
		playUI.SetActive (true);
		isPlaying = true;
		lastMoveTime = Time.time + snakeSpeed;
	}

	public void clickLeftControl(){
		headRotate.z = 90;

		if (direction == Vector2.up) {
			tempDirection = Vector2.left;
		} else if (direction == Vector2.left) {
			tempDirection  = Vector2.down;
		} else if (direction == Vector2.down) {
			tempDirection  = Vector2.right;
		} else if (direction == Vector2.right) {
			tempDirection  = Vector2.up;
		}
	}

	public void clickRightControl(){
		headRotate.z = -90;

		if (direction == Vector2.up) {
			tempDirection  = Vector2.right;
		} else if (direction == Vector2.right) {
			tempDirection  = Vector2.down;
		} else if (direction == Vector2.down) {
			tempDirection  = Vector2.left;
		} else if (direction == Vector2.left) {
			tempDirection  = Vector2.up;
		}
	}
}
