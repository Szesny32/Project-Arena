using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannonScript : MonoBehaviour
{
    // Start is called before the first frame update
    private float reloadTime = 2.5f;
    private float timmer =0.0f;
    void Start()
    {
        timmer = reloadTime;
    }

    // Update is called once per frame
    void Update()
    {
        timmer-=Time.deltaTime;
        if(timmer<=0.0)
            shoot();


    }


    void shoot()
    {
        timmer=reloadTime;   
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction*5.0f, Color.red, 0.2f);   
        RaycastHit hit;

        {
            if(Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                HP_Manager HP = hit.transform.gameObject.GetComponent<HP_Manager>();
                if(HP)
                    HP.takeDamage(25.0f);
                }  
        }
    }

}
