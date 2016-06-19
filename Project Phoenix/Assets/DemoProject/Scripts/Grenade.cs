using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Grenade : Photon.MonoBehaviour 
{
	public int damage;
	public float lifeTime = 3f;
	public float dropForce = 50f;
	public float rad;
    public GameObject explosion;
	public List<GameObject> enemies;

	private Vector3 correctPlayerPos;
	private Quaternion correctPlayerRot;

	// Use this for initialization
	void Start () 
	{
		this.GetComponent<SphereCollider>().radius = rad;
		this.GetComponent<Rigidbody>().velocity = transform.forward * dropForce;//new Vector3(0,kickForce,kickForce);
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Enemy")
		{
			enemies.Add(other.gameObject);
		}
	}

	void Update()
	{
		lifeTime -= Time.deltaTime;
		if(lifeTime<=0)
		{
			photonView.RPC("Explosion",PhotonTargets.All,damage);
		}

	}

	[PunRPC]
	void Explosion(int dam)
	{
		foreach(GameObject enemy in enemies)
		{
			enemy.SendMessage("Hit",dam);
		}
        PhotonNetwork.Instantiate(explosion.name,transform.position,transform.rotation,0);
		PhotonNetwork.Destroy(gameObject);
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
			this.correctPlayerPos = (Vector3)stream.ReceiveNext();
			this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
		}
	}
}
