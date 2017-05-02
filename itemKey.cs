using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class itemKey : NetworkBehaviour {

    [HideInInspector]
    public bool isPickable = false;

	void Start () {
       
    }


    void OnTriggerEnter2D(Collider2D other) {//when player finds key

        if (isServer && isPickable) {
            other.GetComponent<Player>().RpcKeyFound();
            Destroy(this.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other) {//when player drops key
        isPickable = true;///??? or rpc?
    }
}
