using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowStone : MonoBehaviour {


    float timeToFire = 0;
    public float fireRate = 1;
    public float stoneSpeed = 300;

    Transform start;
    Transform end;
    [SerializeField]
    Transform stonePref;
    // Use this for initialization
    void Start() {
        if (stonePref == null) {
            Debug.Log("No stone prefab added");
        }

        start = (Transform)transform.Find("startDirection");
        end = (Transform)transform.Find("endDirection");

    }
    void Update() {



        Debug.DrawLine(start.position, end.position, Color.red);


        if (Input.GetKey("up")) {

            transform.Rotate(new Vector3(0, 0, 1));
        }
        else if (Input.GetKey("down")) {
            transform.Rotate(new Vector3(0, 0, -1));
        }


        if (Input.GetButton("Fire1") && Time.time > timeToFire) {//GetButton true while mouse pressed!
            timeToFire = Time.time + 1 / fireRate;
            throwStone();
        }
    }


    public void throwStone() {
        Transform stone = (Transform)Instantiate(stonePref, transform.position, transform.rotation);
        Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();

        // rb.AddForce(new Vector2(stoneSpeed, 0));

       Vector3 v = (end.position - start.position).normalized;

        rb.AddForce(v* stoneSpeed);
    }

}
