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
    Text goldText;
    Slider slider;
    void Start () {
 
        uioverlay = transform.Find("UIoverlay");

        Transform sliderArea = uioverlay.Find("Border").Find("InnerArea").Find("SliderArea");
        slider = sliderArea.Find("Slider").GetComponent<Slider>();
        minText = sliderArea.Find("MinText").GetComponent<Text>();
        maxText = sliderArea.Find("MaxText").GetComponent<Text>();
        currText =  sliderArea.Find("CurrentText").Find("text").GetComponent<Text>();
        goldText = sliderArea.Find("CoinsLeft").Find("Text").GetComponent<Text>();
    }
	
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.B) && playerClose) {
         


            //set min/max amount of stones that player can afford
            int maxAmountCanBuy = (int) Mathf.Floor(localPlayer.gold / stonePrice);
            goldText.text = ""+localPlayer.gold;
            maxText.text = "" + maxAmountCanBuy;
            slider.maxValue = maxAmountCanBuy;
            currText.text = "0";

            localPlayer.GetComponent<MovementInput>().enabled = false;
            uioverlay.gameObject.SetActive(true);
        }
    }


    public void OnSliderValueChange() {
        currText.text = ""+slider.value;
        goldText.text = (localPlayer.gold - slider.value*stonePrice)+"";
    }

    public void OnOkButtonPressed() {


        //buy stones

        localPlayer.CmdBuyStones((int)slider.value, (int)slider.value*stonePrice);

        localPlayer.GetComponent<MovementInput>().enabled = true;
        uioverlay.gameObject.SetActive(false);
    }

    public void OnCancelButtonPressed() {
       
        localPlayer.GetComponent<MovementInput>().enabled = true;
        uioverlay.gameObject.SetActive(false);
    }




    void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<Player>().isLocalPlayer){
            if (localPlayer == null) {
                localPlayer = other.GetComponent<Player>();
            }
            playerClose = true;
        }
      
    }


    void OnTriggerExit2D(Collider2D other) {
        if (other.GetComponent<Player>().isLocalPlayer) {
            playerClose = false;
        }
    }
}
