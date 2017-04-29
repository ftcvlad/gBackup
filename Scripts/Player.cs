using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


//[RequireComponent(typeof(MovementInput))]
public class Player : NetworkBehaviour {

    public float movementSpeed = 3.5f;
    public float jumpForce = 400f;

    public int maxHealth = 100;
    int currHealth;
    int teamId;
    public int numStonesPossessed;

    StatusBarManager statusBarManager;
    GameManagerScript gmInst;

    void Start() {
        currHealth = maxHealth;
        numStonesPossessed = 3;
        statusBarManager = transform.FindChild("StatusBar").GetComponent<StatusBarManager>();
        


        if (isServer) {
            gmInst = GameObject.Find("GM").GetComponent<GameManagerScript>();
            AllPlayerManager.addPlayer(this);
        }
       
    }

    public void setTeamId(int id) {
        teamId = id;
    }

    public int getTeamId() {
        return teamId;
    }

    [ClientRpc]
    public void RpcTakeDamage(int damage) {
        takeDamage(damage);
    }

    public void takeDamage(int damage) {
      


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


    void OnCollisionEnter2D(Collision2D collision) {

        ContactPoint2D contact = collision.contacts[0];//any contact point, can be >1


        //stone == otherCollider
        //rest == collider

        GameObject collTarg = collision.collider.gameObject;
        string collLayerName = LayerMask.LayerToName(collTarg.layer);
       
        if (collLayerName == "Item_to_pick_layer") {
           
            if (collTarg.tag == "item_stone") {

                

                if (isServer) {
                    gmInst.stoneFreeSpawnPoint(collTarg.GetComponent<itemStone_idx>().spawnPointIndex);
                }
               
                numStonesPossessed++;
                if (isLocalPlayer) {
                    ItemDisplayManager.updateStoneAmount(numStonesPossessed);
                }

                Destroy(collTarg);
               

                

            }

        }





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
