using UnityEngine;
using System.Collections;

public class DestroyedCharacter : Photon.MonoBehaviour {
	public float lifeTime = 5;

	private Vector3 correctObjectPos;
	private Quaternion correctObjectRot;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		lifeTime -= Time.deltaTime;
		if(lifeTime<=0)
		{
			PhotonNetwork.Destroy(gameObject);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else
		{
			// Network player, receive data
			this.correctObjectPos = (Vector3)stream.ReceiveNext();
			this.correctObjectRot = (Quaternion)stream.ReceiveNext();
		}
	}
}
