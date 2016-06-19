using UnityEngine;
using System.Collections;

public class HollowShield : MonoBehaviour {
    public float armor = 100;
    public bool getDam = false;
    public float time;
    public Renderer shield;
    public Color normalColor;
    public Color damageColor;
    public Color critColor;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(Input.GetMouseButtonDown(0))
        {
            armor -= 10;
            time = 0;
            getDam = true;
        }

    	if(getDam)
        {
          
            time += Time.deltaTime;
            if(armor>40)
                shield.material.color = Color.Lerp(damageColor,normalColor,time);
            if(armor<40)
                shield.material.color = Color.Lerp(critColor,normalColor,time);

            if(time > 1)
            {
                getDam = false;
            }
        }
	}
}
