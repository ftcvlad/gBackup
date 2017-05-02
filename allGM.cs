using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class allGM : NetworkBehaviour {



	
    //this is called only once at start to make key pickable -- by default it is not, because when player
    //drops key, it instantiates inside of player
    [ClientRpc]
    public void RpcMakeKeyPickable() {
        Debug.Log("1");
        GameObject keyGo = GameObject.FindGameObjectWithTag("key");
        if (keyGo==null) {
            Debug.Log("key not found!");
        }
        else {
            keyGo.GetComponent<itemKey>().isPickable = true;
        }
       
    }
	
}
