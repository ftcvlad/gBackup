using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


//ONLY USED ON SERVER, BUT SPAWNED EVERYWHERE 
public class AllPlayerManager : NetworkBehaviour {

    static allGM ag;

    static List<Player> allActivePlayers = new List<Player>();
    static List<Player> allFinishedPlayers = new List<Player>();
    static List<Player> allDeadPlayers = new List<Player>();

    static int totalPlayers =0;
    public static int playersActive =0;
    public static int playersFinished =0;
    public static int playersToNextLevel =0;//number of players that should finish to go to next level

 
    public static int getActivePlayerCount() {
        return allActivePlayers.Count;
    }

    void Awake() {
        Debug.Log("allPlayerManagerAwake");
        ag = GameObject.Find("allGM").GetComponent<allGM>();
    }


    void Start() {
        Debug.Log("allPlayerManagerStart");
       
        NetworkServer.RegisterHandler(1001, OnGiveNextPlayerToObserve);

       
    }

   

    public static void addPlayer(Player p) {
        allActivePlayers.Add(p);


        totalPlayers++;
        playersActive++;
        playersToNextLevel = getPlayersToNextLevel(false,playersActive);



        //ALLGM NOT SPAWNED WHEN 1 PLAYER WITH LOBBY
        //ag.RpcUpdatePlayerCountFrame(playersActive, playersFinished, playersToNextLevel);
       
    }

    public static void updatePlayerFrame() {
        ag.RpcUpdatePlayerCountFrame(playersActive, playersFinished, playersToNextLevel);
    }


    static int getPlayersToNextLevel(bool isShop, int activePlayers) {
        if (isShop) {
            return activePlayers;
        }
        else {
            switch (activePlayers) {
                case 1:
                    return 1;
                case 2:
                   return 2;
                case 3:
                    return 2;
                case 4:
                    return 3;
                case 5:
                    return 3;
                case 6:
                    return 4;
                default:
                    return 4;
            }
        }
    }


    public static int determineWinningTeam() {


        int maxCount = -1;
        int count;
        int size = allFinishedPlayers.Count;
        List<int> maxTeamIds = new List<int>();

        //find teamIds which are encountered more often
        for (int i = 0; i < size; i++) {
            count = 0;
            int compareTeamId = allFinishedPlayers[i].getTeamId();
            for (int j = i; j < size; j++) {
                if (compareTeamId == allFinishedPlayers[j].getTeamId()) {
                    count++;
                }
            }
            if (count > maxCount) {
                maxCount = count;
                maxTeamIds = new List<int>();
                maxTeamIds.Add(compareTeamId);
            }
            else if (count == maxCount) {
                maxTeamIds.Add(compareTeamId);
            }
        }
        //if serveral teams have same number of players, wins the one with the 1st player among them
        if (maxTeamIds.Count == 1) {
                return maxTeamIds[0];
        }
        else {
            foreach (Player p in allFinishedPlayers) {
                if ( maxTeamIds.IndexOf(p.getTeamId()) != -1) {
                    return p.getTeamId();
                }
            }
            return -1;//cannot happen
        }
       
    }

    public static PlayerResult calculateResults(int totalTeamPrize, int perPlaceGoldStep, int goldForUnfinished) {


        //Rpc can't send list of objects :(

        PlayerResult result = new PlayerResult();
        int totalSize = allFinishedPlayers.Count + allActivePlayers.Count;
        result.allPlayerNetIds = new NetworkInstanceId[totalSize];
        result.allPlaces = new int[totalSize];
        result.allGoldWon = new int[totalSize];


        //determine winning team 
        //It is the team, which has the most finished players. If equal #, winning is the team with max finished and 1st player among finished

        int winningTeamId = determineWinningTeam();

        int nOfWinningTeam =0;
        foreach(Player p in allFinishedPlayers) {
            if (p.getTeamId() == winningTeamId) {
                nOfWinningTeam++;
            }
        }

        int goldPerPersonInTeam = (int)Mathf.Floor(totalTeamPrize/ nOfWinningTeam);
        int totalPlayersFinished = allFinishedPlayers.Count;

        int i = 0;
        int goldWon = 0;
        foreach (Player p in allFinishedPlayers) {

            result.allPlayerNetIds[i] = p.netId;
            result.allPlaces[i] = i + 1;
            goldWon = (totalPlayersFinished - i) * perPlaceGoldStep;
            if (p.getTeamId()== winningTeamId) {
                goldWon += goldPerPersonInTeam;
            }

            result.allGoldWon[i] = goldWon;
            p.gold += goldWon;//[SyncVar]
            i++;  
        }

        foreach (Player p in allActivePlayers) {
            result.allPlayerNetIds[i] = p.netId;
            result.allPlaces[i] = totalPlayersFinished + 1;
            result.allGoldWon[i] = goldForUnfinished;
            p.gold += result.allGoldWon[i];//[SyncVar]
            i++;
        }

        return result;
    }


