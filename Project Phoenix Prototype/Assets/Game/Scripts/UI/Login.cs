using UnityEngine;
using UnityEngine.SceneManagement;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class Login : UI {

	private enum Menu {
		LOGIN
	}

	Menu currentMenu = Menu.LOGIN;
	AccountSecurity accountSecurity = new AccountSecurity();

	//Login Variables
	string login_message = "";
	string login_username = "";
	string login_password = "";

	void OnGUI() {
		switch (currentMenu) {
		case Menu.LOGIN: 
			LoginGUI ();
			break;
		}
	}

	void LoginGUI(){
		GUILayout.BeginArea (new Rect ((Screen.width / 2) - 100, (Screen.height / 2) - 120, 200, 240), GUIContent.none, "box");

		GUILayout.Label (login_message);

		GUILayout.Label ("Username");
		login_username = GUILayout.TextField (login_username, 20);

		GUILayout.Label ("Password");
		login_password = GUILayout.PasswordField (login_password, "*"[0], 20);

		GUILayout.Space(5);
		if (GUILayout.Button("Login")) {
			CheckAccount ();
		}

		GUILayout.Space(10);
		if (GUILayout.Button("Register")) {
			Application.OpenURL("http://google.com/");
		}

		GUILayout.EndArea ();
	}

	void CheckAccount() {
		string query = string.Empty;
		try
		{
			query = String.Format("SELECT * FROM {0}.account WHERE account_username = '{1}';", DatabaseInformation.DB_NAME, login_username);
			Debug.Log(query);
			if (connection.State.ToString() != "Open")
				connection.Open();
			using (connection)
			{
				using (command = new MySqlCommand(query, connection))
				{
					reader = command.ExecuteReader();
					if (reader.HasRows) {
						while (reader.Read())
						{
							Debug.LogFormat("password: {0}, saved password: {1}, salt: {2}",login_password, reader["account_password"].ToString(), reader["account_salt"].ToString());
							Debug.Log(accountSecurity.VerifyPassword(login_password, reader["account_password"].ToString(), reader["account_salt"].ToString()));
							if (accountSecurity.VerifyPassword(login_password, reader["account_password"].ToString(), reader["account_salt"].ToString())) {
								Debug.Log(reader["account_id"].ToString());
								PlayerPrefs.SetInt("USER_ID", (int)reader["account_idid"]);
								SceneManager.LoadScene("MainMenu");	
							} else {
								login_message = "Incorrect Login Details.\nPlease Try Again.";
							}
						}
					} else {
						login_message = "Incorrect Login Details.\nPlease Try Again.";
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
