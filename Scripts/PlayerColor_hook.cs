using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class PlayerColor_hook : LobbyHook {
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
        Debug.Log("OnLobbyServerSceneLoadedForPlayer");
       
        LobbyPlayer lp = lobbyPlayer.GetComponent<LobbyPlayer>();
        Player p = gamePlayer.GetComponent<Player>();

        p.team = lp.playerTeam;
        p.playerName = lp.playerName;
        p.color = LobbyPlayer.Colors[lp.playerTeam-1];


    }
	
}
