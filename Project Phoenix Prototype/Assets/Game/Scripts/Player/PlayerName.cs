using UnityEngine;
using System.Collections;

public class PlayerName : MonoBehaviour {

	string characterName;

	void Awake() {
		characterName = PlayerPrefs.GetString ("CHARACTER_NAME");
	}

	void OnGUI() {
		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position);

		GUI.Label (new Rect((pos.x - 50), (Screen.height - pos.y + 10), 100, 50), characterName);
	}
}


