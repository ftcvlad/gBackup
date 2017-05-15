using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ServerGM : NetworkBehaviour {//EXISTS ONLY ON SERVER


    [SerializeField]
    GameObject itemStonePref;

    [SerializeField]
    GameObject keyPotPref;

    List<Transform> stone_spawnPositions;
    List<int> stone_freeIndexes;
    int stone_maxPresent;


    System.Random rg;
    float stone_TimeToSpawn = 3f;
    float stone_SpawnRate = 3f;//every 3 sec check if need to spawn stone
    float stoneSpawnNumberFactor = 0.8f;//how many stones should be present in % from # of StoneSpawnPoints
    int currStonesPresent = 0;
    Transform stoneSpawnPointsFolder;
    Transform keyPotSpawnPointsFolder;
    static allGM allGMInst;



    static ServerGM instanceSelf = null;
    static bool isCurrentSceneShop;
  


    private static ServerGM getInstance() {
        if (instanceSelf == null) {
            instanceSelf = GameObject.Find("GM").GetComponent<ServerGM>();
            return instanceSelf;
        }
        return instanceSelf;
    }

   
    void Awake() {//awake called before sceneLoaded unlike start
       
       
    }

    void Start() {
        if (!isServer) {//start is called when NetworkeIdentity object ServerGM is activated, i.e. when player spawns
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
            Destroy(this);
        }
        allGMInst = GameObject.Find("allGM").GetComponent<allGM>();
        rg = new System.Random(((int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) % 1000));
        instanceSelf = this;

        SceneManager.sceneLoaded += OnLevelFinishedLoading;//first time not called! (as it goes before start)
        levelInitialise(true);

    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        Debug.Log("OnLevelFinishedLoading");
        levelInitialise(false);
    }

    void levelInitialise(bool isFirstLevelEverLoaded) {

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "level1" || sceneName == "level2") {
            stoneSpawnPointsFolder = GameObject.Find("StoneSpawnPoints").transform;
            spawnInitialStones();

            keyPotSpawnPointsFolder = GameObject.Find("KeyPotSpawnPoints").transform;
            spawnKeyPot();

            isCurrentSceneShop = false;
        }
        else {
            isCurrentSceneShop = true;
        }


        if (!isFirstLevelEverLoaded) {
            StartCoroutine(AllPlayerManager.activatePlayers());
           
            
        }
       
    }
    
   


    public static bool isSceneShop() {
        return isCurrentSceneShop;
    }

    public void spawnKeyPot() {
        List<Transform> keyPot_spawnPoints = new List<Transform>();
        foreach (Transform child in keyPotSpawnPointsFolder) {
            if (child.gameObject.activeSelf) {
                keyPot_spawnPoints.Add(child);
            }
            
        }
        Transform loc = keyPot_spawnPoints[rg.Next(0, keyPot_spawnPoints.Count)];

        GameObject keyPot = Instantiate(keyPotPref, loc.position, Quaternion.identity);
        NetworkServer.Spawn(keyPot);

        
    }

    public void spawnInitialStones() {

        stone_spawnPositions = new List<Transform>();
        stone_freeIndexes = new List<int>();

        foreach (Transform child in stoneSpawnPointsFolder) {
            stone_spawnPositions.Add(child);
            stone_freeIndexes.Add(stone_freeIndexes.Count);
        }

        stone_maxPresent = (int) Mathf.Floor(stone_spawnPositions.Count* stoneSpawnNumberFactor);

        currStonesPresent = 0;
        spawnNstones(stone_maxPresent);

    }

    public void spawnNstones(int n) {

       

        if (n > stone_freeIndexes.Count) {
            n = stone_freeIndexes.Count;
        }


        Transform spawnLoc;
        int ind;
        for (int i = 0; i < n; i++) {

            ind = rg.Next(0, stone_freeIndexes.Count);
            spawnLoc = stone_spawnPositions[stone_freeIndexes[ind]];

            GameObject newItemStone = Instantiate(itemStonePref, spawnLoc.position, Quaternion.identity);//Start on it not called before the method returns!
            newItemStone.GetComponent<itemStone_idx>().spawnPointIndex = stone_freeIndexes[ind];

            newItemStone.transform.parent = stoneSpawnPointsFolder;
            //spawn stones on clients
            NetworkServer.Spawn(newItemStone);

            stone_freeIndexes.RemoveAt(ind);
        }
        currStonesPresent += n;

    }


    public void stoneFreeSpawnPoint(int index) {
        stone_freeIndexes.Add(index);
        currStonesPresent--;
    }



    void Update() {

        if (!isCurrentSceneShop) {
            if (Time.time > stone_TimeToSpawn) {//GetButton true while mouse pressed!
                stone_TimeToSpawn = Time.time + stone_SpawnRate;
                if (currStonesPresent < stone_maxPresent) {
                    spawnNstones(1);
                }
            }
        }
        
    }

   

    void reduceTimeUntilNextLevel() {
        allGMInst.timeUntilNextLevel -= 1;
        if (allGMInst.timeUntilNextLevel==-1) {
            allGMInst.timeUntilNextLevel = allGMInst.timeToViewResults;
            CancelInvoke("reduceTimeUntilNextLevel");

            serverChangeScene();

        }
    }


   

    static void serverChangeScene() {

        //could use some structure, but harder to develop from scene >1...
        string nextSceneName=null;
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "level1") {
            nextSceneName = "shop1";
        }
        else if (sceneName == "shop1") {
            nextSceneName = "level2";
        }
        else if (sceneName == "level2") {//finish the game
            allGMInst.RpcFinishGame();

            return;
        }

        
        GameObject go = GameObject.FindGameObjectWithTag("SingleNetworkManager");

        if (go.name == "NetMan") {//development
            go.GetComponent<NetworkManager>().ServerChangeScene(nextSceneName);
        }
        else if (go.name == "LobbyManager") {//production :)
            go.GetComponent<NetworkLobbyManager>().ServerChangeScene(nextSceneName);
        }
    }


    public static IEnumerator finishLevel() {

        yield return new WaitForSeconds(1f);//make a little pause between player finishing and displaying results


        if (isCurrentSceneShop == false) {
            string sceneName = SceneManager.GetActiveScene().name;
            int totalTeamPrize = 0;
            int perPlaceGoldStep = 0;
            int goldForUnfinished = 0;
            if (sceneName == "level1") {
                totalTeamPrize = 400;
                perPlaceGoldStep = 10;
                goldForUnfinished = 5;
            }
            else if (sceneName == "level2") {
                totalTeamPrize = 500;
                perPlaceGoldStep = 50;
                goldForUnfinished = 25;
            }

            //calculate scores
            PlayerResult result = AllPlayerManager.calculateResults(totalTeamPrize, perPlaceGoldStep, goldForUnfinished);
            //deactivate remaining players
            AllPlayerManager.finishRemainingPlayers();

            allGMInst.RpcDisplayLevelResults(result.allPlaces, result.allPlayerNetIds, result.allGoldWon);

            instanceSelf.InvokeRepeating("reduceTimeUntilNextLevel", 1, 1f);
        }
        else {
            serverChangeScene();
        }
        
    }

    public static IEnumerator givePlayerToObserve(Player p) {
        yield return new WaitForSeconds(2f);


        AllPlayerManager.givePlayerToObserve(p.netId);

    }

    public static void handlePlayerDied(Player p) {
        AllPlayerManager.playerDied(p.netId);//update arrays; update player counts;

        allGMInst.RpcUpdatePlayerCountFrame(AllPlayerManager.playersActive, AllPlayerManager.playersFinished, AllPlayerManager.playersToNextLevel);
        if (AllPlayerManager.playersFinished + AllPlayerManager.playersActive > 0) {

            if (AllPlayerManager.isPlayersToEndReached()) {//end level
                instanceSelf.StartCoroutine(ServerGM.finishLevel());
            }
            else {
                instanceSelf.StartCoroutine(ServerGM.givePlayerToObserve(p));
            }

        }
        else {//game over
            allGMInst.RpcFinishGame();
        }

       
    } 
}


    public class MyMessage : MessageBase {
        public NetworkInstanceId netId;

    }

public struct PlayerResult {

    public NetworkInstanceId[] allPlayerNetIds { get; set; }
    public int[] allPlaces { get; set; }
    public int[] allGoldWon { get; set; }

}

