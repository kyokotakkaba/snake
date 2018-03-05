using UnityEngine;
using System.Collections;

public class Head_Script : MonoBehaviour {
	private Game_Script game;

	void Start(){
		game = GameObject.Find ("Canvas").GetComponent<Game_Script> ();
	}

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.name.StartsWith ("Food")) {
			//eating
			game.eat ();
			Destroy (coll.gameObject);
		} else if (coll.name.StartsWith("LeftTurnPoint")) {
			game.TutorialTurnLeftInstruction ();
		} else if (coll.name.StartsWith("RightTurnPoint")) {
			game.TutorialTurnRightInstruction ();
		} else if (coll.name.StartsWith("EndPoint")) {
			game.TutorialEndInstruction ();
		} else if (!coll.name.StartsWith("Stage")) {
			//gameover
			game.Gameover();
		}
	}
}
