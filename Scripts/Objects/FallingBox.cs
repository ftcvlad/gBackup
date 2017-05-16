using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBox : MonoBehaviour {

    Rigidbody2D rb;
    void Start() {
        rb = transform.GetComponent<Rigidbody2D>();
    }



    int collidingPlayers = 0;
    void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Player") {
            collidingPlayers++;
        }
    }

    void OnCollisionExit2D(Collision2D coll) {
       
        if (coll.gameObject.tag == "Player") {
            collidingPlayers--;
            if (collidingPlayers == 0) {
               rb.velocity = Vector3.zero;
            }
        }
    }
}
