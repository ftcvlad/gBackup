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

    private void Awake() {

       

       
    }


    void Start() {

        if (!isLocalPlayer) {

           

            Destroy(transform.GetComponent<Player>());
            Destroy(transform.GetComponent<MovementInput>());
            Destroy(this);



            return;
        }

        //get player stuff
        groundCheck = transform.Find("GroundCheck");
        ceilingCheck = transform.Find("CeilingCheck");
        anim = GetComponent<Animator>();

        playerGraphics = transform.FindChild("Graphics");
        if (playerGraphics == null) {
            Debug.LogError("Player has no graphics object!");
        }

        p = transform.GetComponent<Player>();

        //camera
        mainCamera = Camera.main.transform;

      
        cameraOffset = new Vector3(0, cameraHeight, -(Mathf.Abs(transform.position.z) + cameraDistance));
        moveCamera();
    }

    private void FixedUpdate() {

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);

        //anim.SetBool("Ground", grounded);

        //anim.SetFloat("vSpeed", GetComponent<Rigidbody2D>().velocity.y);
      
    }


    public void Move(float move, bool jump) {


        if (grounded || airControl) {

           // anim.SetFloat("Speed", Mathf.Abs(move));

            // Move the character
          GetComponent<Rigidbody2D>().velocity = new Vector2(move * p.movementSpeed, GetComponent<Rigidbody2D>().velocity.y);

      


            if (move > 0 && !facingRight) Flip();
            else if (move < 0 && facingRight) Flip();
        }


      
        // Jump
        if (grounded && jump/* && anim.GetBool("Ground")*/) {
           
            grounded = false;
          //  anim.SetBool("Ground", false);
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, p.jumpForce));
        }

     
    }


    void LateUpdate() {
        moveCamera();//after physics calculations finished!
    }

    private void Flip() {

        facingRight = !facingRight;

        Vector3 theScale = playerGraphics.localScale;
        theScale.x *= -1;
        playerGraphics.localScale = theScale;
    }


    void moveCamera() {

        if (isLocalPlayer) {//this is called once from LateUpdate before the script is deleted (or hz why) (isLocalPlayer==false)
            Vector3 newPos = new Vector3(transform.position.x + cameraOffset.x, transform.position.y + cameraOffset.y, transform.position.z + cameraOffset.z);
            mainCamera.position = newPos;
        }
     
        
        //mainCamera.LookAt(transform);
    }

}