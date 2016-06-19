using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Behavior {Born,Idle,Walk,Attack,Dead}


public class Enemy : Photon.MonoBehaviour {


    public bool isBoss = false;

	public bool targetIsPlayer = false;
	public Transform target;

	public List<Transform> targets;//Player who in detector;
	public bool targetLocked = false; // if enemy select target from targets, value = true;
	public float timeToUpdateTarget = 5.0f;
	
	public float detectorRadius = 6.0f;

	//Behavior
	public Behavior m_behavior = Behavior.Idle;
	//Animation
	public Animator animController;

	//AttackSound
	public AudioClip attackSound;

	//MainSettings
    public bool canShoot;

	public int armor = 100;
	public int meleeDamage = 15;

    public int poisonBallDamage = 5;
    public Transform shootPoint;
    public float shotDistance = 10f;
    public float attactRate = 0.1f;
    public PoisonBall ball;
    
	public float moveSpeed = 3.5f;
	public float attackDistance = 1.5f;
	public float attackRate = 1.0f;

    public float unTargetTime = 5;
    public float despawnTime = 5;
    private float m_despawnTime = 0;

	public GameObject deadModel;

	private NavMeshAgent agent;
	private EnemyPath path;

	private float lastAttack = -10.0f;
	private float lastTime = -10.0f;


	private GameManager manager;
	//[HideInInspector]
	public EnemySpawner spawner;

    public PlayerDetector detector;

    private float m_LastShot = -10.0f;
    private Vector3 correctEnemyPos;
	private Quaternion correctEnemyRot;
	public string killerName;
	// Use this for initialization

