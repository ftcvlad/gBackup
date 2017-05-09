using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShopDoor : MonoBehaviour {

    Player enteringPlayer;
    Player localPlayer = null;
    bool playerClose;


    Transform uioverlay;
    void Start() {
        uioverlay = transform.Find("UIoverlay");
    }

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
        if (Input.GetKeyDown(KeyCode.B) && playerClose) {
            localPlayer.GetComponent<MovementInput>().enabled = false;
            uioverlay.gameObject.SetActive(true);
        }
    }


    public void OnOKButtonPressed() {
        localPlayer.GetComponent<MovementInput>().enabled = true;
        uioverlay.gameObject.SetActive(false);
        localPlayer.CmdExitShop();
    }

    public void OnCancelButtonPressed() {
        localPlayer.GetComponent<MovementInput>().enabled = true;
        uioverlay.gameObject.SetActive(false);
    }

}
