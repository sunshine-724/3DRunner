using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
public class Player : MonoBehaviour
{
    private GameInputs gameInput;
    private Rigidbody rb;
    private Transform tr;
    private Animator anim;

    [SerializeField] private Transform trCamera;
    
    [SerializeField] private float speed;
    private Vector3 moveInput;
        
    private bool isJump = false;
    private bool isSliding = false;

    void Awake()
    {
        gameInput = new GameInputs();
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        gameInput.PlayerKeyBoard.Move.performed += OnMove;
        gameInput.PlayerKeyBoard.Move.canceled += OnMove;

        gameInput.PlayerKeyBoard.Jump.performed += OnJump;
        gameInput.PlayerKeyBoard.Sliding.performed += OnSliding;

        gameInput.PlayerGamePad.Move.performed += OnMove;
        gameInput.PlayerGamePad.Move.canceled += OnMove;
        
        gameInput.PlayerGamePad.Jump.performed += OnJump;
        gameInput.PlayerGamePad.Sliding.performed += OnSliding;
        
        gameInput.Enable();
    }

    void OnDisable()
    {
        gameInput.PlayerKeyBoard.Move.performed -= OnMove;
        gameInput.PlayerKeyBoard.Move.canceled -= OnMove;
        
        gameInput.PlayerKeyBoard.Jump.performed -= OnJump;
        gameInput.PlayerKeyBoard.Sliding.performed -= OnSliding;
        
        gameInput.PlayerGamePad.Move.performed -= OnMove;
        gameInput.PlayerGamePad.Move.canceled -= OnMove;
        
        gameInput.PlayerGamePad.Jump.performed -= OnJump;
        gameInput.PlayerGamePad.Sliding.performed -= OnSliding;
        
        gameInput.Disable();
    }
    
    void FixedUpdate()
    {
        if (moveInput != Vector3.zero)
        {
            Vector3 cameraForward = trCamera.forward;
            Vector3 cameraRight = trCamera.right;

            //次元削減
            cameraForward.y = 0;
            cameraRight.y = 0;
            
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * moveInput.z + cameraRight * moveInput.x;
            moveDirection.Normalize();
            
            // キャラクターを移動方向に回転させる
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, toRotation, 0.1f)); // なめらかに回転
            
            rb.MovePosition(rb.position + moveDirection * speed); // スピードは適宜調整
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector3>(); // Vector3の入力を取得
        anim.SetFloat("speedX",moveInput.x);
        anim.SetFloat("speedZ",moveInput.z);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        float readValue = context.ReadValue<float>();

        bool isInputJump = (readValue > 0.5f);

        //デバッグ用
        bool testflag = (readValue > 0.5f);
        isJump = testflag;
        
        if (isJump)
        {
            Debug.Log("ジャンプします");
            this.rb.AddForce(Vector3.up);
            anim.SetTrigger("JumpTrigger");
        }
    }
    
    private void OnSliding(InputAction.CallbackContext context)
    {
        float readValue = context.ReadValue<float>();
        
        bool isInputSliding = (readValue > 0.5f);

        //デバッグ用
        bool testflag = (readValue > 0.5f);
        isSliding = testflag;
        
        if (isSliding)
        {
            Debug.Log("スライディングをします");
            this.rb.AddForce(Vector3.up);
            anim.SetTrigger("SlidingTrigger");
        }
    }
}