	void Start ()
	{
		agent = this.GetComponent<NavMeshAgent>();
		path = (EnemyPath)GameObject.FindObjectOfType(typeof (EnemyPath));
		target = path.points[Random.Range(0,path.points.Count)];
		//spawner = (EnemySpawner)GameObject.FindObjectOfType(typeof (EnemySpawner));
		manager = (GameManager)GameObject.FindObjectOfType(typeof (GameManager));
       
		detector.GetComponent<SphereCollider>().radius = detectorRadius;
        if(!isBoss)
        {
            canShoot = false;
            int dice = Random.Range(1,3);
            if(dice == 1)
            {
                canShoot = true;
            }
            else
            {
                canShoot = false;
            }
        }
        detector.enabled = true;
		m_behavior = Behavior.Walk;

        if(isBoss)
        {
            attackDistance = attackDistance*2;
            armor = armor*2;
            transform.localScale = new Vector3(2,2,2);
            meleeDamage = meleeDamage*2;
            moveSpeed = moveSpeed*2;
            poisonBallDamage = poisonBallDamage*2;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
        //Check and Set Behavior if its bot IsMine
		if(photonView.isMine)
		CheckBehavior();

		if(target)
        {
            if(m_behavior != Behavior.Dead)
	             agent.SetDestination(target.position);


        }

		if(targets.Count>0)
		{
			if(!targetLocked)
			{
				target = targets[Random.Range(0,targets.Count)];
				targetLocked = true;
				targetIsPlayer = true;
			}
		}

        if(!targetIsPlayer)
        {
            unTargetTime -= Time.deltaTime;
            if(unTargetTime <=0)
            {
                m_behavior = Behavior.Dead;
            }
        }

	}

	void CheckBehavior()
	{
		//Checking Behavior
		if(m_behavior != m_behavior)
			photonView.RPC("BroadcastBehavior",PhotonTargets.AllBuffered,m_behavior.ToString());

		if(m_behavior == Behavior.Idle)
		{
			Idle();
		}
		if(m_behavior == Behavior.Walk)
		{
			Walk();
		}
        if(m_behavior == Behavior.Dead)
        {
            agent.speed = 0;
            agent.enabled = false;
            transform.Translate(Vector3.down * 3 * Time.deltaTime);
            m_despawnTime += Time.deltaTime;
            if(m_despawnTime> despawnTime)
            {
                spawner.spawnedBots.Remove(gameObject);
                PhotonNetwork.Destroy(gameObject);
            }
        }
	
	}
	//Read And Set Behavior if bot not our
	[PunRPC]
	void BroadcastBehavior(string s_behavior)
	{
		if(s_behavior == "Idle")
		{
			Idle();
		}
		if(s_behavior == "Walk")
		{
			Walk();
		}
	}

	//Basic Behavior
	void Idle()
	{
		agent.speed = 0;

		MecAnimContol(0,false);
	}
	void Walk()
	{
        /*if(jump)
        {
            agent.enabled = false;
            this.GetComponent<Rigidbody>().velocity = new Vector3(0,transform.position.y,transform.position.z) * jumpforce;
            jump = false;
        }*/


		agent.speed = moveSpeed;
		//animController.SetInteger("Speed",1);
		MecAnimContol(1,false);
		//If Enemy have target
		if(target)
		{
			if(targetIsPlayer) //if target its player
			{
                /*float distance = Vector3.Distance(transform.position,target.position);
                Vector3 lookPos = target.position;
                lookPos.y = transform.position.y;
                transform.LookAt(lookPos);

                if(distance<40 && distance>15)
                {
                    agent.speed = 0;
                    DistanceAttack();
                }
                if(distance<15)
                {
                    if(distance<attackDistance)
                    {
                        agent.speed = 0;
                        Attack(); 
                    }
                    else
                    {
                        agent.speed = moveSpeed;
                    }
                }
                */
                Vector3 lookPos = target.position;
                lookPos.y = transform.position.y;
                transform.LookAt(lookPos);

                if(!canShoot)
                {
    				if(Vector3.Distance(transform.position,target.position)<= attackDistance) //if distance
    				{
    					agent.speed = 0;
    					Attack(); // attack
    				}
    				else
    				{
    					agent.speed = moveSpeed;
    				}
                }
                else
                {
                    if(Vector3.Distance(transform.position,target.position)<= shotDistance) //if distance
                    {
                        agent.speed = 0;
                        DistanceAttack(); // attack
                    }
                    else
                    {
                        agent.speed = moveSpeed;
                    }
                }
			}
		}
		else //if enemy have no target (target == null)
		{		
			//Debug.Log ("PlayerKill" + target.name); //Enemy kill player
			targets.Remove(target); //delete player from list
			target = path.points[Random.Range(0,path.points.Count)]; // return to waypoints

			targetIsPlayer = false;
			targetLocked = false;
		}

	}

	void Attack()
	{
		if(Time.time > attackRate + lastAttack)
		{
			agent.speed = 0;
			MecAnimContol(0,true);
            target.SendMessage("Damage",meleeDamage);
			this.GetComponent<AudioSource>().PlayOneShot(attackSound);
			lastAttack = Time.time;
		}
	}

    void DistanceAttack()
    {
        if(Time.time > attackRate + lastAttack)
        {
            agent.speed = 0;
            MecAnimContol(0,true);
            shootPoint.LookAt(target.position);
            GameObject inst = PhotonNetwork.Instantiate(ball.name,shootPoint.position,shootPoint.rotation,0) as GameObject;
            PoisonBall m_ball = inst.GetComponent<PoisonBall>();
            m_ball.damage = poisonBallDamage;

			lastAttack = Time.time;
        }
    }

	//Set Animator Values
	void MecAnimContol(int CharacterSpeed,bool AttackAnim)
	{
		//Debug.Log (CharacterSpeed);
		animController.SetInteger("Speed",CharacterSpeed);
		animController.SetBool("Attack",AttackAnim);

	}

	//If bot see player => seek to him
	public void PlayerDetected(GameObject player)
	{
		targetIsPlayer = true;
		target = player.transform;
	}

	//Get Damage
	private bool isDead = false; //without it variable dead meshes spawn more than one time!

	void Hit(int damage)
	{
		armor -= damage;
		photonView.RPC("BroadcastDamage",PhotonTargets.All,damage);

	}



	void DamageFrom(string from)
	{
		killerName = from;
		target = GameObject.Find(from).transform;

		targetIsPlayer = true;
	}


	//Broadcast Damage
	[PunRPC]
	void BroadcastDamage(int damage)
	{
		if(photonView.isMine)
		{
		armor -= damage;
        if(manager)
		manager.PlayerHitMonster(killerName);
		if(!isDead)
			if(armor<=0)
	       	{

				manager.PlayerKillMonster(killerName);

				spawner.spawnedBots.Remove(gameObject);
                GameObject instDeadMode = PhotonNetwork.Instantiate(deadModel.name,transform.position,transform.rotation,0);
                if(isBoss)
                {
                    instDeadMode.transform.localScale = new Vector3(2,2,2);
                }
				PhotonNetwork.Destroy(gameObject);
				isDead = true;
		    }
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
			this.correctEnemyPos = (Vector3)stream.ReceiveNext();
			this.correctEnemyRot = (Quaternion)stream.ReceiveNext();
		}
	}
}
