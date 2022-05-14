using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP_Manager : MonoBehaviour
{
    public Slider slider;
    private float MAXHP = 100.0f;
    private float HP;

    void Start()
    {
        HP = MAXHP;
        slider.value = HP;
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
