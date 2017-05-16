using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerActions : NetworkBehaviour {

    bool isDraggingBox = false;
    DistanceJoint2D dj;
    LineRenderer lr;
    Player player;


    static ContactFilter2D cf;
    static Collider2D[] closeGround = new Collider2D[10];

    GameObject helpFrame = null;
    Rigidbody2D rb;


    void Start () {
        dj = GetComponent<DistanceJoint2D>();
        lr = transform.Find("Rope").GetComponent<LineRenderer>();
        player = transform.GetComponent<Player>();

        cf = new ContactFilter2D();
        cf.SetLayerMask((1 << LayerMask.NameToLayer("Ground")));

        

        rb = GetComponent<Rigidbody2D>();
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


        //SHOW HELP
        if (Input.GetKeyDown(KeyCode.H)) {

            if (helpFrame==null) {
                helpFrame = GameObject.Find("allGM").transform.Find("LevelUI").Find("HelpFrame").gameObject;//to search for inactive object
            }
            helpFrame.SetActive(!helpFrame.activeSelf);
        }

        if (rb.isKinematic) {//if can't move, can't use boxes as well
            return;
        }

        if (Input.GetKeyDown(KeyCode.S)) {
                if (!isDraggingBox) {

                    GameObject go = checkPlayerOverlapBox(transform,"DraggableBox");

                    if (go!=null) {
                        CmdStartDragBox(go.GetComponent<NetworkIdentity>().netId);
                    }    
                }
                else {
                    CmdStopDragBox();
                } 
        }


        //activate spikes

        if (Input.GetKeyDown(KeyCode.S)) {

            GameObject go = checkPlayerOverlapBox(transform, "ControlBox");

            if (go != null) {
                CmdChangeSpikeState(go.GetComponent<NetworkIdentity>().netId);
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


    public static GameObject checkPlayerOverlapBox(Transform playerTrans, string targetTag) {

        closeGround = new Collider2D[10];
        int n = playerTrans.GetComponent<PolygonCollider2D>().OverlapCollider(cf, closeGround);


        if (n == 0) {
            return null;
        }
        else {
            for (int i = 0; i < closeGround.Length; i++) {
                if (closeGround[i] == null) {
                    return null;
                }
                if (closeGround[i].gameObject.tag == targetTag) {
                    return closeGround[i].gameObject;
                }
            }
            return null;
        }
    }

}
