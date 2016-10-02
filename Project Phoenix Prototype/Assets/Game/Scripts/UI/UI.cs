using UnityEngine;
using UnityEngine.SceneManagement;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class UI : MonoBehaviour {

	public bool pooling = true;

	protected string connectionString;
	protected MySqlConnection connection = null;
	protected MySqlCommand command = null;
	protected MySqlDataReader reader = null;

	void Awake() {
		string poolingString = (pooling) ? "true" : "false";
		connectionString = String.Format ("Server={0};Database={1};User={2};Password={3};Pooling={4};", DatabaseInformation.DB_HOST, DatabaseInformation.DB_NAME, DatabaseInformation.DB_USER, DatabaseInformation.DB_PASSWORD, poolingString);

		try {
			connection = new MySqlConnection(connectionString);
			connection.Open();
			Debug.LogFormat("MySQL State: {0}", connection.State);
		} catch (Exception e) {
			Debug.Log (e);
		}
	}

	void OnApplicationQuit() {
		if (connection != null) {
			if (connection.State.ToString() != "Closed") {
				connection.Close ();
				Debug.Log ("MySQL Connection Closed");
			}
		}
	}

	public string GetConnectionState() {
		return connection.State.ToString();
	}
}