using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;
public class GameManagerScript : NetworkBehaviour
{
    // Start is called before the first frame update

    private float roundTime = 300f;
    [SerializeField] private NetworkVariable<float> roundTimer =  new NetworkVariable<float>();
    [SerializeField] private NetworkVariable<int> round =  new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> players =  new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> playersInRedTeam =  new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> playersAliveInRedTeam =  new NetworkVariable<int>();

    [SerializeField] private NetworkVariable<int> playersInBlueTeam =  new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> playersAliveInBlueTeam =  new NetworkVariable<int>();

    [SerializeField] private NetworkVariable<int> blueTeamScore =  new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> redTeamScore =  new NetworkVariable<int>();

    [SerializeField] private NetworkVariable<FixedString64Bytes> blueTeamList =  new NetworkVariable<FixedString64Bytes>();
    [SerializeField] private NetworkVariable<FixedString64Bytes> redTeamList =  new NetworkVariable<FixedString64Bytes>();


    //do podłączenia textboxów z listą graczy dla poszczególnych teamów w menu pod Tab
    public TextMeshProUGUI TabMenuTeam1;
    public TextMeshProUGUI TabMenuTeam2;


    public TextMeshProUGUI HUD_roundTimer;

    int n = 0;
    void Start()
    {
         //Cursor.lockState = CursorLockMode.Locked;
         
    }
    public Texture _red, _blue;
    // Update is called once per frame


    void Update()
    {
      
        if(NetworkManager.IsServer)
        {
              update_RoundTimer_ServerRpc();
            //NetworkManager.ConnectedClientsList
            int tmp = NetworkManager.ConnectedClientsIds.Count;
            
            if(tmp!=n)
            {
                n = tmp;
                int x = n/2;
                int i = 0;
             
                foreach(ulong clientId in NetworkManager.ConnectedClientsIds)
                {
                    //GameObject playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject;
                    NetworkObject playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
                    int newTeam = i%2+1;
                    playerObj.GetComponent<PlayerManager>().UpdateTeamServerRpc(newTeam);

                 

                    // Texture team;
                    // if(i < x)
                    //     team = _red;
                    // else
                    //     team = _blue;

                        // playerObj.transform.Find("Model/Robot_Soldier_Arms1").GetComponent<Renderer>().material.SetTexture("_MainTex", team);
                        // playerObj.transform.Find("Model/Robot_Soldier_Arms2").GetComponent<Renderer>().material.SetTexture("_MainTex", team);
                        // playerObj.transform.Find("Model/Robot_Soldier_Body").GetComponent<Renderer>().material.SetTexture("_MainTex", team);
                        // playerObj.transform.Find("Model/Robot_Soldier_Feet").GetComponent<Renderer>().material.SetTexture("_MainTex", team);
                        // playerObj.transform.Find("Model/Robot_Soldier_Head").GetComponent<Renderer>().material.SetTexture("_MainTex", team);
                        // playerObj.transform.Find("Model/Robot_Soldier_Legs2").GetComponent<Renderer>().material.SetTexture("_MainTex", team);
                ++i; 
                }

            }
            refresh_playerList_ServerRpc();
        }

        float minutes = Mathf.Floor(roundTimer.Value/60);
        float seconds = Mathf.Floor(roundTimer.Value%60f);
        HUD_roundTimer.text = $"{minutes}:{seconds}";


        TabMenuTeam1.text = redTeamList.Value.ToString();
        TabMenuTeam2.text = blueTeamList.Value.ToString();


    }

    [ServerRpc]
    public void refresh_playerList_ServerRpc()
    {
         redTeamList.Value="";
          blueTeamList.Value="";
         
        foreach(ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            NetworkObject playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            if(playerObj.GetComponent<PlayerManager>().team.Value == 1)
                redTeamList.Value+=$"{playerObj.name}\n";
            
            else if(playerObj.GetComponent<PlayerManager>().team.Value == 2)
                blueTeamList.Value+=$"{playerObj.name}\n";
            
        }
    }


    [ServerRpc]
    public void update_RoundTimer_ServerRpc() {
        roundTimer.Value -=Time.deltaTime;
        if(roundTimer.Value < 0f)
        {
            roundTimer.Value = roundTime;
            round.Value++;
        }
    }
    [ServerRpc]
    public void restart_RoundTimer_ServerRpc() {
        roundTimer.Value = roundTime;
    }


}
