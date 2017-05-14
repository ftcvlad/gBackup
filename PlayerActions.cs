using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerActions : NetworkBehaviour {

    bool isDraggingBox = false;
    DistanceJoint2D dj;
    LineRenderer lr;
    Player player;

    ContactFilter2D cf;
    Collider2D[] closeDraggableBox = new Collider2D[1];

    ContactFilter2D cf2;
    Collider2D[] closeControlBox = new Collider2D[1];

    void Start () {
        dj = GetComponent<DistanceJoint2D>();
        lr = transform.Find("Rope").GetComponent<LineRenderer>();
        player = transform.GetComponent<Player>();


        cf = new ContactFilter2D();
        cf.SetLayerMask((1 << LayerMask.NameToLayer("DraggableBoxLayer")));

        cf2 = new ContactFilter2D();
        cf2.SetLayerMask((1 << LayerMask.NameToLayer("ControlBoxLayer")));


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
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, dj.connectedBody.transform.position);
    }

    //STOP draggin
    [Command]
    void CmdStopDragBox() {
        RpcStopDragBox();
    }

    [ClientRpc]
    void RpcStopDragBox() {
        stopDraggingBox();
    }

    public void stopDraggingBox() {
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
                    int n = transform.GetComponent<PolygonCollider2D>().OverlapCollider(cf, closeDraggableBox);
                    if (n>0) {
                        CmdStartDragBox(closeDraggableBox[0].gameObject.GetComponent<NetworkIdentity>().netId);
                    }    
                }
                else {
                    CmdStopDragBox();
                } 
        }


        //activate spikes

        if (Input.GetKeyDown(KeyCode.B)) {
            int n = transform.GetComponent<PolygonCollider2D>().OverlapCollider(cf2, closeControlBox);
            if (n > 0) {
                CmdChangeSpikeState(closeControlBox[0].gameObject.GetComponent<NetworkIdentity>().netId);

            }
        }


        //USE HEALTHPOT
        if (Input.GetKeyDown(KeyCode.P)) {
            player.CmdUseHealthPot();
        }
    }

    //change spikeControlBox

    [Command]
    public void CmdChangeSpikeState(NetworkInstanceId netId) {
        
        SpikeControlBox scb = ClientScene.FindLocalObject(netId).GetComponent<SpikeControlBox>();//to sync with server
        RpcChangeSpikeState(netId, scb.isActivated);
    }


    [ClientRpc]
    public void RpcChangeSpikeState(NetworkInstanceId netId, bool isActivated) {

        SpikeControlBox scb = ClientScene.FindLocalObject(netId).GetComponent<SpikeControlBox>();
        scb.isActivated = !isActivated;
        scb.updateOwnColor();

        foreach (GameObject go in scb.controlledBoxes) {
            go.transform.Find("Spikes").GetComponent<Spikes>().changeActivated(scb.isActivated);

        }
    }


}
