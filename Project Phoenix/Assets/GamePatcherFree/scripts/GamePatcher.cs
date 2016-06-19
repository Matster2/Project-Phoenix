using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

public class GamePatcher : MonoBehaviour {

	private bool force = false; // Change if you always wants to be 100% sure that the user is using the latest version (true)
	public string version = "";
	public string url = "";

	private string nextUpdate = null;

	// GUI stuff
	private bool showError = false;
	private bool reqUpdate = false;

	// Use this for initialization
	void Start () {
		StartCoroutine(checkOnline(url));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void startGame(){
		// Load next scene or whatever...
		Debug.Log("[SYSTEM] Starting game...");
	}

	IEnumerator checkOnline(string url){
		url += "versionInfo.xml";
		
		try {
			// encrypt the PW for better security;
			HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(url);
			
			httpWReq.Method = "POST";
			httpWReq.ContentType = "application/x-www-form-urlencoded";
			
			HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
			if(response.StatusCode == HttpStatusCode.OK)
			{
				string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
				Debug.Log("[SYSTEM] Response from WWW-patch: " + responseString);

				if(isLatestVersion(responseString)){
					// We have the latest version load next screen or show login...
					Debug.Log("[SYSTEM] Client is up to date...");
					startGame();
				}else{
					if(force){
						reqUpdate = true;
					}else{
						startGame();
					}
				}
	
			}else{
				// Most likely the url is wrong
				Debug.Log("[SYSTEM] Error while getting version info: " + response.StatusCode);
				if(force){
					showError = true;
				}else{
					startGame();
				}
			}
		}catch (Exception ex){
			Debug.Log("[SYSTEM] Error while getting version info: " + ex.Message);
			// The WWW server is offline
			if(force){
				showError = true;
			}else{
				startGame();
			}
		}

		yield return new WaitForSeconds(1.0f); // Wait 1 second so we dont overflow anything
	}

	public bool isLatestVersion(string rawXML){
		XmlDocument xml = new XmlDocument();
		xml.LoadXml(rawXML);

		string thisId = null;
		string thisVersion = null;

		// check what id our current version is
		XmlNodeList xnList = xml.SelectNodes("/versions/patch");
		foreach (XmlNode xn in xnList)
		{
			string id = xn["id"].InnerText;
			string versionName = xn["build"].InnerText;
			if(versionName.Equals(version)){
				thisId = id;
				thisVersion = versionName;
				break;
			}
		}

		// if thisId or thisVersion still is null there is an error
		if(thisId == null || thisVersion == null){
			return false;
		}else{
			Debug.Log("[SYSTEM] Current version is: " + thisVersion + " (#" + thisId +")");
		}

		// We now need to check if there are more updates
		foreach (XmlNode xn in xnList)
		{
			string id = xn["id"].InnerText;
			string versionName = xn["build"].InnerText;
			if(id.Equals(Convert.ToInt32(thisId) + 1 + "")){
				nextUpdate = versionName;
				force = Convert.ToBoolean(xn["force"].InnerText);
				Debug.Log("[SYSTEM] Next update is: " + versionName + " (#" + id +") which is forces: " + force);
				return false;
			}
		}

		// If we made it this far, then there are no new updates
		return true;
	}

	Rect windowRect = new Rect((Screen.width / 2) - 200, (Screen.height / 2) - 50, 400, 100);

	void OnGUI(){
		if(showError){
			windowRect = GUI.Window(0, windowRect, errorWindow, "Error");
		}
		if(reqUpdate){
			windowRect = GUI.Window(0, windowRect, updateWindow, "An update is required");
		}

		// Show version number
		GUI.Label(new Rect(10, 10, 500, 100), "Version: " + version.ToString());

	}

	void errorWindow(int windowID){
		GUI.Label(new Rect(10, 20, 500, 100), "There was an error recieving the update, please try again...");
	}

	void updateWindow(int windowID){
		GUI.Label(new Rect(10, 20, 500, 100), "An update to version: " + nextUpdate + " is required.");
		if(GUI.Button(new Rect(10, 50, 100, 33), "Update")){

			// We need to start diffrent launchers depending on our os
			#if UNITY_STANDALONE_WIN

			try{
				System.Diagnostics.Process.Start("../launcher.exe");
			}catch(Exception e){
				Debug.Log("[SYSTEM] Could not locate Launcher.exe: " + e.Message);
			}

			#endif

			#if UNITY_STANDALONE_LINUX

			try{
				System.Diagnostics.Process.Start("../launch");
			}catch(Exception e){
				Debug.Log("[SYSTEM] Could not locate launcher shell script: " + e.Message);
			}

			#endif

			#if UNITY_STANDALONE_OSX

			try{
				System.Diagnostics.Process.Start("../launch");
			}catch(Exception e){
				Debug.Log("[SYSTEM] Could not locate launcher shell script: " + e.Message);
			}

			#endif

			Application.Quit();
		}
	}
}
