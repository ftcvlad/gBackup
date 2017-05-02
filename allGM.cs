//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;

//public class allGM : NetworkBehaviour {



	
//    //this is called only once at start to make key pickable -- by default it is not, because when player
//    //drops key, it instantiates inside of player
//    [ClientRpc]
//    public void RpcMakeKeyPickable() {
//        Debug.Log("0");
//        GameObject keyGo = GameObject.FindGameObjectWithTag("key");
//        if (keyGo==null) {
//            Debug.Log("key NOT SPAWNED!");
//        }
//        else {
//            keyGo.GetComponent<itemKey>().isPickable = true;
//        }
       
//    }
	
//}
