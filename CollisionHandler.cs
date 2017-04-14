using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour {

    public LayerMask whatToHit;
    public Transform stoneBreakParticlesPrefab;

    GameObject stoneThrower;
    

    void Start() {
        Transform t = this.transform;
        while (t.parent != null) {
            if (t.parent.tag == "Player") {
                stoneThrower = t.parent.gameObject;
                break;
            }
            t = t.parent.transform;
        }
       
        if (stoneThrower == null) {
            Debug.Log("Player-thrower, where are you?");
        }
    }


    void OnCollisionEnter2D(Collision2D collision) {

        foreach (ContactPoint2D contact in collision.contacts) {
            Debug.DrawRay(contact.point, contact.normal, Color.white);


            //stone == otherCollider
            //rest == collider

            Debug.Log(LayerMask.LayerToName(collision.otherCollider.gameObject.layer));


            string collLayerName = LayerMask.LayerToName(collision.collider.gameObject.layer);
            if (collLayerName=="Player") {


                if (collision.collider.gameObject != stoneThrower) {
                    //damage other player
                }
                else {
                    Debug.LogError("Move stone spawn point away from player -- they collide!");
                }


            }
            else if (collLayerName == "Ground") {
                Destroy(this.gameObject);

                //particles

               
                Transform stoneBrPart = (Transform)Instantiate(stoneBreakParticlesPrefab, contact.point, Quaternion.FromToRotation(Vector3.right, contact.normal));
                Destroy(stoneBrPart.gameObject, 1f);
                

            }

           

         
            
        }


    }
}
