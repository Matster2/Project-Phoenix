//using UnityEngine;
//using System.Collections;
//
//public class PlayerDetector : MonoBehaviour {
//	private Enemy myParent;
//	void OnEnable () 
//	{
//		myParent = this.transform.parent.GetComponent<Enemy>();
//	}
//	
//	void OnTriggerStay(Collider other)
//	{
//		if(other.GetComponent<Collider>().tag == "Player")
//		{
//            if(myParent)
//			myParent.targets.Add(other.transform);
//		}
//	}
//	void OnTriggerExit(Collider other)
//	{
//		if(other.GetComponent<Collider>().tag == "Player")
//		{
//            if(myParent)
//			myParent.targets.Remove(other.transform);
//		}
//	
//    }
//}
