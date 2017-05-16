using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ThrownStoneObjectPool : MonoBehaviour {

    public int poolSize = 20;
    public GameObject stonePref;
    public static GameObject[] allStones;

    public NetworkHash128 assetId { get; set; }

    public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetId);
    public delegate void UnSpawnDelegate(GameObject spawned);
    GameObject ThrownStonesParent;

    void Start() {

        ThrownStonesParent = GameObject.Find("GroupThrownStones");
        assetId = stonePref.GetComponent<NetworkIdentity>().assetId;
        allStones = new GameObject[poolSize];
        for (int i = 0; i < poolSize; ++i) {
            allStones[i] = (GameObject)Instantiate(stonePref, Vector3.zero, Quaternion.identity);
            allStones[i].name = "stoneToThrow" + i;
            allStones[i].transform.parent = ThrownStonesParent.transform;
            allStones[i].SetActive(false);
        }

        ClientScene.RegisterSpawnHandler(assetId, spawnStone, unspawnStone);
    }

    public static GameObject GetFromPool(Vector3 position) {
        foreach (var next in allStones) {
            if (!next.activeInHierarchy) {
                next.transform.position = position;
                next.SetActive(true);
                return next;
            }
        }
        return null;
    }

    public GameObject spawnStone(Vector3 position, NetworkHash128 assetId) {
        return GetFromPool(position);
    }

    public static void unspawnStone(GameObject spawned) {
        spawned.GetComponent<SpriteRenderer>().enabled = true;
        spawned.GetComponent<CircleCollider2D>().enabled = true;
        spawned.GetComponent<StoneController>().canHitPlayer = false;
        spawned.SetActive(false);
    }
}
