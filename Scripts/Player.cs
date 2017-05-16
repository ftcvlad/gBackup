using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//[RequireComponent(typeof(MovementInput))]
public class Player : NetworkBehaviour {

    public float movementSpeed = 3.5f;
    public float jumpForce = 400f;

   
    public int maxHealth = 100;
    public int currHealth;
    [SyncVar]     public int team;
    [SyncVar]     public string playerName;
    [SyncVar]    public Color color;


   
    
    [SyncVar(hook = "updateStoneTileHook")] public int numStonesPossessed = 3;
    [SyncVar(hook = "updateHealthPotsTileHook")] public int numHealthPotsPossessed = 0;
    
    [SyncVar(hook = "updateGoldTileHook")] public int gold = 0;
    public bool isDead = false;
    public bool hasKey = false;

    StatusBarManager statusBarManager;
    ItemDisplayManager itemDispMan;
    Transform statusBar;
    Transform uioverlay;
    Transform playerBody;
    Transform graphics;
    Transform halo;

    [SerializeField]
    GameObject keyPref_inactive;

    System.Random rg = new System.Random(((int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) % 1000));


    Color deadColor;
    Color blinkColor;
   

    void Start() {


        


        currHealth = maxHealth;

        statusBar = transform.FindChild("StatusBar");
        uioverlay = transform.Find("UIoverlay");
        playerBody = transform.Find("PlayerBody");
        halo = playerBody.Find("HaloPoint");
        graphics = playerBody.Find("Graphics");
        statusBarManager = statusBar.GetComponent<StatusBarManager>();
        itemDispMan = uioverlay.Find("PersonalItemList").GetComponent<ItemDisplayManager>();
        
        if (!isLocalPlayer) {
            uioverlay.Find("ObservingMessage").Find("Text").GetComponent<Text>().text = "Observing player " + playerName + ". Press n to switch to next player.";
        }
        else {
            graphics.GetComponent<SpriteRenderer>().sortingOrder = 5;
            Destroy(uioverlay.Find("ObservingMessage").gameObject);
        }

   


        UnityEngine.Object.DontDestroyOnLoad(this.gameObject);


        deadColor = new Color();
        ColorUtility.TryParseHtmlString("#3A2424FF", out deadColor);
        blinkColor = new Color();
        ColorUtility.TryParseHtmlString("#FFFFFF58", out blinkColor);

        if (team != 0) {
            graphics.GetComponent<SpriteRenderer>().color = color;
          
        }
        else {//**if run scene without lobby
            Color[] Colors = new Color[] { Color.red, Color.green, Color.blue, Color.cyan, Color.yellow, Color.white };

            tempPlayerTot++;
            color = Colors[tempPlayerTot - 1];
            graphics.GetComponent<SpriteRenderer>().color = color;
            playerName = "Player-" + tempPlayerTot;
            team = tempPlayerTot;
        }

    }

    static int tempPlayerTot = 0;//**

    public override void OnStartLocalPlayer() {
        Debug.Log("2.  OnStartLocalPlayer");


        CmdAddPlayerOnServer(this.netId);

        //GetComponent().material.color = Color.blue;
        allGM.localPlayerNetId = transform.GetComponent<NetworkIdentity>().netId;


        StartCoroutine(askPlayerFrameUpdate());
       
    }


    IEnumerator askPlayerFrameUpdate() {
        yield return new WaitForSeconds(1f);
        CmdAskPlayerFrameUpdate();
    }

    [Command]
    void CmdAskPlayerFrameUpdate() {
        AllPlayerManager.updatePlayerFrame();
    }

    [Command]
    void CmdAddPlayerOnServer(NetworkInstanceId netId) {
        AllPlayerManager.addPlayer(NetworkServer.FindLocalObject(netId).GetComponent<Player>());
    }


  










    //ACTIVATE/DEACTIVATE PLAYER

    [ClientRpc]
    public void RpcDeactivateFinishedPlayer() {
        deactivateFinishedPlayer();
    }

    [ClientRpc]
    public void RpcActivatePlayer(Vector3 position) {
        setHealth(maxHealth);


        if (this.isLocalPlayer) {
            this.transform.position = position;
            activatePlayer();
        }
        else {
            StartCoroutine(lagActivate());
        }
       
    }

