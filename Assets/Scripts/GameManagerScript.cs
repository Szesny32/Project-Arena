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

    public NetworkVariable<FixedString64Bytes> blueTeamList =  new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<FixedString64Bytes> redTeamList =  new NetworkVariable<FixedString64Bytes>();


    //do podłączenia textboxów z listą graczy dla poszczególnych teamów w menu pod Tab
    //public TextMeshProUGUI TabMenuTeam1;
    //public TextMeshProUGUI TabMenuTeam2;


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
            refreshMe();
            //NetworkManager.ConnectedClientsList
            // int tmp = NetworkManager.ConnectedClientsIds.Count;

            // if(tmp!=n)
            // {
            //     n = tmp;
            //     int x = n/2;
            //     int i = 0;
            //     foreach(ulong clientId in NetworkManager.ConnectedClientsIds)
            //     {
            //         //GameObject playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject;
            //         //NetworkObject playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            //         //int newTeam = i%2+1;
            //         //playerObj.GetComponent<PlayerManager>().UpdateTeamServerRpc(newTeam);

            //         // Texture team;
            //         // if(i < x)
            //         //     team = _red;
            //         // else
            //         //     team = _blue;
            //         ++i; 
            //     }
            // }
            
        }
        //refresh_playerList_ServerRpc();
        float minutes = Mathf.Floor(roundTimer.Value/60);
        float seconds = Mathf.Floor(roundTimer.Value%60f);
        HUD_roundTimer.text = $"{minutes}:{seconds}";


        //TabMenuTeam1.text = redTeamList.Value.ToString();
        //Debug.Log(TabMenuTeam1.text);
        //TabMenuTeam2.text = blueTeamList.Value.ToString();


    }


private void refreshMe()
{
    string redLista = "";
    string blueLista = "";

    foreach(ulong clientId in NetworkManager.ConnectedClientsIds)
    {
            NetworkObject playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            string playerName = playerObj.GetComponent<PlayerManager>().playerName.Value.ToString();
            int playerTeam = playerObj.GetComponent<PlayerManager>().team.Value;
            if(playerTeam == 1)
                redLista+= playerName + "\n";
            else if(playerTeam == 2)
                blueLista+= playerName + "\n"; 
    }
    refresh_playerList_ServerRpc(redLista, blueLista);
}
    [ServerRpc]
    public void refresh_playerList_ServerRpc(string A, string B)
    {
            redTeamList.Value=A;
            blueTeamList.Value=B;
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
