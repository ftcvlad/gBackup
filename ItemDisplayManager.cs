using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplayManager : MonoBehaviour {

    static Text stoneAmountText;
	void Start () {
       

        stoneAmountText = transform.Find("StonesTile").Find("amountText").GetComponent<Text>() ;
        updateStoneAmount(3);
    }
	
	public static void updateStoneAmount(int val) {
        stoneAmountText.text = val+"";
    }
}
