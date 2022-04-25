using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject modelPrefab;
    [SerializeField] private float mouseSensitivity = 200.0f;
    private GameObject model;
    private GameObject playerCamera;
    
    private Vector3 cameraRotation = Vector3.zero;
    [SerializeField] private float cameraMinRange = -30.0f;
    [SerializeField] private float cameraMaxRange = 60.0f;
    void Start()
    {
        setModel(modelPrefab);
        playerCamera = transform.Find("Camera").gameObject;
        Cursor.lockState = CursorLockMode.Locked;
        refreshCameraPosition();
    }

    void Update()
    {
    }
    private void setModel(GameObject prefab)
    {
        if(prefab != null)
        {
            model = Instantiate(prefab, this.transform);
            model.name = "Model";
            model.transform.localPosition = Vector3.zero;
        }
    }
    private void refreshCameraPosition()
    {
        if(model != null)
        {
            Vector3 headPoint = model.transform.Find("Hips/Spine/Spine1/Spine2/Neck/Head/HeadTop_End").position;
            Vector3 position = headPoint + new Vector3(0.0f, 0.1f, 0.12f) - transform.position;
            playerCamera.transform.localPosition = position;
            playerCamera.transform.localRotation = Quaternion.identity;
            cameraRotation = Vector3.zero;
        }
    }
    public void rotateCamera(Vector3 rotation)
    {
        cameraRotation += rotation;
        cameraRotation.x =  Mathf.Clamp(cameraRotation.x, cameraMinRange, cameraMaxRange);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotation);
    }
    public float MouseSenstivity()
    {
        return mouseSensitivity;
    }
}

