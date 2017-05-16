using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class itemStone_idx : NetworkBehaviour {

    public LayerMask whatToHit;
    public int spawnPointIndex { get; set; }
    ServerGM gmInst;
   

    void Start() {
        if (isServer) {
            gmInst = GameObject.Find("GM").GetComponent<ServerGM>();
        }
        
    }

    void OnTriggerEnter2D(Collider2D other) {


        transform.GetComponent<CircleCollider2D>().enabled = false;
        transform.GetComponent<SpriteRenderer>().enabled = false;

        
        if (isServer) {
            other.GetComponent<Player>().numStonesPossessed += 1;

            gmInst.stoneFreeSpawnPoint(spawnPointIndex);
            Destroy(this.gameObject, 2f);
        }
        //else if (other.GetComponent<Player>().isLocalPlayer) {//BUG: localPlayer client numStonesPossessed wrong RARELY(and then replaced by correct server's value). So, instead update only on server, localPlayer client updated with tiny lag
        //   other.GetComponent<Player>().setStonesPossessed(other.GetComponent<Player>().numStonesPossessed + 1);
        //}
       
        


    }
}
