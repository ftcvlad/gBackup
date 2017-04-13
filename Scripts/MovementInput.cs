using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;



    [RequireComponent(typeof (MovingPlayer))]
    public class MovementInput : MonoBehaviour
    {

        private MovingPlayer mover;
        private bool jump;

        private void Awake()
        {
            mover = GetComponent<MovingPlayer>();
        }

        private void Update()
        {
            if(!jump) 
                jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }

        private void FixedUpdate()
        {
        
            float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");

       
            mover.Move(h, jump);
            jump = false;
        }
    
}