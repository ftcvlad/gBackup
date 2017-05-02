using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class itemKey : NetworkBehaviour {

    //[HideInInspector]
    public bool isPickable;

	void Start () {
       
    }


    void OnTriggerEnter2D(Collider2D other) {//when player finds key

        //if (isServer && isPickable ) {
        //    other.GetComponent<Player>().RpcKeyFound();
        //    Destroy(this.gameObject);
        //}
        Debug.Log("I am client and I found key1");
        if (isPickable) {
            Debug.Log("I am client and I found key2");
            other.GetComponent<Player>().keyFound();
            //this.gameObject.SetActive(false);
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
