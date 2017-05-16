using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class itemKey : NetworkBehaviour {

    [HideInInspector]
    public bool isPickable;

	void Start () {
       
    }


    void OnTriggerEnter2D(Collider2D other) {//when player finds key

        Player p = other.GetComponent<Player>();
        if (isPickable && !p.hasKey) {

            p.keyFound();
            transform.GetComponent<BoxCollider2D>().enabled = false;
            transform.Find("collider").GetComponent<SpriteRenderer>().enabled = false;
           

            if (isServer) {
                Destroy(this.gameObject, 2f);
            }
           
        }
    }

    void OnTriggerExit2D(Collider2D other) {//when player drops key
        isPickable = true;///??? or rpc?
    }
}
