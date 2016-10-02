using UnityEngine;
using UnityEngine.SceneManagement;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class CreateAccount : UI {
	string password = "password03";

	string temp_email = "testuser03@gmail.com";
	string temp_name = "TestUser03";
	string temp_password = "";
	string temp_salt = "";

	private int saltSize = 32;

	void Start() {

		AccountSecurity accountSecurity = new AccountSecurity ();
		temp_salt = accountSecurity.CreateSalt();
		temp_password = accountSecurity.GenerateHashSHA256(password, temp_salt);
		CreateUser ();
	}

	void CreateUser() {
		string query = string.Empty;
		// Error trapping in the simplest form
		try
		{
			using (connection)
			{
				query = String.Format("INSERT INTO {0}.user (user_email, user_name, user_password, user_salt, user_date_created) VALUES (?user_email, ?user_name, ?user_password, ?user_salt, ?user_date_created)", DatabaseInformation.DB_NAME);
				command = new MySqlCommand(query, connection);
				if (connection.State.ToString() != "Open")
					connection.Open();

				using (command = new MySqlCommand(query, connection))
				{
					MySqlParameter oParam1 = command.Parameters.Add("?user_email", MySqlDbType.VarChar);
					oParam1.Value = temp_email;
					MySqlParameter oParam2 = command.Parameters.Add("?user_name", MySqlDbType.VarChar);
					oParam2.Value = temp_name;
					MySqlParameter oParam3 = command.Parameters.Add("?user_password", MySqlDbType.VarChar);
					oParam3.Value = temp_password;
					MySqlParameter oParam4 = command.Parameters.Add("?user_salt", MySqlDbType.VarChar);
					oParam4.Value = temp_salt;
					MySqlParameter oParam5 = command.Parameters.Add("?user_date_created", MySqlDbType.DateTime);
					oParam5.Value = System.DateTime.Now;
					command.ExecuteNonQuery();
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
