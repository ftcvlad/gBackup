using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScaleSpecialSnowflake : NetworkBehaviour {

    Transform playerBody;
    void Start() {
        playerBody = transform.Find("PlayerBody");
    }

    [Command]
    public void CmdUpdateScaleServer(Vector3 newScale) {
        playerBody.localScale = newScale;
    }

    [ClientRpc]
    public void RpcUpdateScaleClient(Vector3 newScale) {
        playerBody.localScale = newScale;
    }
}
