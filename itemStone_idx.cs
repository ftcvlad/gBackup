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
       
        other.GetComponent<Player>().updateStoneAmount(1);

        if (isServer) {
            gmInst.stoneFreeSpawnPoint(spawnPointIndex);
        }

        Destroy(this.gameObject);
    }
}
