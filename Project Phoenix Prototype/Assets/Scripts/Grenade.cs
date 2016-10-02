using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grenade : MonoBehaviour {
	
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
			Explosion (damage);
		}

	}
		
	void Explosion(int dam)
	{
		foreach(GameObject enemy in enemies)
		{
			enemy.SendMessage("Hit",dam);
		}
        Instantiate(explosion ,transform.position ,transform.rotation);
		Destroy(gameObject);
	}
}