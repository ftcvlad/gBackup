using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLevelDoor : MonoBehaviour {


    Player localPlayer = null;
    bool playerClose;


    void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<Player>().isLocalPlayer) {
            if (localPlayer == null) {
                localPlayer = other.GetComponent<Player>();
            }
            playerClose = true;
        }
    }


    void OnTriggerExit2D(Collider2D other) {
        if (other.GetComponent<Player>().isLocalPlayer) {
            playerClose = false;
        }
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.U) && playerClose) {
            if (localPlayer.hasKey) {
                localPlayer.CmdExitLevel();
            }
            
        }
    }

}
