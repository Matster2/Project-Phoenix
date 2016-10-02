/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using RootMotion.FinalIK;
using UnityEngine.UI;
public class Player : Photon.MonoBehaviour {
	//SetUp Health
    [Header("Health Settings")]
	public float health = 100;
	public float maxHealth = 100;
    [Space(10)]
    [Header("Armor Settings")]
	//SetUpArmor
	public float armor = 100;
	public float maxArmor = 100;

    [Space(10)]
	//Regen Values
	public float regenTimeout = 10f;
	public float regenSpeed = 0.5f;

    [Space(10)]
    [Header("Shield Settings")]

    public float shield = 100;
    public float maxShield = 100;

    public float sregenTimeout = 10f;
    public float sregenSpeed = 0.5f;




    [Space(10)]
    [Header("Movement Settings")]
	//Movement
	public float moveSpeed = 3.0f;
	public float runSpeed = 8.0f;
	public float rotSpeed = 30.0f;

    public float _MaxPover = 100;
    public float _MinPeakUsePower = 35;
    public float _RecoverySpeed = 5;
    public float _DecreaseSpeed = 5;
    
    float _Power = 100;
    float _StartPower = 0;
    bool _PressKeyRun = false;
    [Space(10)]
    [Header("Weapon Settings")]
	//Weapons**
	public Transform shootPoint;
	public AudioClip shootSound;
    [Space(10)]
	//Bullet**
	public float attactRate = 0.1f;
	public Projectile bullet;
	public int damage;

    [Space(10)]
   	//Grenade**
	public float dropRate = 2.0f;
	public float dropForce = 20f;
    public float dropForceMax = 35;
	public Grenade grenade;
	public float explosionRadius;
	public int explosionDamage;
    [Space(10)]
    //Melee ***
    public SphereCollider knockTrigger;
    public float knockForce;
    public float knockDamage;

    public List<Transform> enemiesInKnockTrigger;
    [Space(10)]

    [Header("Other Settings")]

    [Space(10)]

    //Audio
    public AudioSource heavyBreathing;
    public AudioClip damageSound;
 
    public AudioSource heartBeat;

    public AudioClip footstepSound;

    public float walkStepInterval = 0.2f;
    public float runStepInterval = 0.1f;

    private float currentStepInterval;

    private float lastStepTime = -10.0f;


    //Shield
    [Header("Hollow Shield Settings")]

    public Renderer r_shield;
    private float hollowTime = 0;//0-1
    private bool activeShield = false;
    public Color normalColor = Color.white;
    public Color damageColor = Color.cyan;
    public Color critColor = Color.red;


	//DeadModel
	public GameObject deadModel;
	//AIM Components **
    [Space(10)]

    [Header("InverseKinematic Settings")]

	public AimIK aim;
	public FullBodyBipedIK ik;

	public Vector3 gunHoldOffset;
	public Vector3 leftHandOffset;

	public GameObject my_camera;

	public Transform aimTarget;
	[Range(0f, 1f)] public float headLookWeight = 1f;

	public Transform cameraLookPos;
	public Animator animController;

	private bool roll = false;

	private Vector3 leftHandPosRelToRightHand;
	private Quaternion leftHandRotRelToRightHand;
	private Vector3 headLookAxis;

	private PlayerCamera camera;
	private float m_LastShot = -10.0f;

	//Stream variables***
	private Vector3 correctPlayerPos;
	private Quaternion correctPlayerRot;
	private Vector3 correctAimPos;


	
	//Graphics
	private GameObject UIContainer;

	private Image bloodScreen;
	private float alpha;
	
	private Image crosshair;
	private float crossSize;

	private Slider healthBar;
	private Slider armorBar;
    private Slider forceBar;
    private Slider shieldBar;

	private float lastDamTime = 0;
    
    
    //***************************************************************************************************************************************************************************************************************
    
