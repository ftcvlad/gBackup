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

            enteringPlayer.gameObject.SetActive(false);//??or on serveR?


            
            if (enteringPlayer.isServer) {
              
                AllPlayerManager.removePlayerById(enteringPlayer.getPlayerId());


                //if (AllPlayerManager.isLastInTeam(enteringPlayer.getTeamId())) {//last player finished --> change scene
                //    StartCoroutine(GameObject.Find("GM").GetComponent<ServerGM>().finishLevel());
                //}
                //else {//still more players left in team

                //    StartCoroutine(GameObject.Find("GM").GetComponent<ServerGM>().playerDeadOrFinished(enteringPlayer));  
                //}
                StartCoroutine(GameObject.Find("GM").GetComponent<ServerGM>().playerDeadOrFinished(enteringPlayer));
            }
            
        }



    }
}
