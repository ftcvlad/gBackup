using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemHealthPot : NetworkBehaviour {



    void OnTriggerEnter2D(Collider2D other) {

        transform.GetComponent<BoxCollider2D>().enabled = false;
        transform.GetComponent<SpriteRenderer>().enabled = false;


        if (isServer) {
            other.GetComponent<Player>().numHealthPotsPossessed += 1;

            Destroy(this.gameObject, 2f);
        }
    }
}