    //if isLocalPlayer==false its position is determined by other client. Give 1sec for NetworkTransform's interpolate to happen.
    //So, not local players appear after +1 sec
    IEnumerator lagActivate() {
        yield return new WaitForSeconds(1f);
        activatePlayer();
    }

    //cannot do p.SetActive(false) because then 1)Rpc cals will not work 2) NetworkTransform will not follow after reactivated (why??)
    public void deactivateAnyPlayer() {
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        statusBar.gameObject.SetActive(false);
        uioverlay.gameObject.SetActive(false);
       
        GetComponent<MovingPlayer>().enabled = false;//if someone was observing this player, camera just stops
        if (isLocalPlayer) {
            GetComponent<MovementInput>().enabled = false;
            GetComponent<ThrowStone>().showHideTrajectories(false);
        }
        GetComponent<ThrowStone>().enabled = false;

        GetComponent<PlayerActions>().stopDraggingBox();
    }


    public void deactivateFinishedPlayer() {
        deactivateAnyPlayer();

        playerBody.gameObject.SetActive(false);
        GetComponent<PolygonCollider2D>().enabled = false;
    }


    void deactivateDeadPlayer() {
        deactivateAnyPlayer();

        graphics.GetComponent<SpriteRenderer>().sortingOrder = 0;
        graphics.GetComponent<SpriteRenderer>().color = deadColor;
        StartCoroutine(lagDestroyCollider());
    }

    IEnumerator lagDestroyCollider() {
        yield return new WaitForSeconds(1f);
        GetComponent<PolygonCollider2D>().enabled = false;
    }

    [ClientRpc]
    public void RpcHideDeadPlayer() {
        SpriteRenderer sr = graphics.GetComponent<SpriteRenderer>();
        sr.color = color;//destroy after 1 sec to allow bullet on clients hit it (and make bullet disappear timely)
       
        if (isLocalPlayer) {
            sr.sortingOrder = 5;
        }
        else {
            sr.sortingOrder = 1;
        }
        playerBody.gameObject.SetActive(false);
    }

    public void activatePlayer() {

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName != "shop1" && isLocalPlayer) {
            GetComponent<ThrowStone>().enabled = true;
            GetComponent<ThrowStone>().showHideTrajectories(true);
        }
        GetComponent<Rigidbody2D>().isKinematic = false;
        GetComponent<PolygonCollider2D>().enabled = true;
        statusBar.gameObject.SetActive(true);
        playerBody.gameObject.SetActive(true);
       

