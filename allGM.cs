using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class RequestNextPlayerMessage : MessageBase {
    public NetworkInstanceId localPlayerNetId;
    public int currObservedPlayerId;

}

public class allGM : NetworkBehaviour {
   

    public static NetworkInstanceId localPlayerNetId;//set in Player
    static int currObservedPlayerId;
    static MovingPlayer currentMovingPlayer = null;
    static Player currentPlayer = null;

    static bool canSwitchCameraView = false;

    
    void Start() {
        NetworkManager.singleton.client.RegisterHandler(1000, switchPlayer);
    }


    void Update() {

        if (canSwitchCameraView) {

            if (Input.GetKeyDown(KeyCode.N)) {
                Debug.Log("n pressed");
               

                RequestNextPlayerMessage m = new RequestNextPlayerMessage();
                m.currObservedPlayerId = currObservedPlayerId;
                m.localPlayerNetId = localPlayerNetId;

                //send message to server
                ClientScene.FindLocalObject(localPlayerNetId).GetComponent<Player>().connectionToServer.Send(1001, m);
            }

        }
    }

   



    static void switchPlayer(NetworkMessage netMsg) {

      
        canSwitchCameraView = true;

        var msg = netMsg.ReadMessage<MyMessage>();

        if (currentPlayer != null) {
            currentMovingPlayer.enabled = false;
            currentPlayer.transform.Find("UIoverlay").gameObject.SetActive(false);
            //hide that player's UI frame
        }

        GameObject go = ClientScene.FindLocalObject(msg.netId);
        currentPlayer = go.GetComponent<Player>();
        currentMovingPlayer = go.GetComponent<MovingPlayer>();
        currObservedPlayerId = currentPlayer.getPlayerId();
        currentMovingPlayer.enabled = true;
        Transform uioverlay = currentPlayer.transform.Find("UIoverlay");
        uioverlay.gameObject.SetActive(true);


    }
}
