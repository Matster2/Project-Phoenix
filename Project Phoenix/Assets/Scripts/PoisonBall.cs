using UnityEngine;
using System.Collections;

public class PoisonBall : Photon.MonoBehaviour {
	public int damage = 10;
	public float lifeTime = 2f;
	public float speed = 60f;
	public GameObject deadEffect;

	private Vector3 correctBulletPos;
	private Quaternion correctBulletRot;

	// Use this for initialization
	void Start () 
	{
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x /*+ Random.Range(-1,1)*/,transform.localEulerAngles.y /*+ Random.Range(-1,1)*/,transform.localEulerAngles.z);
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
		lifeTime -= Time.deltaTime;
		if(photonView.isMine)
		if(lifeTime<=0)
		{
			PhotonNetwork.Destroy(this.gameObject);
		}
	}


	void OnTriggerEnter(Collider other)
	{
		if(other.GetComponent<Collider>().tag == "Player")
		{
			other.GetComponent<Collider>().SendMessage("Damage",damage,SendMessageOptions.DontRequireReceiver);
			//PhotonNetwork.Instantiate(deadEffect.name,transform.position,transform.rotation,0);
			Destroy(gameObject);
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
			this.correctBulletPos = (Vector3)stream.ReceiveNext();
			this.correctBulletRot = (Quaternion)stream.ReceiveNext();
		}
	}
}
