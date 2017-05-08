using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectKeyPot : MonoBehaviour {




    void OnTriggerEnter2D(Collider2D other) {//when player finds key

        Player p = other.GetComponent<Player>();
        if (!p.hasKey) {

            other.GetComponent<Player>().keyFound();

        }
    }

    
}
