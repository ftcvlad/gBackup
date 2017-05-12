using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {

    bool isDraggingBox = false;
    DistanceJoint2D dj;
    LineRenderer lr;

    void Start () {
        dj = GetComponent<DistanceJoint2D>();
        lr = transform.Find("Rope").GetComponent<LineRenderer>();
    }

    void Update() {


        //DRAG BOX
        if (isDraggingBox) {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, dj.connectedBody.transform.position);
        }
        if (Input.GetKeyDown(KeyCode.B)) {
            if (!isDraggingBox) {

                if (currentCloseDraggableBox != null) {
                    dj.enabled = true;
                    dj.connectedBody = currentCloseDraggableBox.GetComponent<Rigidbody2D>();
                    isDraggingBox = true;
                    lr.enabled = true;
                }
            }
            else {
                dj.enabled = false;
                dj.connectedBody = null;
                isDraggingBox = false;
                lr.enabled = false;
            }
        }

        //USE HEALTHPOT
        if (Input.GetKeyDown(KeyCode.P)) {
            transform.GetComponent<Player>().CmdUseHealthPot();
        }
    }

    GameObject currentCloseDraggableBox = null;
    void OnCollisionEnter2D(Collision2D coll) {

        if (coll.gameObject.tag == "DraggableBox") {
            if (currentCloseDraggableBox == null) {
                currentCloseDraggableBox = coll.gameObject;
            }
        }

    }

    void OnCollisionExit2D(Collision2D coll) {
        if (coll.gameObject.tag == "DraggableBox") {
            currentCloseDraggableBox = null;
        }
    }
}
