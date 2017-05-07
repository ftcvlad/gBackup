using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectStoneBag : MonoBehaviour {

    bool playerClose = false;
    Player localPlayer = null;
    Transform uioverlay;
    int stonePrice = 49;
    Text minText;
    Text maxText;
    Text currText;
    Slider slider;
    void Start () {
     

        


        uioverlay = transform.Find("UIoverlay");

        Transform sliderArea = uioverlay.Find("Border").Find("InnerArea").Find("SliderArea");
        slider = sliderArea.Find("Slider").GetComponent<Slider>();
        minText = sliderArea.Find("MinText").GetComponent<Text>();
        maxText = sliderArea.Find("MaxText").GetComponent<Text>();
        currText =  sliderArea.Find("CurrentText").Find("text").GetComponent<Text>();
    }
	
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.B) && playerClose) {
         
            if (localPlayer == null) {
                GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
                Debug.Log(allPlayers.Length);
                foreach (GameObject go in allPlayers) {
                    if (go.GetComponent<Player>().isLocalPlayer) {
                        localPlayer = go.GetComponent<Player>();
                        break;
                    }
                }
            }

            //set min/max amount of stones that player can afford
            int maxAmountCanBuy = (int) Mathf.Floor(localPlayer.gold / stonePrice);
            maxText.text = "" + maxAmountCanBuy;
            slider.maxValue = maxAmountCanBuy;
            currText.text = "0";

            localPlayer.GetComponent<MovementInput>().enabled = false;
            uioverlay.gameObject.SetActive(true);
        }
    }


    public void OnSliderValueChange() {
        currText.text = ""+slider.value;
    }

    public void OnOkButtonPressed() {
       

        //buy stones

        localPlayer.GetComponent<MovementInput>().enabled = true;
        uioverlay.gameObject.SetActive(false);
    }

    public void OnCancelButtonPressed() {
       
        localPlayer.GetComponent<MovementInput>().enabled = true;
        uioverlay.gameObject.SetActive(false);
    }




    void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<Player>().isLocalPlayer){
            playerClose = true;
        }
      
    }


    void OnTriggerExit2D(Collider2D other) {
        if (other.GetComponent<Player>().isLocalPlayer) {
            playerClose = false;
        }
    }
}
