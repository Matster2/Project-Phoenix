using UnityEngine;
using System.Collections;

public class Projectile : Photon.MonoBehaviour {
	public string bulletFrom;
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
		photonView.RPC("From",PhotonTargets.AllBuffered,bulletFrom);
	}

    [PunRPC]
    void From(string from)
	{
	   bulletFrom = from;
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


	void OnCollisionEnter(Collision other)
	{
		if(other.collider.tag == "Enemy")
		{
			other.collider.SendMessage("Hit",damage,SendMessageOptions.DontRequireReceiver);
			other.collider.SendMessage("DamageFrom",bulletFrom,SendMessageOptions.DontRequireReceiver);
		
			//PhotonNetwork.Instantiate(deadEffect.name,transform.position,transform.rotation,0);
			Destroy(gameObject);
		}
        else
        {
            //Vector3 dir = (other.gameObject.transform.position - gameObject.transform.position).normalized;
            PhotonNetwork.Instantiate(deadEffect.name,transform.position,Quaternion.Euler(new Vector3(-90,0,0)),0);
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
