//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class EnemySpawner : Photon.MonoBehaviour 
//{
//    public bool activated = false;
//    private bool ready = true;
//    private bool canSpawn = false;
//	public Enemy enemy;
//
//	public int minCount = 5;
//	public int maxCount = 20;
//    private int count = 0;
//    private int spawnedCount = 0;
//
//    public float reloadTime = 5;
//    private float m_reloadTime = 60;
//
//	public float spawnAccuracy = 5;//Randomation spawn positions
//
//	public float spawnSpeed; //Uploading Speed
//	public float activateTime;//When == 0 => actiovate Bots
//    private float m_activateTime;
//
//	public Transform spawnPos;
//	public float surfaceHeight;
//
//
//	public List<GameObject> spawnedBots;
//
//    public Renderer signal;
//
//    private float m_repeatTimer = 0;
//    
//
//    void FixedUpdate()
//    {
//        if(activated)
//        {
//            signal.material.SetColor("_Color",Color.green);
//
//            if(ready)
//            {
//
//                if(!canSpawn)
//                m_reloadTime+=Time.deltaTime;
//
//                if(m_reloadTime>=reloadTime)
//                {
//                    int dice = Random.Range(1,2);
//                    if(dice == 1)
//                    {
//                        count = Random.Range(minCount,maxCount);
//                        ready = false;
//                        canSpawn = true;  
//                    }
//                    else
//                    {
//                        m_activateTime = 0;
//                        m_reloadTime = 0;
//                        spawnedCount = 0;
//                        ready = true;
//
//                    }
//                }
//            }
//
//
//        }
//        else
//        {
//            signal.material.SetColor("_Color",Color.red);
//        }
//        if(!ready)
//        {
//            Spawn();
//        }
//
//    }
//
//    void Spawn ()
//    {
//        //Spawn Underground
//        if(spawnedCount<count)
//        {
//            Vector3 spawnVector = new Vector3(spawnPos.position.x + Random.Range(-spawnAccuracy,spawnAccuracy),spawnPos.position.y,spawnPos.position.z + Random.Range(-spawnAccuracy,spawnAccuracy));
//            GameObject m_enemy = PhotonNetwork.Instantiate(enemy.name,spawnVector,spawnPos.rotation,0) as GameObject;
//
//            m_enemy.GetComponent<Enemy>().enabled = false;
//            m_enemy.GetComponent<Enemy>().spawner = this;
//            m_enemy.GetComponent<NavMeshAgent>().enabled = false;
//
//            spawnedBots.Add(m_enemy);
//
//            spawnedCount ++;
//        }
//        else//Translate to surface
//        {
//
//            canSpawn = false;
//
//            m_activateTime +=Time.deltaTime;
//            if(m_activateTime < activateTime)
//            {
//                foreach(GameObject m_enemy in spawnedBots)
//                {
//                    m_enemy.transform.Translate(Vector3.up * spawnSpeed * Time.deltaTime);
//                }
//
//            }
//            else
//            {
//                foreach(GameObject m_enemy in spawnedBots)
//                {
//                    m_enemy.GetComponent<Enemy>().enabled = true;
//                    m_enemy.GetComponent<NavMeshAgent>().enabled = true;
//                }
//                
//                
//                
//                m_activateTime = 0;
//                m_reloadTime = 0;
//                spawnedCount = 0;
//                ready = true;
//            }
//        }
//    }
//        
//   
//
//}
