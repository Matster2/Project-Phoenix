using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System;
using System.Text;

public class AccountSecurity {

	private int saltSize = 32;

	public string CreateSalt()
	{
		var rng = new RNGCryptoServiceProvider();
		var buff = new byte[saltSize];
		rng.GetBytes(buff);
		return Convert.ToBase64String(buff);
	}

	public string GenerateHashSHA256(string input, string salt)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(input + salt);
		SHA256Managed sha256HashString = new SHA256Managed();
		byte[] hash = sha256HashString.ComputeHash(bytes);
		return BytesToHex(hash);
	}

	public bool VerifyPassword(string password, string dbPassword, string salt)
	{
		string hashedPass = GenerateHashSHA256(password, salt);
		Debug.LogFormat ("saved password: {0}, checking: {1}", dbPassword, hashedPass);
		if (hashedPass == dbPassword)
		{
			return true;
		}
		else 
		{
			return false;
		}
	}

	private string BytesToHex(byte[] bytes)
	{
		string hexString = string.Empty;

		foreach (byte b in bytes)
		{
			hexString += String.Format("{0:x2}", b);
		}

		return hexString;
	}
}
