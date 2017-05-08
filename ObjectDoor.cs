using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDoor : MonoBehaviour {


    Player enteringPlayer;
	void Start () {
		
	}
    void OnTriggerEnter2D(Collider2D other) {//when player finds key



        enteringPlayer = other.GetComponent<Player>();
        if (enteringPlayer.hasKey == true){

            //enteringPlayer.gameObject.SetActive(false);//??or on serveR?
            enteringPlayer.keyUsed();
            enteringPlayer.deactivatePlayer();
            



            if (enteringPlayer.isServer) {
              
                AllPlayerManager.playerFinished(enteringPlayer.getPlayerId());


                if (AllPlayerManager.isPlayersToEndReached()) {//end level

                    StartCoroutine(GameObject.Find("GM").GetComponent<ServerGM>().finishLevel());
                }
                else {

                    StartCoroutine(GameObject.Find("GM").GetComponent<ServerGM>().playerFinished(enteringPlayer));
                }
                //StartCoroutine(GameObject.Find("GM").GetComponent<ServerGM>().playerDeadOrFinished(enteringPlayer));
            }
            
        }



    }
}
