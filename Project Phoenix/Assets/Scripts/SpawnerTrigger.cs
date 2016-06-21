using UnityEngine;
using System.Collections;

public class SpawnerTrigger : MonoBehaviour {
	public EnemySpawner spawner;

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
            spawner.activated = true;
		}
	}
    
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            spawner.activated = false;
        }
    }
}
