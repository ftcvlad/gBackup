using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//ONLY USED ON SERVER, BUT SPAWNED EVERYWHERE 
public class AllPlayerManager : NetworkBehaviour {


    void Start() {
        NetworkServer.RegisterHandler(1001, OnGiveNextPlayerToObserve);
    }

    static List<Player> allActivePlayers = new List<Player>();

    

    public static void addPlayer(Player p) {
        allActivePlayers.Add(p);

        p.setTeamId(allActivePlayers.Count);//temporary!
        p.setPlayerId(allActivePlayers.Count);
    }

    public static void removePlayerById(int id) {
        for (int i = 0; i < allActivePlayers.Count; i++) {
            if (allActivePlayers[i].getPlayerId() == id) {
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
