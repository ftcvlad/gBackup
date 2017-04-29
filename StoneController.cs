using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StoneController : NetworkBehaviour {

    public LayerMask whatToHit;
    public Transform stoneBreakParticlesPrefab;

    [SyncVar] public int playerTeamId =-1;

    [SerializeField] int damageAmount = 30;

    public int teamNum;
    
    Rigidbody2D rb;
    void Start() {
   
       
        if (playerTeamId == -1) {
            Debug.Log("Player-thrower, where are you?");
        }

        rb = transform.GetComponent<Rigidbody2D>();
    }


    void OnCollisionEnter2D(Collision2D collision) {

            if (isServer) {
                Debug.Log("collide!");
            }

     

            ContactPoint2D contact = collision.contacts[0];//any contact point, can be >1

          
            //stone == otherCollider
            //rest == collider


            string collLayerName = LayerMask.LayerToName(collision.collider.gameObject.layer);
            if (collLayerName=="Player") {
                if (isServer) {
                    if (collision.collider.gameObject.GetComponent<Player>().getTeamId() != playerTeamId) {
                        //damage other player
                      
                        collision.collider.gameObject.GetComponent<Player>().RpcTakeDamage(damageAmount);
                    }
                    else {
                        Debug.Log("Player hits itself!");
                    
                        collision.collider.gameObject.GetComponent<Player>().RpcTakeDamage(damageAmount);
                    }

                    Debug.Log("zz"+collision.collider.gameObject.GetComponent<Player>().getTeamId());
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
