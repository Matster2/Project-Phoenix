using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ManageConnection : NetworkManager {

	void Awake() {

		if (PlayerPrefs.HasKey("CHARACTER_NAME")) {
			StartClient ();	
			Debug.Log ("STARTING CLIENT");
		} else {
			StartServer ();
			Debug.Log ("STARTING SERVER");
		}
	}


}
