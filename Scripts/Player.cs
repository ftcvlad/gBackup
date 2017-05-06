﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

//[RequireComponent(typeof(MovementInput))]
public class Player : NetworkBehaviour {

    public float movementSpeed = 3.5f;
    public float jumpForce = 400f;

    public int maxHealth = 100;
    int currHealth;
    int teamId;
    [SyncVar] int playerId;
    [SyncVar(hook = "updateStoneTileHook")] public int numStonesPossessed = 3;

    StatusBarManager statusBarManager;
    public bool hasKey = false;

    ItemDisplayManager itemDispMan;

    [SerializeField]
    GameObject keyPref_inactive;

    System.Random rg = new System.Random(((int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) % 1000));

    void Start() {

        if (isServer) {
            AllPlayerManager.addPlayer(this);
        }

        currHealth = maxHealth;
       
        statusBarManager = transform.FindChild("StatusBar").GetComponent<StatusBarManager>();

        itemDispMan = transform.Find("UIoverlay").Find("PersonalItemList").GetComponent<ItemDisplayManager>();
        Transform uioverlay = transform.Find("UIoverlay");
        if (!isLocalPlayer) {
            uioverlay.Find("ObservingMessage").Find("Text").GetComponent<Text>().text = "Observing player " + playerId + ". Press n to switch to next player.";
        }
        else {
            Destroy(uioverlay.Find("ObservingMessage").gameObject);
        }

        
    }

    public override void OnStartLocalPlayer() {
        //GetComponent().material.color = Color.blue;
        allGM.localPlayerNetId = transform.GetComponent<NetworkIdentity>().netId;
    }

    
  



    public void dropKey() {
      
        if (isServer && hasKey) {
            GameObject key = Instantiate(keyPref_inactive, transform.position, Quaternion.identity);
            key.GetComponent<Rigidbody2D>().velocity = new Vector2((float)rg.NextDouble() * 2 - 1, 1) * 10;
            NetworkServer.Spawn(key);
            RpcDropKey();
        }

    }

    [ClientRpc]
    void RpcDropKey() {//+
        
        //TODO: this should happen with every player on the same team
       hasKey = false;
       itemDispMan.removeItem("key");
        
    }


    public void updateStoneTileHook(int newStoneNum) {
        itemDispMan.updateStoneAmount(newStoneNum);
    }

    //public void setStonesPossessed(int newAmount) {
    //    numStonesPossessed = newAmount;
    //    itemDispMan.updateStoneAmount(numStonesPossessed);
    //}

    public void keyFound() {
        hasKey = true;
      
        itemDispMan.addItem("key");
       
    }
  

    //[ClientRpc]
    //public void RpcKeyFound() {
       
    //    hasKey = true;
    //    if (isLocalPlayer) {
    //        itemDispMan.addItem("key");
    //    }

    //    //TODO: and the same for every team member!
    //}





    [ClientRpc]
    public void RpcTakeDamage(int damage) {
        takeDamage(damage);
    }
    void takeDamage(int damage) {
      
       
        currHealth = Mathf.Max(0, currHealth - damage);
        statusBarManager.updateHealthBar(currHealth, maxHealth);

        if (!isServer) {
            return;
        }

        if (currHealth <= 0) {
            //die 
            //RpcDied(); // remove prefab, enable camera follow some other prefab of choice
        }
    }


    


    public void setTeamId(int id) {
        teamId = id;
    }

    public int getTeamId() {
        return teamId;
    }

    public void setPlayerId(int id) {
        playerId = id;
    }

    public int getPlayerId() {
        return playerId;
    }

}

    

//    public int fallBoundary = -20;
//    public string deathSoundName = "deathVoice";
//    public string damageSoundName = "grunt";

//    private AudioManager audMan;

//    [SerializeField]
//    private StatusBar statusBar;

//    private PlayerStats ps;


//    void Start() {

//        ps = PlayerStats.instance;

//        ps.curHealth = ps.maxHealth;
//        if (statusBar == null) {
//            Debug.LogError("No status bar added to player!");
//        }
//        else {
//            statusBar.setHealth(ps.curHealth, ps.maxHealth);
//        }

//        audMan = AudioManager.instance;

//        GameMaster.gm.onToggleUpgradeMenu += onUpgradeMenuToggle;

//        InvokeRepeating("regenHealth", 1f / ps.heathRegenRate, 1f / ps.heathRegenRate);
//    }

//    public void regenHealth() {
//        ps.curHealth++;
//        statusBar.setHealth(ps.curHealth, ps.maxHealth);
//    }

//    public void damagePlayer(int damage) {
//        ps.curHealth -= damage;
//        if (ps.curHealth <= 0) {

//            audMan.playSound(deathSoundName);
//            GameMaster.killPlayer(this);
//        }
//        else {
//            audMan.playSound(damageSoundName);
//        }

//        statusBar.setHealth(ps.curHealth, ps.maxHealth);
//    }

//    void Update() {
//        if (transform.position.y <= fallBoundary) {
//            damagePlayer(1000);
//        }
//    }


//    void onUpgradeMenuToggle(bool active) { //handle what happens when upgrade menu is toggled

//        //enable/disable player movement
//        GetComponent<Platformer2DUserControl>().enabled = !active;

//        //enable/disable shooting
//        Weapon _weapon = GetComponentInChildren<Weapon>();
//        if (_weapon != null) {
//            _weapon.enabled = !active;
//        }

//    }

//    //when Player is destroyed, have to unregister from delegate, 
//    //so, that the method is not called on the destroyed Player
//    void OnDestroy() {
//        GameMaster.gm.onToggleUpgradeMenu -= onUpgradeMenuToggle;
//    }


//}
