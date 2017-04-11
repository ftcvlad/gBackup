using UnityEngine;



public class CameraFollow : MonoBehaviour {


   // public float yPosRestriction = -1f;

    float cameraDistance = 10;
    float cameraHeight = 0;


   
    Transform target;//player
    Vector3 cameraOffset;

    void Awake() {


        GameObject go = GameObject.FindWithTag("Player");
        if (go != null) {
            target = go.transform;
        }
        else {
            Debug.LogError("no player found!");

        }

    }


    private void Start() {

      

        cameraOffset = new Vector3(0, cameraHeight, -(Mathf.Abs(target.position.z) + cameraDistance));
        //transform.parent = null;//??

        moveCamera();
       
    }

   
    private void Update() {

        moveCamera();

      
    }

    void moveCamera() {

        Vector3 newPos = new Vector3(target.position.x+ cameraOffset.x, target.position.y + cameraOffset.y, target.position.z + cameraOffset.z );
        transform.position = newPos;
        //transform.LookAt(target);
    }

}


