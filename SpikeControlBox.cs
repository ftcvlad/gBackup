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

    Color activatedColor = new Color();
    Color deactivatedColor = new Color();
    Text ownLetterText;


    void Start () {

        ColorUtility.TryParseHtmlString("#F7FF72FF", out activatedColor);
        ColorUtility.TryParseHtmlString("#B04312FF", out deactivatedColor);

        foreach (GameObject go in controlledBoxes) {
            go.transform.Find("Canvas").Find("Text").GetComponent<Text>().text = letter;
        }

        ownLetterText = transform.Find("Canvas").Find("Text").GetComponent<Text>();
        ownLetterText.text = letter;


        updateOwnColor();
      
       

    }

    public void updateOwnColor() {
        if (isActivated) {
            ownLetterText.color = activatedColor;
        }
        else {
            ownLetterText.color = deactivatedColor;
        }
    }
   


    

    
}
