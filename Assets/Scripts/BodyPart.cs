using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    [SerializeField] private HUD_Manager Health;
    [SerializeField] private float factor = 1.0f;


    public void inflictDamage(float DMG)
    {
        Health.takeDamageServerRpc(DMG * factor);
        Debug.Log($"{Time.frameCount} : {transform.gameObject.name} : HP - {DMG * factor}");

    }

}
