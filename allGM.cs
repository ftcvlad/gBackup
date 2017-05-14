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

    Transform levelui;
    Transform resultsui;
    Transform table;
    Text headerText;
    Transform playercountui;
    Text activeText;
    Text finishedText;
    Text toEndText;

    public GameObject rowPref;

    public int timeToViewResults = 3;
    [SyncVar(hook = "updateTimeLeftUntilNextLevel")] public int timeUntilNextLevel = 3;


    override public void OnStartClient() {
        Debug.Log(" .allGm OnStartClient");
    }

    override public void OnStartServer() {
        Debug.Log(" .allGm OnStartServer");
    }

    void Start() {
        Debug.Log("  allGm Start");


        NetworkManager.singleton.client.RegisterHandler(1000, switchPlayer);

        //results
        levelui = transform.Find("LevelUI");
        resultsui = levelui.Find("Results");
        table = resultsui.Find("Table");
        headerText = resultsui.Find("HeaderPanel").Find("Text").GetComponent<Text>();
        headerText.text = "Level Finished! Next level in " + timeToViewResults;

        //players frame
        playercountui = levelui.Find("PlayerCountFrame");
        activeText = playercountui.Find("Active").Find("CountTextBackground").Find("CountText").GetComponent<Text>();
        finishedText = playercountui.Find("Finished").Find("CountTextBackground").Find("CountText").GetComponent<Text>();
        toEndText = playercountui.Find("ToEnd").Find("CountTextBackground").Find("CountText").GetComponent<Text>();

     
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
            playercountui.gameObject.SetActive(false);
            resultsui.gameObject.SetActive(false);
        }
        else {
            headerText.text = "Level Finished! Next level in " + newValue;
        }
        
    }

    void Update() {

        if (canSwitchCameraView) {

            if (Input.GetKeyDown(KeyCode.N)) {

                RequestNextPlayerMessage m = new RequestNextPlayerMessage();
                m.currObservedPlayerId = currObservedPlayerId;
                m.localPlayerNetId = localPlayerNetId;

                //send message to server
                ClientScene.FindLocalObject(localPlayerNetId).GetComponent<Player>().connectionToServer.Send(1001, m);
            }

        }
    }

    [ClientRpc]
    public void RpcUpdatePlayerCountFrame(int playersActive, int playersFinished, int playersToNextLevel) {

       
        activeText.text = playersActive + "";
        finishedText.text = playersFinished + "";
        toEndText.text = playersToNextLevel + "";

        if (playercountui.gameObject.activeSelf == false) {
            playercountui.gameObject.SetActive(true);
        }
    }


    [ClientRpc]
    public void RpcFinishGame() {
        transform.Find("LevelUI").Find("GameOverFrame").gameObject.SetActive(true);
    }

    public void OnGameOverButtonPressed() {
        //Debug.Log("GameOver");
        Application.Quit();
    }

    [ClientRpc]
    public void RpcDisplayLevelResults(int[] allPlaces, int[] allPlayerNames, int[] allGoldWon) {

        
        //delete previous results if any
        foreach (Transform child in table) {
            if (child.gameObject.name != "HeaderRow") {
                Destroy(child.gameObject);
            }
        }

        //activate result frame
        //add row for each player result
        for (int i = 0; i < allPlaces.Length; i++) {
            
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
