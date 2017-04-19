using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StoneController : NetworkBehaviour {

    public LayerMask whatToHit;
    public Transform stoneBreakParticlesPrefab;

    [SyncVar] public GameObject playerStoneThrower;

    [SerializeField] int damageAmount = 30;

    public int teamNum;

    Rigidbody2D rb;
    void Start() {
   
       
        if (playerStoneThrower == null) {
            Debug.Log("Player-thrower, where are you?");
        }

        rb = transform.GetComponent<Rigidbody2D>();
    }


    void OnCollisionEnter2D(Collision2D collision) {

    

        foreach (ContactPoint2D contact in collision.contacts) {

            //stone == otherCollider
            //rest == collider


            string collLayerName = LayerMask.LayerToName(collision.collider.gameObject.layer);
            if (collLayerName=="Player") {
                if (isServer) {
                    if (collision.collider.gameObject != playerStoneThrower) {
                        //damage other player
                        collision.collider.gameObject.GetComponent<Player>().takeDamage(damageAmount);
                    }
                    else {
                        Debug.LogError("Move stone spawn point away from player -- they collide!");
                    }
                }
               
            }
            else if (collLayerName == "Ground") {
              

                //particles
                Transform stoneBrPart = (Transform)Instantiate(stoneBreakParticlesPrefab, contact.point, Quaternion.FromToRotation(Vector3.right, contact.normal));
                Destroy(stoneBrPart.gameObject, 1f);
               

            }

            //once collided, keep it on same place for Stone transform to correctly be sent to lagging clients.
            //Stone is destroyed by server after 5s
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            transform.GetComponent<SpriteRenderer>().enabled = false;
            transform.GetComponent<CircleCollider2D>().enabled = false;



        }


    }


}
