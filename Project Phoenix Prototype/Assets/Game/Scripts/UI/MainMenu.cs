using UnityEngine;
using UnityEngine.SceneManagement;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class MainMenu : UI {

	private struct CharacterInformation {
		public int character_id;
		public string character_name;
		public int character_level;

		public CharacterInformation(int character_id, string character_name, int character_level) {
			this.character_id = character_id;
			this.character_name = character_name;
			this.character_level = character_level;
		}
	}

	private enum Menu {
		CHARACTER_SELECTION,
		CHARACTER_CREATION
	}

	Menu currentMenu = Menu.CHARACTER_SELECTION;

	int userID;
	string character_creation_name = "";

	List<CharacterInformation> characters;


	void Start() {
		userID = PlayerPrefs.GetInt ("USER_ID");
		FetchCharacters ();
	}

	void OnGUI() {
		switch (currentMenu) {
			case Menu.CHARACTER_CREATION: 
			{
				CharacterCreationGUI ();
				break;
			}
			case Menu.CHARACTER_SELECTION: 
			{
				CharacterSelectionGUI ();
				break;
			}	
		}
	}

	void CharacterSelectionGUI() {
		GUILayout.BeginArea (new Rect ((Screen.width / 2) - 100, (Screen.height / 2) - 200, 200, 400), GUIContent.none, "box");

		GUILayout.Label ("Character Selection");

		if (characters != null) {
			for (int i = 0; i < characters.Count; i++) {
				if (GUILayout.Button(characters[i].character_name)) {
					PlayerPrefs.SetInt ("CHARACTER_ID", characters [i].character_id);
					PlayerPrefs.SetString ("CHARACTER_NAME", characters [i].character_name);
					SceneManager.LoadScene ("Basic Multiplayer Test");
				}
				GUILayout.Space(5);
			}
		} else {
			GUILayout.Label ("No Characters");
			GUILayout.Space (5);
		}

		GUI.backgroundColor = Color.red;
		if (characters == null || characters.Count < 3) {
			if (GUILayout.Button("Create Character")) {
				currentMenu = Menu.CHARACTER_CREATION;
			}
		}

		GUILayout.EndArea ();
	}

	void CharacterCreationGUI() {
		GUILayout.BeginArea (new Rect ((Screen.width / 2) - 100, (Screen.height / 2) - 175, 200, 350), GUIContent.none, "box");

		GUILayout.Label ("Character Creation");

		GUILayout.Label ("Character Name");
		character_creation_name = GUILayout.TextField (character_creation_name, 20);

		GUILayout.Space (5);
		if (GUILayout.Button("Create Character")) {
			CreateCharacter ();
		}

		GUILayout.Space (20);
		if (GUILayout.Button("Back")) {
			FetchCharacters ();
			currentMenu = Menu.CHARACTER_SELECTION;
		}

		GUILayout.EndArea ();
	}

	void FetchCharacters() {
		characters = new List<CharacterInformation>();

		string query = string.Empty;
		try
		{				
			using (connection)
			{
				query = String.Format("SELECT * FROM {0}.character WHERE user_id = '{1}'", DatabaseInformation.DB_NAME, userID);
				command = new MySqlCommand(query, connection);
				if (connection.State.ToString() != "Open")
					connection.Open();

				reader = command.ExecuteReader();

				while (reader.HasRows) {
					while (reader.Read())
					{
						Debug.Log(reader["character_name"].ToString());
						CharacterInformation characterInformation = new CharacterInformation((int)reader["character_id"], reader["character_name"].ToString(), (int)reader["character_level"]);
						characters.Add(characterInformation);
					}
					reader.NextResult();
				}
				reader.Close();
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
		}
		finally
		{
		}
	}

	void CreateCharacter() {
		string query = string.Empty;
		// Error trapping in the simplest form
		try
		{
			using (connection)
			{
				query = String.Format("INSERT INTO {0}.character (user_id, character_name, character_date_created) VALUES (?user_id, ?character_name, ?character_date_created)", DatabaseInformation.DB_NAME);
				command = new MySqlCommand(query, connection);
				if (connection.State.ToString() != "Open")
					connection.Open();
				
				using (command = new MySqlCommand(query, connection))
				{
					MySqlParameter oParam1 = command.Parameters.Add("?user_id", MySqlDbType.Int16);
					oParam1.Value = userID;
					MySqlParameter oParam2 = command.Parameters.Add("?character_name", MySqlDbType.VarChar);
					oParam2.Value = character_creation_name;
					MySqlParameter oParam3 = command.Parameters.Add("?character_date_created", MySqlDbType.DateTime);
					oParam3.Value = System.DateTime.Now;
					if (command.ExecuteNonQuery() >= 1) {
						currentMenu = Menu.CHARACTER_SELECTION;
						FetchCharacters ();
						character_creation_name = String.Empty;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
		}
		finally
		{
		}
	}
}