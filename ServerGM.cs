using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;


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
    void Start () {

       
        if (!isServer) {
            Destroy(this);
        }
        int currMillis = ((int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) % 1000);
        rg = new System.Random(currMillis);


        stoneSpawnPointsFolder = GameObject.Find("StoneSpawnPoints").transform;
        spawnInitialStones();

        keySpawnPointsFolder = GameObject.Find("KeySpawnPoints").transform;
        spawnKey();
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
            stone_TimeToSpawn = Time.time+ 1/ stone_SpawnRate;
          
            if (currStonesPresent < stone_maxPresent  ) {
                spawnNstones(1);
            }
        }



    }

}
