using UnityEngine;
using UnityEngine.SceneManagement;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class DatabaseTest : UI {

	public string email = "jamesisdifferent@gmail.com";

	void Start() {
		Test ();
	}

	void Test() {
		string query = string.Empty;
		try
		{
			query = String.Format("SELECT * FROM {0}.account WHERE account_email = '{1}';", DatabaseInformation.DB_NAME, email);
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
							Debug.LogFormat("Email: {0} | Username: {1}", reader["account_email"].ToString(), reader["account_username"].ToString());
						}
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
