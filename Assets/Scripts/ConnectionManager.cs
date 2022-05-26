using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Transports.UNET;

public class ConnectionManager : MonoBehaviour
{

    public GameObject connectButtonPanel;
    public GameObject playerHUD;
    public string ipAddress = "127.0.0.1";

    UNetTransport transport;
    public AudioSource audioSource;
    public AudioListener audioListener;
    private AudioClip glitchClip;

    private float[] xStartPos = { -2.38f, 5.16f, 5.71f, -4.507f, -2.21f, 1.12f};
    private float[] yStartPos = { 0f, 0f,0f,0f,1.432f, 1.432f};
    private float[] zStartPos = { -2.55f, -2.63f,8.78f, 8.299f, 2.302f, 1.71f};

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        glitchClip = Resources.Load<AudioClip>("Glitch");       
        if(!audioSource.isPlaying)
        {
            audioSource.clip = glitchClip;
            audioSource.Play();
        }

        playerHUD.SetActive(false);

    }

    public void Host() {

        connectButtonPanel.SetActive(false);
        playerHUD.SetActive(true);
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        audioSource.Stop();
        audioListener.enabled = false;
    }
    public void Client() {

        transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        transport.ConnectAddress = ipAddress;
        connectButtonPanel.SetActive(false);
        playerHUD.SetActive(true);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes("Password123");
        NetworkManager.Singleton.StartClient();
        audioSource.Stop();
        audioListener.enabled = false;
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback) {

        bool approve = System.Text.Encoding.UTF8.GetString(connectionData).Equals("Password123");
        callback(true, null, approve, GetRandomSpawnLocation(), Quaternion.identity);

    }

    Vector3 GetRandomSpawnLocation() {

        int spot = UnityEngine.Random.Range(0, 5);

        return new Vector3(xStartPos[spot] + UnityEngine.Random.Range(-0.5f, 0.5f), yStartPos[spot], zStartPos[spot]+UnityEngine.Random.Range(-0.5f, 0.5f));
    }

    public void AddressChanged(string address) {

        this.ipAddress = address;

    }
}
