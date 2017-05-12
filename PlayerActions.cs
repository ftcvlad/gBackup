using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerActions : NetworkBehaviour {

    bool isDraggingBox = false;
    DistanceJoint2D dj;
    LineRenderer lr;
    Player player;
    GameObject currentCloseDraggableBox = null;


    void Start () {
        dj = GetComponent<DistanceJoint2D>();
        lr = transform.Find("Rope").GetComponent<LineRenderer>();
        player = transform.GetComponent<Player>();
    }


    //START dragging
    [Command]
    void CmdStartDragBox(NetworkInstanceId netId) {
        RpcStartDragBox(netId);
    }

    [ClientRpc]
    void RpcStartDragBox(NetworkInstanceId netId) {
        dj.enabled = true;
        dj.connectedBody = ClientScene.FindLocalObject(netId).GetComponent<Rigidbody2D>();
        isDraggingBox = true;
        lr.enabled = true;
    }

    //STOP draggin
    [Command]
    void CmdStopDragBox() {
        RpcStopDragBox();
    }

    [ClientRpc]
    void RpcStopDragBox() {
        dj.enabled = false;
        dj.connectedBody = null;
        isDraggingBox = false;
        lr.enabled = false;
    }

   


    void Update() {


        //DRAG BOX
        if (isDraggingBox) {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, dj.connectedBody.transform.position);
        }

        if (!player.isLocalPlayer) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.B)) {
                if (!isDraggingBox) {

                    if (currentCloseDraggableBox != null) {
                        CmdStartDragBox(currentCloseDraggableBox.GetComponent<NetworkIdentity>().netId);
                    }
                }
                else {
                    CmdStopDragBox();
                } 
        }

        //USE HEALTHPOT
        if (Input.GetKeyDown(KeyCode.P)) {
           player.CmdUseHealthPot();
        }
    }


    void OnCollisionEnter2D(Collision2D coll) {

        if (!player.isLocalPlayer) {
            return;
        }

        if (coll.gameObject.tag == "DraggableBox") {
            if (currentCloseDraggableBox == null) {
                currentCloseDraggableBox = coll.gameObject;
            }
        }

    }

    void OnCollisionExit2D(Collision2D coll) {
        if (!player.isLocalPlayer) {
            return;
        }

        if (coll.gameObject.tag == "DraggableBox") {
            currentCloseDraggableBox = null;
        }
    }
}
