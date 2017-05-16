using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectKeyPot : MonoBehaviour {




    void OnTriggerEnter2D(Collider2D other) {//when player finds key

        if (other.gameObject.tag == "Player") {
            Player p = other.GetComponent<Player>();
            if (!p.hasKey) {
                other.GetComponent<Player>().keyFound();
            }
        }
        
    }

    
}
