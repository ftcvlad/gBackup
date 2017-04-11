using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class PlayerColor_hook : LobbyHook {
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
        LobbyPlayer lp = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerColor pc = gamePlayer.GetComponent<PlayerColor>();

        pc.color = lp.playerColor; 
    }
	
}
