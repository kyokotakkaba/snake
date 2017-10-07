using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class Game_Script : MonoBehaviour {
	//ads component
	private Ads ads;

	//Audio
	public AudioClip selectSound;
	public AudioClip eatSound;
	public AudioClip rotateSound;
	public AudioClip deathSound;
	private AudioSource BGMaudioSource;
	private AudioSource SEaudioSource;
	//mute
	private GameObject muteONButton;
	private GameObject muteOFFButton;
	private int muteState;

	//revive
	private GameObject reviveButton;
	private GameObject reviveDisableButton;
	private bool reviveState;
	private GameObject startReviveUI;

	//UI object
	private GameObject menuUI;
	private GameObject howtoUI;
	private GameObject playUI;
	private GameObject gameoverUI;
	private GameObject reviveUI;
	private GameObject pauseUI;
	private GameObject quitUI;





	//camera
	private Camera cameraView;
	private float InitialOrthographicSize;
	public float zoomOutSpeed;

	//Scoring UI
	private Text currentScoreText;
	private Text scoreText;
	private Text bestscoreText;
	private Text newBestScoreText;

	//moving field
	private GameObject movingField;
	private Vector2 movingFieldPosition;
	private float movingLine;
	private int distance;

	//moving spike
	private GameObject borderBottom;
	private Vector2 spikePosition;
	public float spikeSpeed;
	public float spikeRestoreSpeed;

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

	//stages
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
	public float currentSnakeSpeed;
	public float speedIncrement;
	public float currentSpeedIncrement;
	public float incrementReducer;
	private float lastMoveTime;

	private bool isPlaying;

	private Vector2 direction;
	private Vector2 lastDirection;
	private Queue<Vector2> tempDirection; //Multiple direction change on delay
	private Queue<int> tempRotate;

	//scoring
	private int score;
	private int bestscore;

	//restart initial parameter
	private Vector2 movingFieldInitialPosition;
	private Vector2 spikeInitialPosition;
	private Vector2 headInitialPosition;
	private Quaternion headInitialRotation;
	private Vector2 shadowInitialPosition;

	public GameObject stageInitialPrefab;
	public GameObject stageRevivePrefab;



	// Use this for initialization
	void Start () {
		ads = this.GetComponent<Ads> ();

		//audio
		BGMaudioSource = this.GetComponent<AudioSource>();
		SEaudioSource = this.gameObject.AddComponent<AudioSource> ();
		//mute
		muteONButton = GameObject.Find ("MuteON");
		muteOFFButton = GameObject.Find ("MuteOFF");
		muteState = PlayerPrefs.GetInt("mutestate",0);
		if (muteState > 0) {
			muteONButton.SetActive (false);
			//BGMaudioSource.Stop ();
		} else {
			muteOFFButton.SetActive (false);
			BGMaudioSource.Play ();
		}


		//revive
		reviveButton = GameObject.Find ("Revive");
		reviveDisableButton = GameObject.Find ("ReviveDisabled");
		reviveDisableButton.SetActive (false);
		reviveState = false;
		startReviveUI = GameObject.Find ("StartRevive");
		startReviveUI.SetActive (false);


		//UI
		menuUI = GameObject.Find ("MainMenu");
		howtoUI = GameObject.Find ("HowTo");
		playUI = GameObject.Find ("GamePlay");
		gameoverUI = GameObject.Find ("GameOver");
		pauseUI = GameObject.Find ("Pause");
		quitUI = GameObject.Find ("Quit");



		//Camera
		cameraView = GameObject.Find ("Camera").GetComponent<Camera>();
		InitialOrthographicSize = cameraView.orthographicSize;

		currentScoreText = GameObject.Find ("CurrentScore").GetComponent<Text>();
		scoreText = GameObject.Find ("Score").GetComponent<Text>();
		bestscoreText = GameObject.Find ("BestScore").GetComponent<Text>();
		newBestScoreText = GameObject.Find ("NewBestScoreLabel").GetComponent<Text>();

		howtoUI.SetActive (false);
		playUI.SetActive (false);
		gameoverUI.SetActive (false);
		pauseUI.SetActive (false);
		quitUI.SetActive (false);

		//moving field
		movingField = GameObject.Find("MovingField");
		movingFieldPosition = movingField.transform.position;
		distance = 0;
		movingLine = GameObject.Find ("MovingLine").transform.position.y;

		//moving spike
		borderBottom = GameObject.Find("BorderBottom");
		spikeInitialPosition = borderBottom.transform.position;
		spikePosition = spikeInitialPosition;

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
		tempDirection = new Queue<Vector2>();
		tempRotate = new Queue<int> ();

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

		//speed
		currentSnakeSpeed = snakeSpeed;
		currentSpeedIncrement = speedIncrement;

		//score
		score = 0;
		bestscore = PlayerPrefs.GetInt("bestscore",0);


		//restart initial parameter
		movingFieldInitialPosition = movingField.transform.position;
		headInitialPosition = head.transform.position;
		headInitialRotation = head.transform.rotation;
		shadowInitialPosition = headShadow.transform.position;


	}
	
	// Update is called once per frame
	void Update () {


		if (isPlaying) {

			if (cameraView.orthographicSize < 5) {
				cameraView.orthographicSize += Time.deltaTime * zoomOutSpeed;
			}else if (cameraView.orthographicSize > 5.1) {
				cameraView.orthographicSize -= Time.deltaTime;
			}
			
			//if back button pressed, pause game
			if (Input.GetKeyDown (KeyCode.Escape)) {
				Pause ();
			} 

			//call every snakeSpeed seconds
			if (Time.time >= lastMoveTime) {
				
				lastMoveTime = Time.time + currentSnakeSpeed; //refresh rapid fire
				if (tempDirection.Count>0) {
					direction = tempDirection.Dequeue(); //assign direction
				}

				Digesting ();
				Move ();

			}
		} else {
			//quiting
			if (Input.GetKeyDown (KeyCode.Escape)) {
				if (!quitUI.activeSelf) {
					quitUI.SetActive (true);
				} else {
					clickCancelQuit();
				}

			} 
		}
	}

	private void Move(){

		//rotate head sprite
		if (tempRotate.Count>0) {
			SEaudioSource.PlayOneShot (rotateSound, 0.6f);
			headRotate.z = tempRotate.Dequeue();
		}
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
			currentSnakeSpeed = currentSnakeSpeed / (1 + currentSpeedIncrement / 1000);
			currentSpeedIncrement = currentSpeedIncrement / (1 + incrementReducer / 1000);


			//restore spike
			if (spikePosition.y>spikeInitialPosition.y) {
				spikePosition.y = spikePosition.y - (headSize.y * spikeSpeed * spikeRestoreSpeed);
				borderBottom.transform.position = spikePosition;
			}
		}else{
			//move spike
			spikePosition.y = spikePosition.y + (headSize.y * spikeSpeed);
			borderBottom.transform.position = spikePosition;
		}

	}

	public void eat(){
		SEaudioSource.PlayOneShot (eatSound, 0.6f);

		tempDigestObject = (GameObject)Instantiate (digestPrefab, head.transform.position, Quaternion.identity);
		tempDigestObject.transform.parent = movingField.transform;
		digestObject.Enqueue(tempDigestObject);

		//adding score
		score++;
		currentScoreText.text = "" + score;
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

	private void Pause(){
		if (isPlaying) {
			playSelectSound ();

			isPlaying = false;
			pauseUI.SetActive (true);
		}
	}

	public void clickResume(){
		if (!isPlaying) {
			playSelectSound ();

			isPlaying = true;
			pauseUI.SetActive (false);
		}
	}


	public void Gameover(){
		if (isPlaying) {
			SEaudioSource.PlayOneShot (deathSound, 0.6f);
			isPlaying = false;
			playUI.SetActive (false);
			gameoverUI.SetActive (true);
			pauseUI.SetActive (false);

			if (reviveState) {
				reviveButton.SetActive (false);
				reviveDisableButton.SetActive (true);
			} else {
				reviveButton.SetActive (true);
				reviveDisableButton.SetActive (false);
			}


			newBestScoreText.text = "";
			if (score>bestscore) {
				PlayerPrefs.SetInt("bestscore", score);
				bestscore = score;
				newBestScoreText.text = "NEW!";
			}
			scoreText.text = "" + score;
			bestscoreText.text = "" + bestscore;


			ads.addInterstitialCounter ();
		}
	}


	public void clickStart(){
		playSelectSound ();


		menuUI.SetActive (false);
		playUI.SetActive (true);
		currentScoreText.text = "" + score;
		isPlaying = true;
		lastMoveTime = Time.time + snakeSpeed;
	}

	public void clickLeftControl(){
		
		tempRotate.Enqueue(90);


		if (tempDirection.Count <= 0) {
			lastDirection = direction;
		}

		if (lastDirection == Vector2.up) {
			lastDirection = Vector2.left;
		} else if (lastDirection == Vector2.left) {
			lastDirection = Vector2.down;
		} else if (lastDirection == Vector2.down) {
			lastDirection = Vector2.right;
		} else if (lastDirection == Vector2.right) {
			lastDirection = Vector2.up;
		}

		tempDirection.Enqueue (lastDirection);

		
	}

	public void clickRightControl(){
		
		tempRotate.Enqueue(-90);


		if (tempDirection.Count <= 0) {
			lastDirection = direction;
		}

		if (lastDirection == Vector2.up) {
			lastDirection = Vector2.right;
		} else if (lastDirection == Vector2.right) {
			lastDirection = Vector2.down;
		} else if (lastDirection == Vector2.down) {
			lastDirection = Vector2.left;
		} else if (lastDirection == Vector2.left) {
			lastDirection = Vector2.up;
		}

		tempDirection.Enqueue (lastDirection);

	}

	private void refresh(){
		//destroy all stage and tail
		GameObject[] clones = GameObject.FindGameObjectsWithTag("Clone");
		foreach (GameObject clone in clones) {
			GameObject.Destroy(clone);
		}

		//return to initial position
		movingField.transform.position = movingFieldInitialPosition;

		spikePosition = spikeInitialPosition;
		borderBottom.transform.position = spikePosition;

		head.transform.position = headInitialPosition;
		head.transform.rotation = headInitialRotation;
		headShadow.transform.position = shadowInitialPosition;


		//instantiate stage 0
		recentStageObject = (GameObject)Instantiate (stageInitialPrefab);
		recentStageObject.transform.parent = movingField.transform;

		//reset UI
		playUI.SetActive (false);
		gameoverUI.SetActive (false);

		//reset parameter
		movingFieldPosition = movingField.transform.position;
		distance = 0;
		headRotate.z = 0;
		isPlaying = false;
		direction = Vector2.up;
		tempDirection.Clear();
		tempRotate.Clear ();
		score = 0;
		currentSnakeSpeed = snakeSpeed;
		currentSpeedIncrement = speedIncrement;
		reviveState = false;

		//tail initial spawn
		tailTransform.Clear();
		tailSpawnPosition = new Vector2(head.transform.position.x,head.transform.position.y - headSize.y);
		for (int i = 0; i < tailInitialCount; i++) {
			tailSpawnObject = (GameObject)Instantiate (tailPrefab, tailSpawnPosition, Quaternion.identity);
			tailSpawnObject.transform.parent = movingField.transform;
			tailTransform.Add (tailSpawnObject.transform);
			tailSpawnPosition.y = tailSpawnPosition.y - headSize.y;
		}

		//digest
		digestObject.Clear();


		//stages inital spawn
		for (int i = 0; i < stagesInitialCount; i++) {
			spawnStages ();
		}
	}

	public void refreshRevive(){
		//destroy all stage and tail
		GameObject[] clones = GameObject.FindGameObjectsWithTag("Clone");
		foreach (GameObject clone in clones) {
			GameObject.Destroy(clone);
		}

		//return to initial position
		movingField.transform.position = movingFieldInitialPosition;

		spikePosition = spikeInitialPosition;
		borderBottom.transform.position = spikePosition;

		head.transform.position = headInitialPosition;
		head.transform.rotation = headInitialRotation;
		headShadow.transform.position = shadowInitialPosition;


		//instantiate stage 0
		recentStageObject = (GameObject)Instantiate (stageRevivePrefab);
		recentStageObject.transform.parent = movingField.transform;

		//reset UI
		playUI.SetActive (true);
		gameoverUI.SetActive (false);

		//reset parameter
		movingFieldPosition = movingField.transform.position;
		headRotate.z = 0;
		isPlaying = false;
		direction = Vector2.up;
		tempDirection.Clear();
		tempRotate.Clear ();
		reviveState = true;

		//tail initial spawn
		int tailCount = tailTransform.Count;
		tailTransform.Clear();
		tailSpawnPosition = new Vector2(head.transform.position.x,head.transform.position.y - headSize.y);
		for (int i = 0; i < tailCount; i++) {
			tailSpawnObject = (GameObject)Instantiate (tailPrefab, tailSpawnPosition, Quaternion.identity);
			tailSpawnObject.transform.parent = movingField.transform;
			tailTransform.Add (tailSpawnObject.transform);
			tailSpawnPosition.y = tailSpawnPosition.y - headSize.y;
		}

		//digest
		digestObject.Clear();


		//stages inital spawn
		for (int i = 0; i < stagesInitialCount; i++) {
			spawnStages ();
		}

		startReviveUI.SetActive (true);
	}


	public void clickRestart(){

		refresh ();

		clickStart ();
	}

	public void clickRevive(){
		ads.showRewardVideo ();
	}

	public void clickStartRevive(){
		clickStart ();
		startReviveUI.SetActive (false);
	}

	public IEnumerator screenshotAndShare(){
		
		yield return new WaitForEndOfFrame();

		//take screen shot
		Texture2D screenTexture = new Texture2D(Screen.width, Screen.height,TextureFormat.RGB24,true);
		screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);
		screenTexture.Apply();

		//save screen shot
		byte[] dataToSave = screenTexture.EncodeToPNG();
		string destination = Application.persistentDataPath+"/myscreenshot.png";
		File.WriteAllBytes(destination, dataToSave);


		//share
		if(!Application.isEditor)
		{
			//if UNITY_ANDROID
			string body = "Can you beat my best score";
			string subject = "Snucky Snake score";

			AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
			intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
			AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
			AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse","file://" + destination);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body );
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
			intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

			// run intent from the current Activity
			currentActivity.Call("startActivity", intentObject);
		}


	}

	public void clickShare(){
		playSelectSound ();
		StartCoroutine (screenshotAndShare ());
	}

	public void clickExit(){
		playSelectSound ();
		menuUI.SetActive (true);
		pauseUI.SetActive (false);
		cameraView.orthographicSize = InitialOrthographicSize;
		refresh ();
	}

	public void clickQuit(){
		playSelectSound ();
		Application.Quit();
	}

	public void clickCancelQuit(){
		playSelectSound ();
		quitUI.SetActive (false);
	}

	public void clickHowTo(){
		playSelectSound ();
		howtoUI.SetActive (true);
	}

	public void clickCloseHowTo(){
		playSelectSound ();
		howtoUI.SetActive (false);
	}

	public void clickMute(){
		if (muteState > 0) {
			muteOFFButton.SetActive (false);
			muteONButton.SetActive (true);
			BGMaudioSource.Play ();
			muteState = 0;
		} else {
			muteONButton.SetActive (false);
			muteOFFButton.SetActive (true);
			BGMaudioSource.Stop ();
			muteState = 1;
		}
		PlayerPrefs.SetInt("mutestate", muteState);
	}

	private void playSelectSound(){
		SEaudioSource.PlayOneShot (selectSound, 0.6f);
	}
}
