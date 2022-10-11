using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Player move variables below 

    Vector2 moveInput;
    Rigidbody2D myRb;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;

    public int numberOfJumps = 1;
    private int numberOfJumpsLeft;

    public float groundCheckRadius;
    public float movX;

    public bool Grounded;
    private bool canJump;
    
    [Header("Dash variables")]
    public float dashForce;
    public float startDashCoolDown;
    private float currentDashCoolDown;
    private float dashDirection;
    private bool isDashing;

    [SerializeField] private LayerMask GroundLayerMask;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 8f;

    public Transform groundCheck;

    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        numberOfJumpsLeft = numberOfJumps;
    }

    void Update()
    {
        Run();
        FlipSprite();
        CheckIfCanJump();

        movX = Input.GetAxis("Horizontal");

        if(Input.GetKeyDown(KeyCode.LeftShift) && movX != 0){
            isDashing = true;
            currentDashCoolDown = startDashCoolDown;
            myRb.velocity = Vector2.zero;
            dashDirection = (int)movX;
        }

        if(isDashing){
            myRb.velocity = transform.right * dashDirection * dashForce;

            currentDashCoolDown -= Time.deltaTime;

            if(currentDashCoolDown <= 0){
                isDashing = false;
            }
        }
    }

    private void FixedUpdate() {
        CheckSurroundings();
        UpdateAnimations();
    }

    //This is function to move left & right

    void OnMove(InputValue value){
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    /*Function to jump below. Had tried including ground check in same function but found better way. canJump is used to determine that 
    all requirements are met in order to jump. numberOfJumpsLeft is used to determine the amount of jumps left and to later include
    an item for a double jump*/

    void OnJump(InputValue value){
        /*if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            return;
        }*/
        if (canJump && Input.GetKeyDown(KeyCode.Space)){
            myRb.velocity = Vector2.up * jumpSpeed;
            numberOfJumpsLeft--;
        }
    }

    //Function below used to determine character animations

    private void UpdateAnimations(){
        myAnimator.SetBool("Grounded", Grounded);
        myAnimator.SetFloat("yVelocity", myRb.velocity.y);
        myAnimator.SetBool("isDashing", isDashing);
    }

    //Another attempt at groundCheck that did not quite work out.
    /*private bool isGrounded(){
        float extraHeightText = 1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(myFeetCollider.bounds.center, myFeetCollider.bounds.size, 0f, Vector2.down, extraHeightText, GroundLayerMask);
        Color rayColor;
        if(raycastHit.collider != null){
            rayColor = Color.green;
           } else{
            rayColor = Color.red;
            }
            return raycastHit.collider != null;        
    }*/

    //Function below used to make player move left and right. isRunning is used in order to apply animations to the movement. 

    void Run(){
        Vector2 playerVelocity = new Vector2 (moveInput.x * runSpeed, myRb.velocity.y);
        myRb.velocity = playerVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(myRb.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    //Function below used to flip sprite and ensure that it remains faced in the direction where idle is commmenced.

    void FlipSprite(){
        bool playerHasHorizontalSpeed = Mathf.Abs(myRb.velocity.x) > Mathf.Epsilon;
        if(playerHasHorizontalSpeed){
        transform.localScale = new Vector2 (Mathf.Sign(myRb.velocity.x), 1f);
        }
    }

    //Function below is necessary in order to apply overlap circle below player that checks for ground layer.

    private void CheckSurroundings(){
        Grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, GroundLayerMask);
    }

    /*Function below is used to prevent infinite jumping. The player can only jump if it is touching ground unless specified in 
    numberOfJumps variable*/

    private void CheckIfCanJump(){
        if(Grounded && myRb.velocity.y <= 0){
            numberOfJumpsLeft = numberOfJumps;
        }
        if(numberOfJumpsLeft <= 0){
            canJump = false;
        } else {
            canJump = true;
        }
    }

    //Function below is to show overlap circle in debug so that it can be manipulated and placed under player's feet. 
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
