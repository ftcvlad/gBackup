using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SpikeControlBox : NetworkBehaviour {

   
    public List<GameObject> controlledBoxes = new List<GameObject>();


    public bool isActivated = true;

    [SerializeField]
    string letter;


	void Start () {
        foreach (GameObject go in controlledBoxes) {
            go.transform.Find("Canvas").Find("Text").GetComponent<Text>().text = letter;
        }
	}

   


    

    
}
