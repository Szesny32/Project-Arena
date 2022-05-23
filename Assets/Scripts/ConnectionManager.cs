using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Transports.UNET;

public class ConnectionManager : MonoBehaviour
{
    public GameObject connectButtonPanel;
    public string ipAddress = "127.0.0.1";

    UNetTransport transport;
    public AudioSource audioSource;
    private AudioClip glitchClip;
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
    }
    public void Host() {

        connectButtonPanel.SetActive(false);
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        audioSource.Stop();
    }
    public void Client() {

        transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        transport.ConnectAddress = ipAddress;
        connectButtonPanel.SetActive(false);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes("Password123");
        NetworkManager.Singleton.StartClient();
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback) {

        bool approve = System.Text.Encoding.UTF8.GetString(connectionData).Equals("Password123");
        callback(true, null, approve, GetRandomSpawnLocation(), Quaternion.identity);

    }

    Vector3 GetRandomSpawnLocation() {

        return new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, UnityEngine.Random.Range(-5f, 5f));
    }

    public void AddressChanged(string address) {

        this.ipAddress = address;

    }
}
