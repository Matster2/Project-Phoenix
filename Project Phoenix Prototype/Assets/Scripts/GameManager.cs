/*
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerInGame.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Photon.MonoBehaviour
{
    public Transform playerPrefab;
	public int playerKillMonster;
	public Text playerName;
	public Text monsterKillerCounter;
	public Text hitCombo;
	public int m_hitCombo;
	//[HideInInspector]
	public float hitComboLifeTime = 0.5f;
	public float respawnTime = 5.0f;
	public List<Transform> spawnPoses;
	[HideInInspector]
	public GameObject player;
	private float timer = 0;

    float deltaTime = 0.0f;

	//VolumeSlide
	public Slider volumeSlider;

    public void Awake()
    {
        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.connected)
        {
            Application.LoadLevel(DemoLobby.SceneNameMenu);
            return;
        }
		monsterKillerCounter.text = "Player Kill " + playerKillMonster.ToString() + " Monsters";
		playerName.text = PhotonNetwork.playerName;
        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoses[Random.Range(0,spawnPoses.Count)].position, spawnPoses[Random.Range(0,spawnPoses.Count)].rotation, 0) as GameObject;
        Camera.main.GetComponent<GrayscaleEffect>().enabled = false;
    }

    public void PlayerHitMonster(string from)
	{
		if(player.name == from)
		{
			m_hitCombo ++;
			hitComboLifeTime = 1.0f;
		}
	}

	public void PlayerKillMonster(string from)
	{
		if(player.name == from)
		{
			playerKillMonster++;
			monsterKillerCounter.text = "Player Kill " + playerKillMonster.ToString() + " Monsters";
		}
	}

    public void OnGUI()
    {
        if (GUILayout.Button("Return to Lobby"))
        {
            PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
        }

        int w = Screen.width, h = Screen.height;
        
        GUIStyle style = new GUIStyle();
        
        Rect rect = new Rect(50, 150, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color (0.0f, 1f, 0.0f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }


    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.Disconnect();
        }
    }

	void FixedUpdate()
	{


		if(player == null)
		{
            Camera.main.GetComponent<GrayscaleEffect>().enabled = true;
			timer += Time.deltaTime;
			if(timer>=respawnTime)
			{
                Camera.main.GetComponent<GrayscaleEffect>().enabled = false;
				player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoses[Random.Range(0,spawnPoses.Count)].position, spawnPoses[Random.Range(0,spawnPoses.Count)].rotation, 0) as GameObject;

			}


		}
		else
		{
			timer = 0;
		}

		if(m_hitCombo > 1)
		{
			hitComboLifeTime -= Time.deltaTime;
			if(hitComboLifeTime <= 0)
			{
				m_hitCombo = 0;
				//hitComboLifeTime = 0.5f;
			}
		}

		if(hitComboLifeTime >=0)
		{
			hitCombo.text = "Hit Combo " + m_hitCombo.ToString() + "X";
			hitCombo.color = new Color(1,0,0,hitComboLifeTime);
		}
		else
		{
			hitCombo.text = "";
		}

	}

	public void SoundVolume()
	{
		AudioListener.volume = 0.8f;
	}
    


    public void OnMasterClientSwitched(PhotonPlayer player)
    {
        Debug.Log("OnMasterClientSwitched: " + player);

        string message;
        InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message

        if (chatComponent != null)
        {
            // to check if this client is the new master...
            if (player.isLocal)
            {
                message = "You are Master Client now.";
            }
            else
            {
                message = player.name + " is Master Client now.";
            }


            chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
        }
    }

    public void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom (local)");
        
        // back to main menu        
        Application.LoadLevel(DemoLobby.SceneNameMenu);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");

        // back to main menu        
		Application.LoadLevel(DemoLobby.SceneNameMenu);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerConnected: " + player);
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPlayerDisconneced: " + player);
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("OnFailedToConnectToPhoton");

        // back to main menu        
		Application.LoadLevel(DemoLobby.SceneNameMenu);
    }
}
*/