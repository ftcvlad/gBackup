using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBarManager : MonoBehaviour {


    Transform healthBar;
	void Start () {

        
        healthBar = (Transform)transform.Find("HealthBar");
	}
	
	public void updateHealthBar(int curHealth, int maxHealth) {


        float newScaleX = curHealth / (maxHealth*1.0f);

        healthBar.localScale = new Vector3(newScaleX, healthBar.localScale.y, healthBar.localScale.z);

    }
}