	// Use this for initialization
	void Start () 
	{
		//DiableCursor
	   Cursor.lockState = CursorLockMode.Locked;
	   Cursor.visible = false;
	   //aim.Disable();
	   // ik.Disable();

	   headLookAxis = ik.references.head.InverseTransformVector(ik.references.root.forward);

	   animController.SetLayerWeight(1, 1f);

	   if(photonView.isMine)
	    {
			//name += " ID : " + PhotonNetwork.playerName;
			photonView.RPC("UpdateName",PhotonTargets.AllBuffered,PhotonNetwork.playerName);
			camera = Camera.main.GetComponent<PlayerCamera>();
			camera.LookAt = this.transform;
			camera.MCamera = Camera.main.transform;
			camera.CameraObject = Camera.main;
			camera.AimTarget = cameraLookPos;
			camera.Player = this.transform;

			aimTarget.parent = null;

			//SetUIContainer
			UIContainer = GameObject.Find("Canvas");

			//Set BloodScreen
			bloodScreen = UIContainer.transform.FindChild("BloodScreen").GetComponent<Image>();
			bloodScreen.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,Screen.width);
			bloodScreen.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,Screen.height);

			//Set Crosshair
			crosshair = GameObject.Find("Crosshair").GetComponent<Image>();

			//SetHealthBar
			healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();
			healthBar.maxValue = maxHealth;
			healthBar.value = health;
            
            //SetArmorBar
            armorBar = GameObject.Find("ArmorBar").GetComponent<Slider>();
            armorBar.maxValue = maxArmor;
            armorBar.value = armor;

            //SetForceBar
            forceBar = GameObject.Find("ForceBar").GetComponent<Slider>();
            forceBar.maxValue = _MaxPover;
            forceBar.value = _Power;

