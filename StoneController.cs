using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StoneController : NetworkBehaviour {

    public LayerMask whatToHit;
    public Transform stoneBreakParticlesPrefab;

    public GameObject stoneThrower;

    [SerializeField] int damageAmount = 30;

    public int teamNum;

    void Start() {
   
       
        if (stoneThrower == null) {
            Debug.Log("Player-thrower, where are you?");
        }
    }


    void OnCollisionEnter2D(Collision2D collision) {

        foreach (ContactPoint2D contact in collision.contacts) {

            //stone == otherCollider
            //rest == collider


            string collLayerName = LayerMask.LayerToName(collision.collider.gameObject.layer);
            if (collLayerName=="Player") {
                if (!isServer) {
                    return;
                }
                if (collision.collider.gameObject != stoneThrower) {
                    //damage other player
                    collision.collider.gameObject.GetComponent<Player>().takeDamage(damageAmount);
                }
                else {
                    Debug.LogError("Move stone spawn point away from player -- they collide!");
                }

            }
            else if (collLayerName == "Ground") {
              

                //particles
                Transform stoneBrPart = (Transform)Instantiate(stoneBreakParticlesPrefab, contact.point, Quaternion.FromToRotation(Vector3.right, contact.normal));
                Destroy(stoneBrPart.gameObject, 1f);
               

            }


            //if (isServer) {
            //    NetworkServer.Destroy(this.gameObject);
            //}

            Destroy(this.gameObject);




        }


    }


}
