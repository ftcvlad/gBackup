using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ThrowStone : NetworkBehaviour {


    float timeToFire = 0;
    public float fireRate = 1;
    public float stoneSpeed = 400;

    Transform start;
    Transform end;
    [SerializeField]   GameObject stonePref;
    float stoneMass;

    bool throwPending = false;//input in update, Physics in FixedUpdate

    int vertCount = 40;

    bool isDrawTrajectory = true;

    public GameObject trajectoryPointPrefeb;
    List<GameObject> trajectoryPoints;

    

    public int everyNth = 3;
    public LayerMask whatToHit;

    GameObject ThrownStonesParent;

    Transform stoneThrower;
   

    void Start() {

       

        if (stonePref == null) {
            Debug.Log("No stone prefab added");
            return;
        }
        stoneMass = stonePref.GetComponent<Rigidbody2D>().mass;
        stoneThrower = transform.Find("stoneThrower");

        start = (Transform)stoneThrower.Find("startDirection");
        end = (Transform)stoneThrower.Find("endDirection");

        ThrownStonesParent = GameObject.Find("GroupThrownStones");

        if (isLocalPlayer) {
            trajectoryPoints = new List<GameObject>();
            for (int i = 0; i < vertCount; i++) {
                GameObject dot = (GameObject)Instantiate(trajectoryPointPrefeb);
                dot.transform.parent = this.transform;
                trajectoryPoints.Insert(i, dot);
            }
        }

       


        if (isServer) {
            // get assetId on an existing prefab
            NetworkHash128 stoneAssetId = stonePref.GetComponent<NetworkIdentity>().assetId;

            // register handlers for an existing prefab you'd like to custom spawn
            ClientScene.RegisterSpawnHandler(stoneAssetId, SpawnStone, UnSpawnStone);
        }

       


    }

    public GameObject  SpawnStone(Vector3 position, NetworkHash128 assetId) {
        Debug.Log("I am called, mat' va6u!");
        GameObject stone = (GameObject)Instantiate(stonePref, stoneThrower.position, stoneThrower.rotation);
        stone.GetComponent<StoneController>().stoneThrower = stoneThrower.gameObject;
        stone.GetComponent<StoneController>().teamNum = 69;
        return stone;
    }

    public void UnSpawnStone(GameObject spawned) {
        Destroy(spawned);
    }



    int maxBendAngle = 80;

    void Update() {


        if (!isLocalPlayer) {
            return;
        }

        Debug.DrawLine(start.position, end.position, Color.red);
        

        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel"))>0) {
       
            float deltaRot = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 50*100;
            float currAngle = stoneThrower.localEulerAngles.z;
            currAngle = (currAngle > 180) ? currAngle - 360 : currAngle;
            if (Mathf.Abs(currAngle + deltaRot) < maxBendAngle) {
                stoneThrower.Rotate(new Vector3(0, 0, deltaRot));
            }
         
        }

        if (Input.GetKeyDown("t")) {
          
            isDrawTrajectory = !isDrawTrajectory;
            foreach (GameObject go in trajectoryPoints) {
                go.GetComponent<SpriteRenderer>().enabled = isDrawTrajectory;
            }
        }

        if (Input.GetButton("Fire1") && Time.time > timeToFire) {//GetButton true while mouse pressed!
            timeToFire = Time.time + 1 / fireRate;
            throwPending = true;
        }

      
        if (isDrawTrajectory) {
            drawTraj(stoneThrower.position, (end.position - start.position).normalized * stoneSpeed * Time.fixedDeltaTime / stoneMass);
        }
        

    }


    void FixedUpdate() {
        if (throwPending) {
            throwPending = false;
            CmdThrowStone();//done by the server

        }
    }

   
    [Command]
    public void CmdThrowStone() {
        GameObject stone = Instantiate(stonePref, stoneThrower.position, stoneThrower.rotation);//Start on it not called before the method returns!
        stone.GetComponent<StoneController>().stoneThrower = stoneThrower.gameObject;


       // RpcTotalDebug(stone.GetComponent<StoneController>().stoneThrower!=null);

       // Debug.Log(stone.GetComponent<StoneController>().stoneThrower);
        stone.transform.parent = ThrownStonesParent.transform;
        Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();

        rb.velocity = (end.position - start.position).normalized * stoneSpeed * Time.fixedDeltaTime / (rb.mass);//same as rb.AddForce(v* stoneSpeed, ForceMode2D.Force);

        NetworkServer.Spawn(stonePref);

        
        StartCoroutine(WaitAndDestroy(stonePref));//if didn't collide with anything somehow 
   
    }




    [ClientRpc]
    void RpcTotalDebug(bool res) {
        Debug.Log(res);
    }

    IEnumerator WaitAndDestroy(GameObject obj) {
        yield return new WaitForSeconds(5f);
        NetworkServer.Destroy(obj);
    }





    void drawTraj(Vector2 pos, Vector2 velocity) {

       
        int count = 0;
        int i = -1;
        Vector2 prevPos = pos;
       
        while (count < vertCount) {
            i++;


            if (i%everyNth==0) {

                
                RaycastHit2D hit = Physics2D.Raycast(prevPos, pos, Vector3.Distance(prevPos, pos), whatToHit);

                if (hit.collider != null) {
                   
                    for (int j=count;j< vertCount;j++ ) {
                        trajectoryPoints[j].GetComponent<SpriteRenderer>().enabled = false;
                    }
                    break;
                }
                else {
                    trajectoryPoints[count].GetComponent<SpriteRenderer>().enabled = true;
                    trajectoryPoints[count].transform.position = pos;
                }

                  
                   
                count++;

            }

            prevPos = pos;

            velocity += Physics2D.gravity * Time.fixedDeltaTime/(stoneMass);
            pos += velocity * Time.fixedDeltaTime;
        }

   
}


}
