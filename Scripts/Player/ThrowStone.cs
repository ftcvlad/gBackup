using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class ThrowStone : NetworkBehaviour {


    float timeToFire = 0;
    public float fireRate = 1;
    public float stoneSpeed = 500;

    Transform start;
    Transform end;
    [SerializeField]   GameObject stonePref;
    Rigidbody2D rb;
    float stoneMass;

    bool throwPending = false;//input in update, Physics in FixedUpdate

    int vertCount = 40;

    bool isDrawTrajectory = true;

    public GameObject trajectoryPointPrefeb;
    List<GameObject> trajectoryPoints;

    

    public int everyNth = 3;
    public LayerMask whatToHit;


    Player playerInst;
    Transform arm;
    Transform playerBody;

 
  
    void Start() {
  
        //DEVELOPEMENT
        if (SceneManager.GetActiveScene().name=="shop1") {
            this.enabled = false;
        }//ZZZZZZZZZ
       
        if (stonePref == null) {
            Debug.Log("No stone prefab added");
            return;
        }
        rb = stonePref.GetComponent<Rigidbody2D>();
        stoneMass =rb.mass;

        playerInst = transform.GetComponent<Player>();
        playerBody = transform.Find("PlayerBody");
        arm = playerBody.Find("arm");
        start = (Transform)arm.Find("startDirection");
        end = (Transform)arm.Find("endDirection");


        //ThrownStonesParent = GameObject.Find("GroupThrownStones");

        if (isLocalPlayer) {
            trajectoryPoints = new List<GameObject>();
            for (int i = 0; i < vertCount; i++) {
                GameObject dot = (GameObject)Instantiate(trajectoryPointPrefeb);
                dot.transform.parent = this.transform;
                trajectoryPoints.Insert(i, dot);
            }
        }


    }

  


    int maxBendAngle = 80;

    void Update() {


        if (!isLocalPlayer) {
            return;
        }

        Debug.DrawLine(start.position, end.position, Color.red);
        

        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel"))>0) {
       
            float deltaRot = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 45*100;
            float currAngle = arm.localEulerAngles.z;
            currAngle = (currAngle > 180) ? currAngle - 360 : currAngle;
            if (Mathf.Abs(currAngle + deltaRot) < maxBendAngle) {
                arm.Rotate(new Vector3(0, 0, deltaRot));
            }
            else {
                if (deltaRot > 0) {
                    deltaRot = maxBendAngle - currAngle;
                }
                else {
                    deltaRot = -(maxBendAngle + currAngle);
                }

                arm.Rotate(new Vector3(0, 0, deltaRot));
              
            }
       


        }

        if (Input.GetKeyDown("t")) {
          
            isDrawTrajectory = !isDrawTrajectory;
            foreach (GameObject go in trajectoryPoints) {
                go.GetComponent<SpriteRenderer>().enabled = isDrawTrajectory;
            }
        }


        if (playerInst.numStonesPossessed > 0) {
            if (Input.GetButton("Fire1") && Time.time > timeToFire) {//GetButton true while mouse pressed!
                timeToFire = Time.time + 1 / fireRate;
                throwPending = true;

            }
        }
        

      
        if (isDrawTrajectory) {//arm.position
            drawTraj(start.position, (end.position - start.position).normalized * stoneSpeed * Time.fixedDeltaTime / stoneMass);
        }
        

    }

    public void showHideTrajectories(bool show) {
        foreach (GameObject go in trajectoryPoints) {
            go.GetComponent<SpriteRenderer>().enabled = show;
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

        

        if (playerInst.numStonesPossessed > 0) {//sort of for anti-cheating on client
            GameObject stone = ThrownStoneObjectPool.GetFromPool(start.position);

            if (stone == null) {//no objects in pull, shouldn't happen
                Debug.LogError("no objects in pool, increase pool size");
                return;
            }

            playerInst.numStonesPossessed -= 1;//syncVar changed, and hook called on server and clients
           

            Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
            rb.velocity = (end.position - start.position).normalized * stoneSpeed * Time.fixedDeltaTime / (rb.mass);//same as rb.AddForce(v* stoneSpeed, ForceMode2D.Force);

            NetworkServer.Spawn(stone);

            StartCoroutine(WaitAndUnspawn(stone));//1) if didn't collide with anything 2) if collided and was set invisible

        }

    }




    IEnumerator WaitAndUnspawn(GameObject obj) {
        yield return new WaitForSeconds(3f);
        ThrownStoneObjectPool.unspawnStone(obj);//on server
        NetworkServer.UnSpawn(obj);//on clients
    }

   

    void drawTraj(Vector2 pos, Vector2 velocity) {



        for (int j = 0; j < 5; j++) {
            velocity += Physics2D.gravity * rb.gravityScale * Time.fixedDeltaTime / (stoneMass);
            pos += velocity * Time.fixedDeltaTime;
        }


        int count = 0;
        int i = -1;
        Vector2 prevPos = pos;
       
        while (count < vertCount) {
            i++;

          

            if ((i%everyNth)==0) {
               
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

          
            velocity += Physics2D.gravity * rb.gravityScale* Time.fixedDeltaTime/(stoneMass);
            pos += velocity * Time.fixedDeltaTime;
        }
     

    }


}
