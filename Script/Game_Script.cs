using UnityEngine;
using System.Collections;

public class Game_Script : MonoBehaviour {
	//UI object
	private GameObject menuUI;
	private GameObject playUI;

	//Head dimension
	private GameObject head;
	private Sprite headSprite;
	private Vector2 headSize;
	private Vector2 headNextPosition;

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

		lastMoveTime = 0f; //initialize only

		isPlaying = false;
		direction = Vector2.up;
		tempDirection = Vector2.up;
	}
	
	// Update is called once per frame
	void Update () {
		if (isPlaying) {

			if (Time.time >= lastMoveTime) {
				lastMoveTime = Time.time + snakeSpeed; //refresh rapid fire
				direction = tempDirection; //assign direction
				Move ();
			}

		}
	}

	private void Move(){
		//new position = current position + width * direction value
		headNextPosition.x = head.transform.position.x + (direction.x * headSize.x);
		headNextPosition.y = head.transform.position.y + (direction.y * headSize.y);
		head.transform.position = headNextPosition;
	}


	public void clickStart(){
		menuUI.SetActive (false);
		playUI.SetActive (true);
		isPlaying = true;
		lastMoveTime = Time.time + snakeSpeed;
	}

	public void clickLeftControl(){
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
