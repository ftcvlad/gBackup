using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dontDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
	}
	
}
