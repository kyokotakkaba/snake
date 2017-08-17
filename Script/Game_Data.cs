using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game_Data : MonoBehaviour {
	public Sprite splashImage;

	private bool BGMisPlayed;
	private AudioSource SEaudioSource;


	void Awake() {
		GameObject.DontDestroyOnLoad (gameObject); 
	}
	void OnDestroy() {
		//SaveLoad.Clear ();
	}

	// Use this for initialization
	void Start () {
		SEaudioSource = this.gameObject.AddComponent<AudioSource> ();
		GameObject.Find ("splash").GetComponent<Image> ().sprite = splashImage;
		PlayerPrefs.SetString ("selectedStage", "");
		PlayerPrefs.SetString ("lastLevel", "");
		//SaveLoad.Store (gameObject);
		BGMisPlayed = false;
	}

	public void BGMPlayed(){
		BGMisPlayed = true;
	}

	public bool getBGMStatus(){
		return BGMisPlayed;
	}

	public AudioSource getSEaudioSource(){
		return SEaudioSource;
	}
}
