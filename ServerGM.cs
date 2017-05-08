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
    GameObject keyPref_active;

    List<Transform> stone_spawnPositions = new List<Transform>();
    List<int> stone_freeIndexes = new List<int>();
    int stone_maxPresent = 5;


    System.Random rg;
    float stone_TimeToSpawn = 3f;//every 3 sec check if need to spawn stone
    float stone_SpawnRate = 1 / 3f;
    int currStonesPresent = 0;
    Transform stoneSpawnPointsFolder;
    Transform keySpawnPointsFolder;
    allGM allGMInst;

    void Start() {


        if (!isServer) {
            Destroy(this);
        }
        int currMillis = ((int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) % 1000);
        rg = new System.Random(currMillis);


        stoneSpawnPointsFolder = GameObject.Find("StoneSpawnPoints").transform;
        spawnInitialStones();

        keySpawnPointsFolder = GameObject.Find("KeySpawnPoints").transform;
        spawnKey();

        allGMInst = GameObject.Find("allGM").GetComponent<allGM>();

        SceneManager.sceneLoaded += OnLevelFinishedLoading;

        UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
    }


    public void spawnKey() {
        List<Transform> key_spawnPoints = new List<Transform>();
        foreach (Transform child in keySpawnPointsFolder) {
            key_spawnPoints.Add(child);
        }
        Transform loc = key_spawnPoints[rg.Next(0, key_spawnPoints.Count)];

        GameObject key = Instantiate(keyPref_active, loc.position, Quaternion.identity);

        key.GetComponent<itemKey>().isPickable = true;//on server
        NetworkServer.Spawn(key);

        // transform.GetComponent<allGM>().RpcMakeKeyPickable();//on clients
    }

    public void spawnInitialStones() {


        foreach (Transform child in stoneSpawnPointsFolder) {
            stone_spawnPositions.Add(child);
            stone_freeIndexes.Add(stone_freeIndexes.Count);
        }

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


        if (Time.time > stone_TimeToSpawn) {//GetButton true while mouse pressed!
            stone_TimeToSpawn = Time.time + 1 / stone_SpawnRate;

            if (currStonesPresent < stone_maxPresent) {
                spawnNstones(1);
            }
        }
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {

        if (scene.name == "shop1") {
            //

            StartCoroutine(AllPlayerManager.activateAlivePlayers());
        }

    }

    void reduceTimeUntilNextLevel() {
        allGMInst.timeUntilNextLevel -= 1;
        if (allGMInst.timeUntilNextLevel==0) {
            allGMInst.timeUntilNextLevel = allGMInst.timeToViewResults;
            CancelInvoke("reduceTimeUntilNextLevel");




            ////change scene
            GameObject go = GameObject.FindGameObjectWithTag("SingleNetworkManager");

            if (go.name == "NetMan") {//development
                go.GetComponent<NetworkManager>().ServerChangeScene("shop1");
            }
            else if (go.name == "LobbyManager") {//production :)
                go.GetComponent<NetworkLobbyManager>().ServerChangeScene("shop1");
            }
        }
    }

    public IEnumerator finishLevel(int winningTeamId) {

        yield return new WaitForSeconds(1f);//make a little pause between player finishing and displaying results

        string sceneName= SceneManager.GetActiveScene().name;
        int totalTeamPrize = 0;
        int perPlaceGoldStep = 0;
        int goldForUnfinished = 0;
        if (sceneName == "level1") {
            totalTeamPrize = 400;
            perPlaceGoldStep = 10;
            goldForUnfinished = 5;
        }

        //calculate scores

        PlayerResult result = AllPlayerManager.calculateResults(winningTeamId, totalTeamPrize, perPlaceGoldStep, goldForUnfinished);
        //deactivate remaining players
        AllPlayerManager.finishRemainingPlayers();

        allGMInst.RpcDisplayLevelResults(result.allPlaces, result.allPlayerIds, result.allGoldWon);

        InvokeRepeating("reduceTimeUntilNextLevel", 1, 1f);
       


 
       
    }

    public IEnumerator playerFinished(Player p) {
        yield return new WaitForSeconds(2f);


        AllPlayerManager.givePlayerToObserve(p.netId);

    }

    public IEnumerator playerDead(Player p) {
        yield return new WaitForSeconds(2f);

        p.isDead = true;

        AllPlayerManager.givePlayerToObserve(p.netId);


    }
}


    public class MyMessage : MessageBase {
        public NetworkInstanceId netId;

    }

public struct PlayerResult {

    public int[] allPlayerIds { get; set; }
    public int[] allPlaces { get; set; }
    public int[] allGoldWon { get; set; }

}

