using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScaleSpecialSnowflake : NetworkBehaviour {



    [Command]
    public void CmdUpdateScaleServer(Vector3 newScale) {
        transform.localScale = newScale;
    }

    [ClientRpc]
    public void RpcUpdateScaleClient(Vector3 newScale) {
        transform.localScale = newScale;
    }
}
