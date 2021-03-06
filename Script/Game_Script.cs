﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_Script : MonoBehaviour {
	//UI object
	private GameObject menuUI;
	private GameObject playUI;
	//moving field
	private GameObject movingField;
	private Vector2 movingFieldPosition;
	private float movingLine;
	private int distance;

	//Head
	private GameObject head;
	private Sprite headSprite;
	private Vector2 headSize;
	private Vector2 headNextPosition;
	private Vector3 headRotate;

	//shadow
	private GameObject headShadow;
	private Vector2 headShadowNextPosition;

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

	//endless stages spawn
	public Stage_Templates[] stageTemplates;
	public int stagesInitialCount;
	private GameObject recentStageObject;
	private float recentStageHeight;
	//random prefabs to spawn
	private List<GameObject> prefabsToRand;
	private GameObject selectedPrefab;
	private float selectedPrefabHeight;
	private Vector2 spawnPosition;


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

		//moving field
		movingField = GameObject.Find("MovingField");
		movingFieldPosition = movingField.transform.position;
		distance = 0;
		movingLine = GameObject.Find ("MovingLine").transform.position.y;

		//Head dimension
		head = GameObject.Find ("Head");
		headSprite = head.GetComponent<SpriteRenderer> ().sprite;

		//sprite width = bound x 2 x scale
		headSize = new Vector2 (headSprite.bounds.extents.x * 2 * head.transform.localScale.x, headSprite.bounds.extents.y * 2 * head.transform.localScale.y);

		headNextPosition = new Vector2 (0, 0); // initialize only
		headRotate = new Vector3(0,0,0); //initialize only

		//shadow
		headShadow = GameObject.Find ("Shadow");
		headShadowNextPosition = new Vector2 (0, 0); // initialize only

		lastMoveTime = 0f; //initialize only

		isPlaying = false;
		direction = Vector2.up;
		tempDirection = Vector2.up;

		//tail initial spawn
		tailTransform = new List<Transform> ();
		tailSpawnPosition = new Vector2(head.transform.position.x,head.transform.position.y - headSize.y);
		for (int i = 0; i < tailInitialCount; i++) {
			tailSpawnObject = (GameObject)Instantiate (tailPrefab, tailSpawnPosition, Quaternion.identity);
			tailSpawnObject.transform.parent = movingField.transform;
			tailTransform.Add (tailSpawnObject.transform);
			tailSpawnPosition.y = tailSpawnPosition.y - headSize.y;
		}

		//digest
		digestObject = new Queue<GameObject>();


		//stages inital spawn
		prefabsToRand = new List<GameObject>();
		recentStageObject = GameObject.Find("Stage0");
		for (int i = 0; i < stagesInitialCount; i++) {
			spawnStages ();
		}

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

		//move shadow
		headShadowNextPosition.x = headShadow.transform.position.x + (direction.x * headSize.x);
		headShadowNextPosition.y = headShadow.transform.position.y + (direction.y * headSize.y);
		headShadow.transform.position = headShadowNextPosition;

		//move field
		if (head.transform.position.y>movingLine) {
			movingFieldPosition.y = movingFieldPosition.y - (direction.y * headSize.y);
			movingField.transform.position = movingFieldPosition;
			distance++;
		}

	}

	public void eat(){
		tempDigestObject = (GameObject)Instantiate (digestPrefab, head.transform.position, Quaternion.identity);
		tempDigestObject.transform.parent = movingField.transform;
		digestObject.Enqueue(tempDigestObject);
	}

	private void Digesting(){
		if (digestObject.Count>0 &&
			Vector2.Distance(digestObject.Peek().transform.position,
				tailTransform[tailTransform.Count-1].position) <= 0.1f) {
			tempDigestObject = digestObject.Dequeue ();


			//spawn tail
			tailSpawnPosition = tempDigestObject.transform.position;
			tailSpawnObject = (GameObject)Instantiate (tailPrefab, tailSpawnPosition, Quaternion.identity);
			tailSpawnObject.transform.parent = movingField.transform;
			tailTransform.Add (tailSpawnObject.transform);

			GameObject.Destroy (tempDigestObject);
		}
	}

	public void spawnStages(){
		prefabsToRand.Clear ();

		for (int i = 0; i < stageTemplates.Length; i++) {
			//check distance condition (-1 for endless distance max)
			if (distance>=stageTemplates[i].distanceMin && 
				(distance<=stageTemplates[i].distanceMax || stageTemplates[i].distanceMax<0)) {
				//put all prefabs to list
				for (int j = 0; j < stageTemplates[i].stage_prefabs.Length; j++) {
					prefabsToRand.Add (stageTemplates [i].stage_prefabs [j]);
				}
			
			}
		}

		selectedPrefab = prefabsToRand [Random.Range (0, prefabsToRand.Count)];

		selectedPrefabHeight = selectedPrefab.GetComponent<SpriteRenderer> ().sprite.bounds.extents.y * selectedPrefab.transform.localScale.y;
		recentStageHeight = recentStageObject.GetComponent<SpriteRenderer> ().sprite.bounds.extents.y * recentStageObject.transform.localScale.y;
		spawnPosition = recentStageObject.transform.position;
		spawnPosition.y = spawnPosition.y + (recentStageHeight + selectedPrefabHeight);
		recentStageObject = (GameObject)Instantiate (selectedPrefab, spawnPosition, Quaternion.identity);
		recentStageObject.transform.parent = movingField.transform;
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
