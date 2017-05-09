﻿using System.Collections;
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
    static int playersActive =0;
    static int playersFinished =0;
    static int playersToNextLevel =0;//number of players that should finish to go to next level

 

    void Start() {
        
        ag = GameObject.Find("allGM").GetComponent<allGM>();
        NetworkServer.RegisterHandler(1001, OnGiveNextPlayerToObserve);

       
    }

   

    public static void addPlayer(Player p) {
        allActivePlayers.Add(p);

        p.setTeamId(allActivePlayers.Count);//temporary!
        p.setPlayerId(allActivePlayers.Count);

        totalPlayers++;
        playersActive++;
        playersToNextLevel = getPlayersToNextLevel(false,playersActive);

        ag.RpcUpdatePlayerCountFrame(playersActive, playersFinished, playersToNextLevel);
       
    }

    static int getPlayersToNextLevel(bool isShop, int activePlayers) {
        if (isShop) {
            return activePlayers;
        }
        else {
            switch (totalPlayers) {
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
        result.allPlayerIds = new int[totalSize];
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

            result.allPlayerIds[i] = p.getPlayerId();
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

        Debug.Log("count:"+ allFinishedPlayers.Count);

        foreach (Player p in allFinishedPlayers) {
            ind = (ind + 1) % allPlayerSpawnPoints.Length;
            p.RpcActivatePlayer(allPlayerSpawnPoints[ind].transform.position);

        }
        allActivePlayers = allFinishedPlayers;
        allFinishedPlayers = new List<Player>();

        //reset player numbers
        playersActive = allActivePlayers.Count;
        playersFinished = 0;
        playersToNextLevel = getPlayersToNextLevel(ServerGM.isSceneShop(),playersActive);
        ag.RpcUpdatePlayerCountFrame(playersActive, playersFinished, playersToNextLevel);

    }

 

    public static void playerFinished(int id) {
        for (int i = 0; i < allActivePlayers.Count; i++) {
            if (allActivePlayers[i].getPlayerId() == id) {
                allFinishedPlayers.Add(allActivePlayers[i]);
                allActivePlayers.RemoveAt(i);
                playersActive--;
                playersFinished++;
                ag.RpcUpdatePlayerCountFrame(playersActive, playersFinished, playersToNextLevel);
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



    public static bool isPlayersToEndReached() {
        return (playersFinished == playersToNextLevel);
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