        if (isLocalPlayer) {
            uioverlay.gameObject.SetActive(true);
            GetComponent<MovementInput>().enabled = true;
            GetComponent<MovingPlayer>().enabled = true;
           
        }
        else {
            GetComponent<MovingPlayer>().enabled = false;
        }
    }











    //EXIT SHOP
    [Command]
    public void CmdExitShop() {
        RpcDeactivateFinishedPlayer();
        AllPlayerManager.playerFinished(netId);

        if (AllPlayerManager.isPlayersToEndReached()) {//???
            StartCoroutine(ServerGM.finishLevel());
        }
    }

    [Command]
    public void CmdExitLevel() {
        RpcKeyUsed();
        RpcDeactivateFinishedPlayer();

        AllPlayerManager.playerFinished(netId);

        if (AllPlayerManager.isPlayersToEndReached()) {//end level
            StartCoroutine(ServerGM.finishLevel());
        }
        else {
            StartCoroutine(ServerGM.givePlayerToObserve(this));
        }

    }

   
   
            


      

    //KEY
    public void dropKey() {
      
        if (isServer) {
            GameObject key = Instantiate(keyPref_inactive, transform.position, Quaternion.identity);
            key.GetComponent<Rigidbody2D>().velocity = new Vector2((float)rg.NextDouble() * 2 - 1, 1) * 10;
            NetworkServer.Spawn(key);
            RpcDropKey();
        }

    }

    [ClientRpc]
    void RpcDropKey() {
       hasKey = false;
       itemDispMan.removeItem("key");
       halo.gameObject.SetActive(false);
    }

    public void keyFound() {
        hasKey = true;
        itemDispMan.addItem("key");
        halo.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void RpcKeyUsed() {
        hasKey = false;
        itemDispMan.removeItem("key");
        halo.gameObject.SetActive(false);
    }

  

    //STONES

    public void updateStoneTileHook(int newStoneNum) {
        itemDispMan.updateStoneAmount(newStoneNum);
        numStonesPossessed = newStoneNum;
    }

    [Command]
    public void CmdBuyStones(int stoneAmount, int cost) {
        gold -= cost;
        numStonesPossessed += stoneAmount;
    }

    //GOLD

    public void updateGoldTileHook(int newGoldValue) {
        itemDispMan.updateGold(newGoldValue);
    }



    //HEALTH POTS
    public void updateHealthPotsTileHook(int newHealthPotsNum) {
        
        itemDispMan.updateHealthPotsAmount(numHealthPotsPossessed, newHealthPotsNum);
        
        //1 was used
        if (numHealthPotsPossessed > newHealthPotsNum) {
            setHealth(Mathf.Min(maxHealth, currHealth + 50));//will make some class for health pot :)
        }

        numHealthPotsPossessed = newHealthPotsNum;
    }

    [Command]
    public void CmdUseHealthPot() {
        numHealthPotsPossessed--;
    }

    [Command]
    public void CmdBuyHealthPots(int amount, int cost) {
        gold -= cost;
        numHealthPotsPossessed += amount;
    }

    //DAMAGE

    public void reduceHealth(int damage) {
        currHealth = Mathf.Max(0, currHealth - damage);
        statusBarManager.updateHealthBar(currHealth, maxHealth);
    }

    public void setHealth(int health) {
        currHealth = health;
        statusBarManager.updateHealthBar(currHealth, maxHealth);
    }


    public void loseItemsAndGold() {
        gold = 0;
        numStonesPossessed = 0;
        numHealthPotsPossessed = 0;
        //TODO: all other items
    }
    


    [ClientRpc]
    public void RpcTakeDamage(int damageAmount) {
        reduceHealth(damageAmount);
        if (currHealth <= 0) {
            deactivateDeadPlayer();
            loseItemsAndGold();

            if (isServer) {
                ServerGM.handlePlayerDied(this);
            }
        }

    }

    public bool isDamageable = true;//only on server correct
    


    //SPIKES damage

    
    public void enterSpikes(int damage) {
        if (isDamageable) {
            StartCoroutine(checkSpikeCollisionRecursively(damage));
        }
    }

 

    IEnumerator checkSpikeCollisionRecursively(int damage) {

        GameObject go = PlayerActions.checkPlayerOverlapBox(transform,"SpikedBox");
      
        if (go!=null) {//if collides with any spikes
            isDamageable = false;
            RpcTakeDamageFromSpikes(damage);
        }
        else {
            isDamageable = true;
            RpcStopBlinkingPlayer();
            yield break;
        }
       
        yield return new WaitForSeconds(3f);
        StartCoroutine(checkSpikeCollisionRecursively(damage));
    }

    [ClientRpc]
    public void RpcTakeDamageFromSpikes(int damageAmount) {
        if (isLocalPlayer) {
            transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,300));
        }

        if (!IsInvoking("blinkPlayer")) {
            InvokeRepeating("blinkPlayer", 0, 0.2f);
        }
        reduceHealth(damageAmount);
        if (currHealth <= 0) {

            deactivateDeadPlayer();
            loseItemsAndGold();

            isDamageable = true;
            stopBlinkingPlayer();
            if (isServer) {
                ServerGM.handlePlayerDied(this);
            }
        }

    }

    void blinkPlayer() {
        if (graphics.GetComponent<SpriteRenderer>().color == blinkColor) {
            graphics.GetComponent<SpriteRenderer>().color = color;
        }
        else {
            graphics.GetComponent<SpriteRenderer>().color = blinkColor;
        } 
    }

    [ClientRpc]
    void RpcStopBlinkingPlayer() {
        stopBlinkingPlayer();
    }

    void stopBlinkingPlayer() {
        CancelInvoke("blinkPlayer");
        graphics.GetComponent<SpriteRenderer>().color = color;
    }


  

    //public void damagePlayer(int damageAmount) {//LOCAL VERSION. STONE TRAP NOT SYNCHRONISED :(
    //    if (hasKey) {
    //        dropKey();
    //    }
    //    reduceHealth(damageAmount);//health+status bar
    //    if (currHealth <= 0) {
    //        deactivatePlayer();//??? die animation
    //        loseItemsAndGold();

    //        if (isServer) {
    //            ServerGM.handlePlayerDied(this);
    //        }
    //    }
    //}

    //SETTERS/GETTERS


    public void setTeamId(int id) {
        team = id;
    }

    public int getTeamId() {
        return team;
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
