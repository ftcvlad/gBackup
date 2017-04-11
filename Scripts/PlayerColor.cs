using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerColor : NetworkBehaviour {
    [SyncVar] public Color color;

    SpriteRenderer sr;

	// Use this for initialization
	void Start () {
        sr = transform.Find("Graphics").GetComponent<SpriteRenderer>();
        sr.material.color = color;
	}
	

}
