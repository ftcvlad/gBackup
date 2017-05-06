using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScaleSpecialSnowflake : NetworkBehaviour {

    Transform playerBody;
    [SyncVar(hook = "updateScale")] Vector3 scale;

    void Start() {
        playerBody = transform.Find("PlayerBody");
        scale = playerBody.localScale;
    }

    void updateScale(Vector3 newScale) {//called when scale changed and both on client+server
        if (playerBody != null) {
            playerBody.localScale = newScale;
        }
    }


    [Command]
    public void CmdUpdateScaleServer(Vector3 newScale) {
        scale = newScale;
    }

   

   

   
}
