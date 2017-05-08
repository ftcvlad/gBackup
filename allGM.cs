using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    Transform resultsui;
    Transform table;
    Text headerText;

    public GameObject rowPref;

    public int timeToViewResults = 3;
    [SyncVar(hook = "updateTimeLeftUntilNextLevel")] public int timeUntilNextLevel = 3;

    


    void Start() {

       

        NetworkManager.singleton.client.RegisterHandler(1000, switchPlayer);

        resultsui = transform.Find("LevelResultsUI");
        table = resultsui.Find("Results").Find("Table");

        headerText = resultsui.Find("Results").Find("HeaderPanel").Find("Text").GetComponent<Text>();
        headerText.text = "Level Finished! Next level in " + timeToViewResults;

        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {//do initial level preparation for each respective level
     
        
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "shop1") {
            Vector3 staticCameraPos = GameObject.Find("CameraPositionPoint").transform.position;
            Camera.main.transform.position = new Vector3(staticCameraPos.x, staticCameraPos.y, staticCameraPos.z);
        }
       


    }

    void updateTimeLeftUntilNextLevel(int newValue) {

        if (newValue == 0) {
            resultsui.gameObject.SetActive(false);
        }
        else {
            headerText.text = "Level Finished! Next level in " + newValue;
        }
        
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

    [ClientRpc]
    public void RpcDisplayLevelResults(int[] allPlaces, int[] allPlayerNames, int[] allGoldWon) {

        //activate result frame
        //add row for each player result

        for (int i = 0; i < allPlaces.Length; i++) {
            Debug.Log("place: " + allPlaces[i] + " playerID:" + allPlayerNames[i] + " gold:" + allGoldWon[i]);



            GameObject g = GameObject.Instantiate(rowPref, table);
            g.transform.Find("Place").Find("Text").GetComponent<Text>().text = ""+allPlaces[i];
            g.transform.Find("PlayerName").Find("Text").GetComponent<Text>().text = "" + allPlayerNames[i];
            g.transform.Find("GoldWon").Find("Text").GetComponent<Text>().text = "" + allGoldWon[i];

            resultsui.gameObject.SetActive(true);
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
