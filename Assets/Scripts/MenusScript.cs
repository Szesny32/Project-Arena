using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenusScript : MonoBehaviour
{
    public GameObject tabMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Tab)){
            tabMenu.gameObject.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.Tab)){
            tabMenu.gameObject.SetActive(true);
        }
        
        
    }
}
