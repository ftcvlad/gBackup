using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemDisplayManager : MonoBehaviour {

    Text stoneAmountText;
    Transform stonesTile;
    Text goldText;
    Transform noStonesForeground;
    Transform healthPotTile;

    List<Tuple<string, Transform>>   allItems = new List<Tuple<string, Transform>>();

    int tileSideSize = 100;
    int offsetBetweenTiles = 10;

    [SerializeField]
    GameObject keyItemTilePref;

    [SerializeField]
    GameObject healthPotItemTilePref;//ssdsdf


    //and other tileItem prefabs


    void Start () {

        //stones init
        stonesTile = transform.Find("StonesTile");
        stoneAmountText = stonesTile.Find("amountText").GetComponent<Text>() ;
        noStonesForeground = stonesTile.Find("noStonesForeground");
        allItems.Add(new Tuple<string, Transform>("stones",stonesTile));//always [0] element
        updateStoneAmount(3);

        //gold init

        goldText = transform.Find("Gold").Find("GoldText").GetComponent<Text>();

        //disable UIoverlay if not local player
        Transform current = transform;
        Transform parentPlayer;
        while (true) {
            if (current.parent.tag == "Player") {
                parentPlayer = current.parent;
                break;
            }
            current = current.parent.transform;
        }

        if (parentPlayer.GetComponent<Player>().isLocalPlayer == false) {
            parentPlayer.Find("UIoverlay").gameObject.SetActive(false);
        }

    }

    public void updateGold(int newValue) {
        goldText.text = newValue+"";
    }
	
	public void updateStoneAmount(int val) {
        stoneAmountText.text = val+"";
        if (val == 0) {
            noStonesForeground.gameObject.SetActive(true);
        }
        else if (val>0) {
            noStonesForeground.gameObject.SetActive(false);
        }
    }


 
    public void updateHealthPotsAmount(int oldVal, int newVal) {


        if (oldVal == 0) {
            addItem("healthPot");
        }
        else if (newVal == 0) {
            removeItem("healthPot");
        }
        else {
            healthPotTile.Find("amountText").GetComponent<Text>().text = newVal + "";

        }
    }

    public void addItem(string itemName) {

        Vector2 position = new Vector2(allItems[0].second.localPosition.x + allItems.Count * (offsetBetweenTiles + tileSideSize), 0);

        if (itemName == "key") {
            
            GameObject newItemStone = Instantiate(keyItemTilePref, position, Quaternion.identity);
            newItemStone.transform.SetParent(transform, false);
            allItems.Add(new Tuple<string, Transform>("key", newItemStone.transform));
        }
        else if (itemName == "healthPot") {
            
            healthPotTile =  Instantiate(healthPotItemTilePref, position, Quaternion.identity).transform;
            healthPotTile.SetParent(transform, false);
            allItems.Add(new Tuple<string, Transform>("healthPot", healthPotTile));
        }
    }

    public void removeItem(string itemName) {


        for (int i=0;i< allItems.Count; i++) {
            if (allItems[i].first == itemName) {
                Destroy(allItems[i].second.gameObject);

                //move further items left
                for (int j = i + 1; j < allItems.Count; j++) {
                    allItems[i].second.position = new Vector2(allItems[i].second.localPosition.x-offsetBetweenTiles-tileSideSize, allItems[i].second.position.y);
                }

                allItems.RemoveAt(i);
                break;
            }
        }
    }


}
