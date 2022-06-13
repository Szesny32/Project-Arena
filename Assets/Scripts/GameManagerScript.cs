using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;
public class GameManagerScript : NetworkBehaviour
{

    //Red Team
    public NetworkVariable<FixedString64Bytes> redTeamList =  new NetworkVariable<FixedString64Bytes>();
    public TextMeshProUGUI teamRedScore;
    [SerializeField] private NetworkVariable<int> redTeamScore =  new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> playersInRedTeam =  new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> playersAliveInRedTeam =  new NetworkVariable<int>();

    //Blue Team
    public NetworkVariable<FixedString64Bytes> blueTeamList =  new NetworkVariable<FixedString64Bytes>();
    public TextMeshProUGUI teamBlueScore;
    [SerializeField] private NetworkVariable<int> blueTeamScore =  new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> playersInBlueTeam =  new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> playersAliveInBlueTeam =  new NetworkVariable<int>();
    
    public TextMeshProUGUI HUD_roundTimer;
    public NetworkVariable<float> pauseTimer =  new NetworkVariable<float>();
    public NetworkVariable<bool> pause =  new NetworkVariable<bool>();
    private float roundTime = 300f;
    [SerializeField] private NetworkVariable<float> roundTimer =  new NetworkVariable<float>();
    public NetworkVariable<int> gameMode =  new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> round =  new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> players =  new NetworkVariable<int>();

    
    void Start()
    {
        teamRedScore.text="0";
        teamBlueScore.text="0";
        setPause_ServerRpc();
    }

    void Update()
    {
        float minutes;
        float seconds;
        if(NetworkManager.IsHost)
        {
            if(Input.GetKeyDown(KeyCode.P) && players.Value > 1)
            {
                if(gameMode.Value!=0)
                    setGameModeServerRpc(0);
                else
                    setGameModeServerRpc(1);
            }

        }
        if(NetworkManager.IsServer)
        {
            refreshMe();
            if(gameMode.Value != 0)
                update_RoundTimer_ServerRpc();
           
        }

        teamRedScore.text=$"{redTeamScore.Value}";
        teamBlueScore.text=$"{blueTeamScore.Value}";
        if(gameMode.Value != 0)
        {  
            if(pause.Value)
            {
                minutes = Mathf.Floor(pauseTimer.Value/60);
                seconds = Mathf.Floor(pauseTimer.Value%60f);
               HUD_roundTimer.text = "PAUSE\n"+ minutes.ToString("00") + " : "+seconds.ToString("00");
                //$"PAUSE\n{minutes}:{seconds}";
            }
            else
            {

                minutes = Mathf.Floor(roundTimer.Value/60);
                seconds = Mathf.Floor(roundTimer.Value%60f);
                HUD_roundTimer.text = minutes.ToString("00") + " : "+seconds.ToString("00");
            }               
        }
        else
            HUD_roundTimer.text = "PAUSE";
    }



[ServerRpc]
public void setGameModeServerRpc(int mode)
{
    gameMode.Value = mode;
    pause.Value = true;
    restart_PauseTimer_ServerRpc();
    restart_RoundTimer_ServerRpc();
    round.Value=0;
    blueTeamScore.Value = 0;
    redTeamScore.Value = 0;          
}


private void refreshMe()
{
    string redLista = "";
    string blueLista = "";
    int RedAlive = 0;
    int BlueAlive = 0;
    int N = 0;
    foreach(ulong clientId in NetworkManager.ConnectedClientsIds)
    {
            NetworkObject playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            string playerName = playerObj.GetComponent<PlayerManager>().playerName.Value.ToString();
            int playerTeam = playerObj.GetComponent<PlayerManager>().team.Value;
            float HP = playerObj.GetComponent<HUD_Manager>().HP.Value;
            N++;
            if(playerTeam == 1)
            {
                    redLista+= playerName + "\n";
                    if(HP<=0f)
                        RedAlive++;
            }
            else if(playerTeam == 2)
            {
                    blueLista+= playerName + "\n"; 
                    if(HP<=0f)
                        BlueAlive++;
            }
    }
    setPlayerNumberServerRpc(N);
    setPlayerAliveServerRpc(RedAlive,BlueAlive);
    refresh_playerList_ServerRpc(redLista, blueLista);
}



    [ServerRpc]
    public void setPlayerAliveServerRpc(int A, int B)
    {
        playersAliveInRedTeam.Value = A;
        playersAliveInBlueTeam.Value = B;
    }

   [ServerRpc]
    public void setPlayerNumberServerRpc(int N)
    {
        players.Value = N;
    }


    [ServerRpc]
    public void refresh_playerList_ServerRpc(string A, string B)
    {
            redTeamList.Value=A;
            blueTeamList.Value=B;
    }


    [ServerRpc]
    public void setPause_ServerRpc() 
    {
         pause.Value = true;
    }


    [ServerRpc]
    public void update_RoundTimer_ServerRpc() 
    {   
        if(pauseTimer.Value > 0.0)
            pauseTimer.Value -= Time.deltaTime;
            
        else
        {
            pause.Value = false;
            roundTimer.Value -= Time.deltaTime;

            if(playersAliveInRedTeam.Value ==0 && playersAliveInBlueTeam.Value != 0)
            {
                    roundTimer.Value = roundTime;
                    round.Value++;
                    setBlueScoreServerRpc(1);
                   restart_PauseTimer_ServerRpc();
                    pause.Value = true;
                    restart_RoundTimer_ServerRpc();
            }
            else if(playersAliveInRedTeam.Value !=0 && playersAliveInBlueTeam.Value == 0)
            {
                    roundTimer.Value = roundTime;
                    round.Value++;
                    setRedScoreServerRpc(1);
                    restart_PauseTimer_ServerRpc();
                    pause.Value = true;
                    restart_RoundTimer_ServerRpc();
            }
            else if(roundTimer.Value < 0f)
            {
                roundTimer.Value = roundTime;
                round.Value++;
                restart_PauseTimer_ServerRpc();
                pause.Value = true;
                restart_RoundTimer_ServerRpc();
            }

            if(round.Value ==8)
            {
                setGameModeServerRpc(0);
            }
        }
    }


    [ServerRpc]
    public void setRedScoreServerRpc(int x) 
    {
       if(x==1)
            ++redTeamScore.Value;
        else
            redTeamScore.Value = x;
    }
    
    [ServerRpc]
    public void setBlueScoreServerRpc(int x) 
    {
        if(x==1)
            ++blueTeamScore.Value;
        else
            blueTeamScore.Value = x;
    }

    [ServerRpc]
    public void restart_RoundTimer_ServerRpc() {
        roundTimer.Value = roundTime;
    }

    [ServerRpc]
    public void restart_PauseTimer_ServerRpc() {
        pauseTimer.Value = 5.0f;
    }

}