    public static void finishRemainingPlayers() {
        foreach (Player p in allActivePlayers) {
            allFinishedPlayers.Add(p);
            p.RpcDeactivateFinishedPlayer();
        }

        foreach (Player p in allDeadPlayers) {
            p.RpcHideDeadPlayer();
        }
        allActivePlayers = null;
    }


   

    public static IEnumerator activatePlayers() {
        
        //connection.isReady is set to false after ServerChangeScene, but then to true in OnConnection
        //so, it is true after 2 sec (rpc works again). + 2 sec is a nice pause before players are spawned
        yield return new WaitForSeconds(2f);

        GameObject[] allPlayerSpawnPoints = GameObject.FindGameObjectsWithTag("playerSpawnPoint");
        int ind = -1;

        allFinishedPlayers.AddRange(allDeadPlayers);//here all players out there
        allActivePlayers = allFinishedPlayers;
        allFinishedPlayers = new List<Player>();
        allDeadPlayers = new List<Player>();

        foreach (Player p in allActivePlayers) {
            ind = (ind + 1) % allPlayerSpawnPoints.Length;
            p.RpcActivatePlayer(allPlayerSpawnPoints[ind].transform.position);

        }
        

        //reset player numbers
        playersActive = allActivePlayers.Count;
        playersFinished = 0;
        playersToNextLevel = getPlayersToNextLevel(ServerGM.isSceneShop(),playersActive);
        ag.RpcUpdatePlayerCountFrame(playersActive, playersFinished, playersToNextLevel);

    }

 

    public static void playerFinished(NetworkInstanceId netId) {
        for (int i = 0; i < allActivePlayers.Count; i++) {
            if (allActivePlayers[i].netId == netId) {
                allFinishedPlayers.Add(allActivePlayers[i]);
                allActivePlayers.RemoveAt(i);
                playersActive--;
                playersFinished++;
                ag.RpcUpdatePlayerCountFrame(playersActive, playersFinished, playersToNextLevel);
                return;
            }
        }
    }

    public static void playerDied(NetworkInstanceId netId) {
        for (int i = 0; i < allActivePlayers.Count; i++) {
            if (allActivePlayers[i].netId == netId) {
                allDeadPlayers.Add(allActivePlayers[i]);
                allActivePlayers.RemoveAt(i);
                playersActive--;

            
                if (playersFinished + playersActive > 0) {
                    playersToNextLevel = Mathf.Min(playersActive + playersFinished, playersToNextLevel);
                }
                
                return;
            }
        }
    }



    public static bool isPlayersToEndReached() {
        return (playersFinished == playersToNextLevel);
    }



   
    public void OnGiveNextPlayerToObserve(NetworkMessage netMsg) {
        Debug.Log("message received by server");

        var msgReceived = netMsg.ReadMessage<RequestNextPlayerMessage>();
        NetworkInstanceId currObservedPlayerId = msgReceived.currObservedPlayerId;
        NetworkInstanceId requesterId = msgReceived.localPlayerNetId;

        int i = 0;

        for (; i < allActivePlayers.Count; i++) {
            if (allActivePlayers[i].netId == currObservedPlayerId) {
                break;
            }
        }
        i = (i + 1) % allActivePlayers.Count;



        var msgSend = new MyMessage();
        msgSend.netId = allActivePlayers[i].netId;


        NetworkServer.FindLocalObject(requesterId).GetComponent<Player>().connectionToClient.Send(1000, msgSend);

    }


    public static void givePlayerToObserve(NetworkInstanceId requesterId) {
        var msg = new MyMessage();
        msg.netId = allActivePlayers[0].netId;

        //client is the one, where this p.isLocalPlayer == true
        //connectionToClient == The connection associated with this NetworkIdentity
        NetworkServer.FindLocalObject(requesterId).GetComponent<Player>().connectionToClient.Send(1000, msg);
    }


}
