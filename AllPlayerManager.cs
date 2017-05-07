using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


//ONLY USED ON SERVER, BUT SPAWNED EVERYWHERE 
public class AllPlayerManager : NetworkBehaviour {

    allGM ag;

    void Start() {

        ag = GameObject.Find("allGM").GetComponent<allGM>();
        NetworkServer.RegisterHandler(1001, OnGiveNextPlayerToObserve);
        
    }

    static List<Player> allActivePlayers = new List<Player>();
    static List<Player> allFinishedPlayers = new List<Player>();
    static List<Player> allDeadPlayers = new List<Player>();



    public static PlayerResult calculateResults(int winningTeamId, int totalTeamPrize, int perPlaceGoldStep, int goldForUnfinished) {


        //Rpc can't send list of objects :(

        PlayerResult result = new PlayerResult();
        int totalSize = allFinishedPlayers.Count + allActivePlayers.Count;
        result.allPlayerIds = new int[totalSize];
        result.allPlaces = new int[totalSize];
        result.allGoldWon = new int[totalSize];



        int nOfWinningTeam=0;
        foreach(Player p in allFinishedPlayers) {
            if (p.getTeamId() == winningTeamId) {
                nOfWinningTeam++;
            }
        }
        int goldPerPersonInTeam = (int)Mathf.Floor(totalTeamPrize/ nOfWinningTeam);
        int totalPlayersFinished = allFinishedPlayers.Count;

        int i = 0;
        foreach (Player p in allFinishedPlayers) {

            result.allPlayerIds[i] = p.getPlayerId();
            result.allPlaces[i] = i + 1;
            result.allGoldWon[i] = goldPerPersonInTeam + (totalPlayersFinished-i)*perPlaceGoldStep;
            p.gold += result.allGoldWon[i];//[SyncVar]
            i++;  
        }

        foreach (Player p in allActivePlayers) {
            result.allPlayerIds[i] = p.getPlayerId();
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
            p.RpcDeactivatePlayer();
        }
        allActivePlayers = null;
    }

   


    public static IEnumerator activateAlivePlayers() {
        //connection.isReady is set to false after ServerChangeScene, but then to true in OnConnection
        //so, it is true after 2 sec (rpc works again). + 2 sec is a nice pause before players are spawned
        yield return new WaitForSeconds(2f);

        GameObject[] allPlayerSpawnPoints = GameObject.FindGameObjectsWithTag("playerSpawnPoint");
        int ind = -1;
        foreach (Player p in allFinishedPlayers) {
            ind = (ind + 1) % allPlayerSpawnPoints.Length;
            p.RpcActivatePlayer(allPlayerSpawnPoints[ind].transform.position);

        }
        allActivePlayers = allFinishedPlayers;
        allFinishedPlayers = new List<Player>(); 
    }

    public static void addPlayer(Player p) {
        allActivePlayers.Add(p);

        p.setTeamId(allActivePlayers.Count);//temporary!
        p.setPlayerId(allActivePlayers.Count);
    }

    public static void playerFinished(int id) {
        for (int i = 0; i < allActivePlayers.Count; i++) {
            if (allActivePlayers[i].getPlayerId() == id) {
                allFinishedPlayers.Add(allActivePlayers[i]);
                allActivePlayers.RemoveAt(i);
                return;
            }
        }
    }

    public static void playerDied(int id) {
        for (int i = 0; i < allActivePlayers.Count; i++) {
            if (allActivePlayers[i].getPlayerId() == id) {
                allDeadPlayers.Add(allActivePlayers[i]);
                allActivePlayers.RemoveAt(i);
                return;
            }
        }
    }


    public static bool isLastInTeam(int teamId) {
        for (int i = 0; i < allActivePlayers.Count; i++) {
            if (allActivePlayers[i].getTeamId() == teamId) {
                return false;
            }
        }
        return true;
    }



   
    public void OnGiveNextPlayerToObserve(NetworkMessage netMsg) {
        Debug.Log("message received by server");

        var msgReceived = netMsg.ReadMessage<RequestNextPlayerMessage>();
        int currObservedPlayerId = msgReceived.currObservedPlayerId;
        NetworkInstanceId requesterId = msgReceived.localPlayerNetId;

        int i = 0;

        for (; i < allActivePlayers.Count; i++) {
            if (allActivePlayers[i].getPlayerId() == currObservedPlayerId) {
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