            //SetShieldBar
            shieldBar = GameObject.Find("ShieldBar").GetComponent<Slider>();
            shieldBar.maxValue = maxShield;
            shieldBar.value = shield;
	    }

	}

	[PunRPC]
	void UpdateName(string m_name)
	{
		name = "Player ID : " + m_name;
	}

	Vector3 hitPos; //Hit pos from center camera
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(photonView.isMine)
		{
			ControlLocally();
		}
        //HollowShield **
        
        if(activeShield)
        {
            HollowShield();
        }
	}

	void ControlLocally()
	{
		//Mecanim 
		Read();
		
		// AimIK pass
		AimIK();
		
		// FBBIK pass - put the left hand back to where it was relative to the right hand before AimIK solved
		FBBIK();

		HeadLookAt(aimTarget.position);


		//MOVEMENT
		if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
		{
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,Camera.main.transform.eulerAngles.y,transform.localEulerAngles.z);
			animController.SetBool("IsMoving",true);

            if(Input.GetKey(KeyCode.LeftShift))
            {
                currentStepInterval = runStepInterval;
            }else
            {
                currentStepInterval = walkStepInterval;
            }
            if(Time.time>currentStepInterval + lastStepTime)
            {
                this.GetComponent<AudioSource>().PlayOneShot(footstepSound);
                lastStepTime = Time.time;
            }


		}
		else
		{
			animController.SetBool("IsMoving",false);
		}
        if(health>20)
        {
		transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime,Space.Self);
		transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime,Space.Self);
        }
        if(health<20)
        {

        }
		animController.SetFloat("Z",Input.GetAxis("Vertical"));
		animController.SetFloat("X",Input.GetAxis("Horizontal"));
 
        //RUNNING
        forceBar.value = _Power;

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            _StartPower = _Power;
        }
        if(Input.GetKey(KeyCode.LeftShift))
		{
            _PressKeyRun = true;
        }
        else
        {
            _PressKeyRun = false;
        }

        if(_PressKeyRun == true)
        {
            if( _StartPower > _MinPeakUsePower && _Power > 0 )
            {
                _Power = Mathf.MoveTowards(_Power, 0, Time.fixedDeltaTime*_DecreaseSpeed);
                transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * runSpeed * Time.deltaTime,Space.Self);
                transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * runSpeed * Time.deltaTime,Space.Self);
                
                animController.SetFloat("Z",Input.GetAxis("Vertical") * 2);
                animController.SetFloat("X",Input.GetAxis("Horizontal") * 2);
            }
            else
            {
                 
                _Power = Mathf.MoveTowards(_Power, _MaxPover, Time.fixedDeltaTime*_RecoverySpeed);
            }
            
            if(_Power > _MinPeakUsePower && _StartPower <= _MinPeakUsePower)
            {
                _StartPower = _Power;
            }
        }
        if(_Power<_MinPeakUsePower)
        {
            heavyBreathing.volume+=Time.deltaTime;
        }
        else
        {
            heavyBreathing.volume-=Time.deltaTime;
        }
        if(_PressKeyRun == false)
        {
            _Power = Mathf.MoveTowards(_Power, _MaxPover, Time.fixedDeltaTime*_RecoverySpeed);
        }


		//Rotation
		RaycastHit hit;
		Ray ray =  Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
		if (Physics.Raycast(ray,out hit)) 
		{
			hitPos = hit.point;
			hitPos = new Vector3(hitPos.x,transform.position.y,hitPos.z);
			aimTarget.position = hit.point;

		}
		
		var targetRotation = Quaternion.LookRotation(hitPos - transform.position);
		
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);

		//WeaponControl

		//Shoot
		if(Input.GetMouseButton(0))
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			Shoot();
		}
		//DropGrenade
		if(Input.GetKey(KeyCode.G))
		{
            dropForce+=Time.deltaTime * 10;
            if(dropForce>dropForceMax)
            {
                dropForce = dropForceMax;
            }
		}
        if(Input.GetKeyUp(KeyCode.G))
        {
            //dropForce *= dropForceX;
            DropGrenade();
            dropForce = 0;
           // dropForceX = 0;
        }
        //Melee Attack
        if(Input.GetKeyDown(KeyCode.C))
        {
            MeleeAttack();
        }

		//Zoom
		if(Input.GetMouseButton(1))
		{
			camera.IsAimed = true;
		}
		else
		{
			camera.IsAimed = false;
		}

		//BloodScreen
		if(bloodScreen.color.a > 0)
		{
			alpha -= Time.deltaTime;
			bloodScreen.color = new Color(1,1,1,alpha);
		}

		//Crosshair **

		//Set Min Size
		if(crossSize>30)
		{
			crossSize -= 150 * Time.deltaTime;
			crosshair.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,crossSize);
			crosshair.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,crossSize);
		}

		//Set MaxSize
		if(crossSize>60)
			crossSize = 60;


		//Regen
		if(health<maxHealth)
		{
            if(health<50)
            {
                heartBeat.volume+=Time.deltaTime;
            }
            else
            {
                heartBeat.volume-=Time.deltaTime;
            }
			if(Time.time > lastDamTime + regenTimeout)
			{
		
				health += regenSpeed;
				healthBar.value = health;
			}
		}

        if(shield<maxShield)
        {    
            if(Time.time > lastDamTime + sregenTimeout)
            {
                shieldBar.value = shield;
                shield += sregenSpeed;
            }
           
        }



	}
    //***********************************************************************************************************************************
	private void Read() {
//		Debug.Log ("Read");
		// Remember the position and rotation of the left hand relative to the right hand
		leftHandPosRelToRightHand = ik.references.rightHand.InverseTransformPoint(ik.references.leftHand.position);
		leftHandRotRelToRightHand = Quaternion.Inverse(ik.references.rightHand.rotation) * ik.references.leftHand.rotation;
	}
	
	private void AimIK() {
		//Debug.Log("AimIK");
		// Set AimIK target position and update
		aim.solver.IKPosition = aimTarget.position;
		aim.solver.Update(); // Update AimIK
	}
	
	// Positioning the left hand on the gun after aiming has finished
	private void FBBIK() {
		//Debug.Log("FBBIK");
		// Store the current rotation of the right hand
		Quaternion rightHandRotation = ik.references.rightHand.rotation;
		
		// Put the left hand back to where it was relative to the right hand before AimIK solved
		Vector3 leftHandTarget = ik.references.rightHand.TransformPoint(leftHandPosRelToRightHand);
		ik.solver.leftHandEffector.positionOffset += leftHandTarget - ik.references.leftHand.position;
		
		// Offsetting hands, you might need that to support multiple weapons with the same aiming pose
		Vector3 rightHandOffset = ik.references.rightHand.rotation * gunHoldOffset;
		ik.solver.rightHandEffector.positionOffset += rightHandOffset;
		ik.solver.leftHandEffector.positionOffset += rightHandOffset + ik.references.rightHand.rotation * leftHandOffset;
		
		// Update FBBIK
		ik.solver.Update();
		
		// Rotating the hand bones after IK has finished
		ik.references.rightHand.rotation = rightHandRotation;
		ik.references.leftHand.rotation = rightHandRotation * leftHandRotRelToRightHand;
	}

	private void HeadLookAt(Vector3 lookAtTarget) {
		Quaternion headRotationTarget = Quaternion.FromToRotation(ik.references.head.rotation * headLookAxis, lookAtTarget - ik.references.head.position);
		ik.references.head.rotation = Quaternion.Lerp(Quaternion.identity, headRotationTarget, headLookWeight) * ik.references.head.rotation;
	}

    //****************************************************************************************************************************************************************************************************************

	void Shoot()
	{
		if (Time.time > attactRate + m_LastShot) 
		{
            shootPoint.localEulerAngles = new Vector3(0,90,0);
			crossSize += 30f;

			photonView.RPC("PlayShootSound",PhotonTargets.All);
			GameObject inst = PhotonNetwork.Instantiate(bullet.name,shootPoint.position,shootPoint.rotation,0) as GameObject;
			Projectile proj = inst.GetComponent<Projectile>();
			proj.bulletFrom = gameObject.name;
			proj.damage = damage;
			m_LastShot = Time.time;
		}
	}

	void DropGrenade()
	{
		if (Time.time > dropRate + m_LastShot) 
		{
            shootPoint.eulerAngles = Camera.main.transform.eulerAngles;
       
			GameObject inst = PhotonNetwork.Instantiate(grenade.name,shootPoint.position,shootPoint.rotation,0) as GameObject;
			Grenade gren = inst.GetComponent<Grenade>();
			gren.dropForce = dropForce;
			gren.damage = explosionDamage;
			gren.rad = explosionRadius;
			//gren.
			m_LastShot = Time.time;
		}
	}
    
    void MeleeAttack()
    {
        foreach(Transform m_enemy in enemiesInKnockTrigger)
        {
            Vector3 dir = m_enemy.position - transform.position;
            dir.y = 0;
            m_enemy.SendMessage("Hit",knockDamage);
            m_enemy.position += dir * knockForce;
        }
    }

	[PunRPC]
	void PlayShootSound()
	{
		this.GetComponent<AudioSource>().PlayOneShot(shootSound);
	}

	void Damage(int m_damage)
	{
        if(shield>0)
        {
            shield -= m_damage;
            shieldBar.value = shield;
        }
        if(shield<=0)
        {
            armor -= 5;
            if(armor<100 && armor>75)
            {
                health -= m_damage/2;   
            }
            if(armor<75 && armor>50)
            {
                health -= (m_damage * 2)/3;   
            }
            if(armor<50 && armor>25)
                health -= (m_damage * 3)/2;   
            if(armor<25)
                health -= m_damage;   

            if(armor<10)
                armor = 10;
            if(photonView.isMine)
             {
                alpha = 1;
                bloodScreen.color = new Color(1,1,1,alpha);
             }
        }



		photonView.RPC("BroadcastDamage",PhotonTargets.All,m_damage);
	}

	[PunRPC]
	void BroadcastDamage(int m_damage)
	{
        if(shield>0)
        {
            shield -= m_damage;
            hollowTime = 0;
            activeShield = true;
        }
        if(shield<0)
        {
            //play Dam sound
            this.GetComponent<AudioSource>().PlayOneShot(damageSound);

            armor -= 5;
            if(armor<100 && armor>75)
            {
                health -= m_damage/2;   
            }
            if(armor<75 && armor>50)
            {
                health -= (m_damage * 2)/3;   
            }
            if(armor<50 && armor>25)
                health -= (m_damage * 3)/2;   
            if(armor<25)
                health -= m_damage;   
            
            if(armor<10)
                armor = 10;
            if(photonView.isMine)
            {
                alpha = 1;
                bloodScreen.color = new Color(1,1,1,alpha);
            }
        }

		lastDamTime = Time.time;


		if(photonView.isMine)
		{

			healthBar.value = health;
            armorBar.value = armor;
            
			if(health <=0)
			{
					PhotonNetwork.Instantiate(deadModel.name,transform.position,transform.rotation,0);
					PhotonNetwork.Destroy(gameObject);
			}
		}
	}

    void HollowShield()
    {
        hollowTime += Time.deltaTime;
        if(shield>40)
            r_shield.material.color = Color.Lerp(damageColor,normalColor,hollowTime);
        if(shield<40)
            r_shield.material.color = Color.Lerp(critColor,normalColor,hollowTime);
        
        if(hollowTime > 1)
        {
            hollowTime = 0;
            activeShield = false;
        }
    }

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(aimTarget.position);
		}
		else
		{
			// Network player, receive data
			this.correctPlayerPos = (Vector3)stream.ReceiveNext();
			this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
			this.correctAimPos = (Vector3)stream.ReceiveNext();
		}
	}
}
*/