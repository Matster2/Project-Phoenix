using UnityEngine;
using System.Collections;

public class KnockTrigger : MonoBehaviour {
    public Player player;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            player.enemiesInKnockTrigger.Add(other.transform);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            player.enemiesInKnockTrigger.Remove(other.transform);
        }
    }
}
