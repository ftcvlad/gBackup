using UnityEngine;
using UnityEngine.Networking;


public class MovingPlayer : NetworkBehaviour {
    private bool facingRight = true;



    [SerializeField]    private bool airControl = true;
    [SerializeField]     private LayerMask whatIsGround;

    private Transform groundCheck;
    private Transform ceilingCheck; // A position marking where to check for ceilings

    private float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    [SerializeField]    private bool grounded = false;

    private float ceilingRadius = .01f;
    private Animator anim;

    private Transform playerGraphics;
    private Player p;

    //camera
    private Transform mainCamera;
    float cameraDistance = 10;
    float cameraHeight = 0;
    Vector3 cameraOffset;
    public float heightDamping = 2.0f;

    Rigidbody2D rb;

    float currMovSpeed;
    [SerializeField] int totalAvailableJumps = 2;
    int currJumps = 0;


    ScaleSpecialSnowflake sss;
    Transform playerBody;


    void Start() {

        
        mainCamera = Camera.main.transform;

        
        if (!isLocalPlayer) {
         
            Destroy(transform.GetComponent<MovementInput>());
            this.enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        //get player stuff
        playerBody = transform.Find("PlayerBody");
        groundCheck = playerBody.Find("GroundCheck");
        ceilingCheck = playerBody.Find("CeilingCheck");
        anim = GetComponent<Animator>();

        playerGraphics = playerBody.Find("Graphics");
        if (playerGraphics == null) {
            Debug.LogError("Player has no graphics object!");
        }

        p = transform.GetComponent<Player>();

      

      
        cameraOffset = new Vector3(0, cameraHeight, -(Mathf.Abs(transform.position.z) + cameraDistance));
        moveCamera();

        sss = transform.GetComponent<ScaleSpecialSnowflake>();
       
    }

    void OnEnable() {
       
        if (playerBody != null) {//after movingPlayer is enabled by viewer, make sure facingRight is correct
            facingRight = playerBody.localScale.x > 0 ? true : false;
        }
       
        cameraOffset = new Vector3(0, cameraHeight, -(Mathf.Abs(transform.position.z) + cameraDistance));
     
    }

    void OnDisable() {
       
    }
  

    public void Move(float move, bool jump) {//CALLED FROM FIXED UPDATE

        //anim.SetBool("Ground", grounded);

        //anim.SetFloat("vSpeed", GetComponent<Rigidbody2D>().velocity.y);

        // anim.SetFloat("Speed", Mathf.Abs(move));

        bool newGrounded = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);
        if (newGrounded && !grounded) {//landed for the 1st fixedUpdate
            currMovSpeed = p.movementSpeed;
            currJumps = 0;
        }
        grounded = newGrounded;

        if (!grounded ) {
            if ((move > 0 && !facingRight) || (move < 0 && facingRight)) {

                currMovSpeed /=1;// 2;
            }
        }
        
        rb.velocity = new Vector2(move * currMovSpeed, Mathf.Clamp(rb.velocity.y, -10, 10));//limit vertical acceleration




        if (move > 0 && !facingRight) Flip();
        else if (move < 0 && facingRight) Flip();

      

        // Jump
        if (currJumps< totalAvailableJumps && jump/* && anim.GetBool("Ground")*/) {
            currJumps++;
           
            //  anim.SetBool("Ground", false);

            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.angularVelocity = 0;
            rb.AddForce(new Vector2(0f, p.jumpForce));
        }

     
    }


    void LateUpdate() {
        
        moveCamera();//after physics calculations finished!
    }

    private void Flip() {

        facingRight = !facingRight;
        int factor = facingRight == true ? 1 : -1;

        playerBody.localScale = new Vector3(Mathf.Abs(playerBody.localScale.x) * factor, playerBody.localScale.y, playerBody.localScale.z);


        
        sss.CmdUpdateScaleServer(playerBody.localScale);
        
    }

   

    

    void moveCamera() {


        if (mainCamera != null) {
            float newYsmooth = Mathf.Lerp(mainCamera.position.y, transform.position.y + cameraOffset.y, heightDamping * Time.deltaTime);
            newYsmooth = Mathf.Clamp(newYsmooth, 0, Mathf.Infinity);
            mainCamera.position = new Vector3(transform.position.x + cameraOffset.x, newYsmooth, transform.position.z + cameraOffset.z);

        }




    }



}