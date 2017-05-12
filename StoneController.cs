using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StoneController : NetworkBehaviour {

    public LayerMask whatToHit;
    public Transform stoneBreakParticlesPrefab;

    [SyncVar] public int playerTeamId =-1;

    [SerializeField] int damageAmount = 40;

    public int teamNum;
    
    Rigidbody2D rb;
    void Start() {
   
       
        if (playerTeamId == -1) {
            Debug.Log("Player-thrower, where are you?");
        }

        rb = transform.GetComponent<Rigidbody2D>();
    }


    void OnTriggerEnter2D(Collider2D other) {



        string collLayerName = LayerMask.LayerToName(other.gameObject.layer);
        int layer_mask = LayerMask.GetMask("Player", "Ground");


        Vector2 currentPosition2D = (Vector2)(transform.position);
        RaycastHit2D hit = Physics2D.Raycast(currentPosition2D, currentPosition2D+rb.velocity,10f,layer_mask);


        if (collLayerName=="Player") {

            if (isServer) {
                Player p = other.GetComponent<Player>();

                if (p.hasKey) {
                    p.dropKey();
                }
                p.RpcTakeDamage(damageAmount);

            }






        }
            else if (collLayerName == "Ground") {
            //particles
            
                Transform stoneBrPart = (Transform)Instantiate(stoneBreakParticlesPrefab, hit.point, Quaternion.FromToRotation(Vector3.right, hit.normal));
                Destroy(stoneBrPart.gameObject, 1f);
            }

            //once collided, keep it on same place for Stone transform to correctly be sent to lagging clients.
            //Stone is destroyed by server after 5s
          
            transform.GetComponent<SpriteRenderer>().enabled = false;
            transform.GetComponent<CircleCollider2D>().enabled = false;



        


    }


}
