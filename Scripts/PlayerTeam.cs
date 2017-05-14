using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTeam : NetworkBehaviour {
    [SyncVar] public int team;
    [SyncVar] public string name;
    [SyncVar] public Color color;

    SpriteRenderer sr;
    Player p;

	
	void Start () {
        sr = transform.Find("PlayerBody").Find("Graphics").GetComponent<SpriteRenderer>();
        p = transform.GetComponent<Player>();

        Debug.Log("team:"+team);
        if (team != 0) {
            sr.color = color;
            p.setTeamId(team);
            p.setName(name);
        }
        else {//**if run scene without lobby
            p.setTeamId(AllPlayerManager.getActivePlayerCount());
            p.setName("Player-"+ AllPlayerManager.getActivePlayerCount());
        }
       
       


        Debug.Log(p.getName()+" **** "+p.getTeamId());

    }
	

}
