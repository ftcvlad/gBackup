using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spikes : MonoBehaviour {


  
    int damage = 20;
   

    float distanceFraction;//the smaller distance, the larger intervals in Lerp
    float t = 0;
    float timeToReachTarget = 1f;//time of transition

    Vector3 topTargetY = new Vector3(0, 0.34f, 0);
    Vector3 bottomTargetY = new Vector3(0, 0.04f, 0);
    Vector3 currentTarget;


    public void changeActivated(bool isActivated) {
    
        currentTarget = isActivated ? topTargetY : bottomTargetY;

        t = 0;
        distanceFraction = Mathf.Abs((transform.localPosition.y - currentTarget.y) / (topTargetY.y-bottomTargetY.y));

        if (!IsInvoking("doAnimation")) {
            InvokeRepeating("doAnimation", 0, Time.deltaTime);
        }
    }

 
    void doAnimation() {
        t += Time.deltaTime / (timeToReachTarget* distanceFraction);
        transform.localPosition = Vector3.Lerp(transform.localPosition, currentTarget, t);
        if (transform.localPosition == currentTarget) {
            CancelInvoke("doAnimation");
        }
        
    }

 

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            Player p = other.gameObject.GetComponent<Player>();
            if (p.isServer) {
                p.enterSpikes(damage);
            }
        } 
    }

 }
