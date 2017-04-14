using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowStone : MonoBehaviour {


    float timeToFire = 0;
    public float fireRate = 1;
    public float stoneSpeed = 400;

    Transform start;
    Transform end;
    [SerializeField]   Transform stonePref;
    float stoneMass;

    bool throwPending = false;//input in update, Physics in FixedUpdate

    int vertCount = 40;

    bool isDrawTrajectory = true;

    public GameObject trajectoryPointPrefeb;
    List<GameObject> trajectoryPoints;

    

    public int everyNth = 3;
    public LayerMask whatToHit;

    void Start() {
        if (stonePref == null) {
            Debug.Log("No stone prefab added");
        }
        stoneMass = stonePref.GetComponent<Rigidbody2D>().mass;

        start = (Transform)transform.Find("startDirection");
        end = (Transform)transform.Find("endDirection");

        trajectoryPoints = new List<GameObject>();
        for (int i = 0; i < vertCount; i++) {
            GameObject dot = (GameObject)Instantiate(trajectoryPointPrefeb);
            dot.transform.parent = this.transform;
            trajectoryPoints.Insert(i, dot);
        }
     





    }
    void Update() {


        Debug.DrawLine(start.position, end.position, Color.red);


        if (Input.GetKey("up")) {
            transform.Rotate(new Vector3(0, 0, 1));
        }
        else if (Input.GetKey("down")) {
            transform.Rotate(new Vector3(0, 0, -1));
        }
        else if (Input.GetKeyDown("t")) {
         
            foreach (GameObject go in trajectoryPoints) {
                isDrawTrajectory = !isDrawTrajectory;
                go.GetComponent<SpriteRenderer>().enabled = isDrawTrajectory;
            }
        }

        if (Input.GetButton("Fire1") && Time.time > timeToFire) {//GetButton true while mouse pressed!
            timeToFire = Time.time + 1 / fireRate;
            throwPending = true;
        }

      
        if (isDrawTrajectory) {
            drawTraj(transform.position, (end.position - start.position).normalized * stoneSpeed * Time.fixedDeltaTime / stoneMass);
        }
        

    }


    void FixedUpdate() {
        if (throwPending) {
            throwPending = false;
            throwStone();

        }
    }

   
  
    public void throwStone() {
        Transform stone = (Transform)Instantiate(stonePref, transform.position, transform.rotation);
        stone.parent = this.transform;
        Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();

        rb.velocity = (end.position - start.position).normalized * stoneSpeed * Time.fixedDeltaTime / (rb.mass);//same as rb.AddForce(v* stoneSpeed, ForceMode2D.Force);

        Destroy(stone.gameObject, 5f);//if didn't collide with anything somehow 
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
