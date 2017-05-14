using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class PlayerColor_hook : LobbyHook {
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
        Debug.Log("OnLobbyServerSceneLoadedForPlayer");
       
        LobbyPlayer lp = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerTeam pt = gamePlayer.GetComponent<PlayerTeam>();

        pt.team = lp.playerTeam;
        pt.name = lp.playerName;
        pt.color = LobbyPlayer.Colors[lp.playerTeam-1];


    }
	
}
