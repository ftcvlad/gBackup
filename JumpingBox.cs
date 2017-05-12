using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingBox : MonoBehaviour {

   

  


   
    void OnCollisionEnter2D(Collision2D coll) {

       
        if (coll.gameObject.tag == "Player") {

            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            Vector3 contactPoint = coll.contacts[0].point;

            bool top = Mathf.Abs(contactPoint.y - collider.bounds.max.y) < 0.1 ? true : false;
            bool bottom = Mathf.Abs(contactPoint.y - collider.bounds.min.y) < 0.1 ? true : false;


            if (top) {
                coll.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 1000));
            }
            else if (bottom) {
                coll.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -1000));
            }
        
        }


    }

   

}
