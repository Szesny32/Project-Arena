using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_Manager : MonoBehaviour
{
    private float HP = 100.0f;
    private float MAXHP = 100.0f;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public float getHP()
    {
        return HP;
    }
    public void takeDamage(float DMG)
    {
        HP-=DMG;
        Debug.Log(this.name+" has taken: "+DMG+" HP! Now has HP:"+HP);
    }
}
