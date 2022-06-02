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
    public NetworkVariable<float> pauseTimer =  new NetworkVariable<float>();
    public NetworkVariable<bool> pause =  new NetworkVariable<bool>();


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
    [SerializeField] private NetworkVariable<int> gameMode =  new NetworkVariable<int>();

    //do podłączenia textboxów z listą graczy dla poszczególnych teamów w menu pod Tab
    //public TextMeshProUGUI TabMenuTeam1;
    //public TextMeshProUGUI TabMenuTeam2;

    public TextMeshProUGUI teamRedScore;
    public TextMeshProUGUI teamBlueScore;

    public TextMeshProUGUI HUD_roundTimer;

    int n = 0;
    void Start()
    {
         //Cursor.lockState = CursorLockMode.Locked;

            teamRedScore.text="X";
            teamBlueScore.text="X";
         
    }
    public Texture _red, _blue;
    // Update is called once per frame


    void Update()
    {
        float minutes;
        float seconds;
    if(NetworkManager.IsHost)
    {
        if(Input.GetKeyDown(KeyCode.P) && players.Value > 1)
            setGameModeServerRpc(1);
            
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
                       HUD_roundTimer.text = $"PAUSE\n{minutes}:{seconds}";
                }
                else
                {
                    minutes = Mathf.Floor(roundTimer.Value/60);
                    seconds = Mathf.Floor(roundTimer.Value%60f);
                    HUD_roundTimer.text = $"{minutes}:{seconds}";
                }
        
                    
            }
            else
            {
                HUD_roundTimer.text = "PAUSE";
            }

            
        }



        
        


        //TabMenuTeam1.text = redTeamList.Value.ToString();
        //Debug.Log(TabMenuTeam1.text);
        //TabMenuTeam2.text = blueTeamList.Value.ToString();


    



[ServerRpc]
public void setGameModeServerRpc(int mode)
{
    gameMode.Value = mode;
    if(mode!=0)
        pause.Value = false;
        restart_RoundTimer_ServerRpc();
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
    public void update_RoundTimer_ServerRpc() 
    {   
        if(pauseTimer.Value>0.0)
        {
            pauseTimer.Value -= Time.deltaTime;
            
        }

        else
        {
            pause.Value = false;
            roundTimer.Value -= Time.deltaTime;

            if(playersAliveInRedTeam.Value ==0 && playersAliveInBlueTeam.Value != 0)
            {
                    roundTimer.Value = roundTime;
                    round.Value++;
                    setBlueScoreServerRpc(1);
                    pauseTimer.Value = 5.0f;
                    pause.Value = true;
                    restart_RoundTimer_ServerRpc();
            }
            else if(playersAliveInRedTeam.Value !=0 && playersAliveInBlueTeam.Value == 0)
            {
                    roundTimer.Value = roundTime;
                    round.Value++;
                    setRedScoreServerRpc(1);
                    pauseTimer.Value = 5.0f;
                    pause.Value = true;
                    restart_RoundTimer_ServerRpc();
            }
            else if(roundTimer.Value < 0f)
            {
                roundTimer.Value = roundTime;
                round.Value++;
                pauseTimer.Value = 5.0f;
                pause.Value = true;
                restart_RoundTimer_ServerRpc();
            }

            if(round.Value ==8)
            {
                round.Value=0;
                setBlueScoreServerRpc(0);
                setRedScoreServerRpc(0);
                pauseTimer.Value = 5.0f;
                pause.Value = true;
                restart_RoundTimer_ServerRpc();
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


}
