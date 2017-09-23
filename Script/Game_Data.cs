using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using UnityEngine.Advertisements;

public class Game_Data : MonoBehaviour {
	public Sprite splashImage;

	private bool BGMisPlayed;
	private AudioSource SEaudioSource;

	//private string gameId = "1552172";


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
		//SaveLoad.Store (gameObject);
		BGMisPlayed = false;

		/*if (Advertisement.isSupported) {
			Advertisement.Initialize (gameId, true);
		}*/
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

	/*public void ShowAd()
	{
		if (Advertisement.IsReady())
		{
			Advertisement.Show();
		}
	}*/
}
