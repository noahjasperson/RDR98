using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private InputManager _input;
    private CharacterController _controller;
    
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [SerializeField] private float runSpeed = 5;
    [SerializeField] private float sprintSpeed = 8;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float jumpHeight = .5f;
    [SerializeField] private float gravityValue = -9.81f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = InputManager.instance;
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        groundedPlayer = _controller.isGrounded;
        isSprinting();

        if (groundedPlayer)
        {
            // Slight downward velocity to keep grounded stable
            if (playerVelocity.y < -2f)
                playerVelocity.y = -2f;
        }

        // Read input
        //Vector2 input = _input.Move;
        //Vector3 move = new Vector3(input.x, 0, input.y);
        //move = Vector3.ClampMagnitude(move, 1f);

        //if (move != Vector3.zero)
            //transform.forward = move;

        // Jump using WasPressedThisFrame()
        if (groundedPlayer && _input.JumpAction.WasPressedThisFrame())
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        
        HandleMovement(Time.deltaTime);

        // Move
        Vector3 finalMove = Vector3.up * playerVelocity.y;
        _controller.Move(finalMove * Time.deltaTime);
    }

    private void HandleMovement(float delta)
    {
        Vector3 moveDir = (_input.Move.x * transform.right) + (_input.Move.y * transform.forward);
        _controller.Move(moveDir * (delta * moveSpeed));
    }

    private void isSprinting()
    {
        if (_input.SprintDown)
        {
           Debug.Log("SprintNotAdded");
           //moveSpeed = sprintSpeed;
        }
        else
        {
            moveSpeed = runSpeed;
        }
    }
}
