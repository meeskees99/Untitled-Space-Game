using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
// using TMPro;
// using Unity.VisualScripting;
// using UnityEngine.UI;
// using UnityEngine.Animations;

public class CharStateMachine : MonoBehaviour
{
    #region Variables

    [Header("Refrences")]
    #region Refrences

    [SerializeField] private PlayerInput playerInput = null;
    public PlayerInput PlayerInput => playerInput;

    [SerializeField] Transform _playerObj;
    public Transform PlayerObj
    {
        get { return _playerObj; }
    }

    [SerializeField] Animator _playerAnimator;
    public Animator PlayerAnimator
    {
        get { return _playerAnimator; }
    }

    [SerializeField] Transform _playerCam;
    public Transform PlayerCam
    {
        get { return _playerCam; }
    }

    [SerializeField] private Transform _orientation;
    public Transform Orientation
    {
        get { return _orientation; }
    }

    [SerializeField] Rigidbody _rb;
    public Rigidbody Rb
    {
        get { return _rb; }
    }

    CharStateFactory _states;
    CharBaseState _currentState;
    public CharBaseState CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }

    [SerializeField] Collider[] _colliders;
    public Collider[] Colliders
    {
        get { return _colliders; }
    }

    #endregion

    [Header("Movement")]
    #region Movement

    [SerializeField] Vector2 _currentMovementInput;
    public Vector2 CurrentMovementInput
    {
        get { return _currentMovementInput; }
    }

    [SerializeField] Vector3 _currentMovement;
    public Vector3 CurrentMovement
    {
        get { return _currentMovement; }
        set { _currentMovement = value; }
    }

    [SerializeField] Vector3 _movement;
    public Vector3 Movement
    {
        get { return _movement; }
        set { _movement = value; }
    }

    [SerializeField] float _movementSpeed;
    public float MovementSpeed
    {
        get { return _movementSpeed; }
    }

    [SerializeField] float _moveForce;
    public float MoveForce
    {
        get { return _moveForce; }
        set { _moveForce = value; }
    }

    [SerializeField] float _desiredMoveForce;
    public float DesiredMoveForce
    {
        get { return _desiredMoveForce; }
        set { _desiredMoveForce = value; }
    }

    [SerializeField] float _lastDesiredMoveForce;
    public float LastDesiredMoveForce
    {
        get { return _lastDesiredMoveForce; }
        set { _lastDesiredMoveForce = value; }
    }

    #endregion

    [Header("Groundcheck")]
    #region GroundCheck

    [SerializeField] bool _isGrounded;
    public bool IsGrounded
    {
        get { return _isGrounded; }
        set { _isGrounded = value; }
    }

    [SerializeField] LayerMask _groundLayer;

    [SerializeField] float _groundDrag;
    public float GroundDrag
    {
        get { return _groundDrag; }
    }

    [SerializeField] float sphereRadius;
    [SerializeField] float sphereOffset;

    #endregion

    [Header("Jumping")]
    #region Jumping

    [SerializeField] float _jumpForce;
    public float JumpForce
    {
        get { return _jumpForce; }
    }

    [SerializeField] Vector3 _jumpMent;
    public Vector3 JumpMent
    {
        get { return _jumpMent; }
        set { _jumpMent = value; }
    }

    [SerializeField] float _maxJumpTime;
    public float MaxJumpTime
    {
        get { return _maxJumpTime; }
    }

    [SerializeField] float _isJumpTime;
    public float IsJumpTime
    {
        get { return _isJumpTime; }
        set { _isJumpTime = value; }
    }

    #endregion

    [Header("SlopeCheck")]
    #region SlopeCheck

    [SerializeField] bool _isSloped;
    public bool IsSloped
    {
        get { return _isSloped; }
        set { _isSloped = value; }
    }
    [SerializeField] bool _isExitingSlope;
    public bool IsExitingSlope
    {
        get { return _isExitingSlope; }
        set { _isExitingSlope = value; }
    }

    public RaycastHit _slopeHit;

    [SerializeField] float _maxSlopeAngle;

    [SerializeField] float _playerHeight;

    #endregion

    [Header("Inputs")]
    #region Inputs

    [SerializeField] bool _isMove;
    public bool IsMove
    {
        get { return _isMove; }
    }

    [SerializeField] bool _isJump;
    public bool IsJump
    {
        get { return _isJump; }
    }

    [SerializeField] bool _isShoot;
    public bool IsShoot
    {
        get { return _isShoot; }
    }

    #endregion

    [Header("Speeds")]
    #region Speeds

    [SerializeField] float _moveSpeed;
    public float MoveSpeed
    {
        get { return _moveSpeed; }
    }

    [SerializeField] float _airSpeed;
    public float AirSpeed
    {
        get { return _airSpeed; }
    }

    #endregion

    [Header("Speed Multipliers")]
    #region Speed Multipliers

    [SerializeField] float _speedIncreaseMultiplier;
    public float SpeedIncreaseMultiplier
    {
        get { return _speedIncreaseMultiplier; }
    }

    [SerializeField] float _slopeSpeedIncreaseMultiplier;
    public float SlopeSpeedIncreaseMultiplier
    {
        get { return _slopeSpeedIncreaseMultiplier; }
    }

    [SerializeField] float _grappleSpeedIncreaseMultiplier;
    public float GrappleSpeedIncreaseMultiplier
    {
        get { return _grappleSpeedIncreaseMultiplier; }
    }

    [SerializeField] float _moveMultiplier;
    public float MoveMultiplier
    {
        get { return _moveMultiplier; }
        set { _moveMultiplier = value; }
    }

    #endregion

    [Header("States")]
    #region States

    [SerializeField] bool _isAired;
    public bool IsAired
    {
        get { return _isAired; }
        set { _isAired = value; }
    }

    [SerializeField] bool _isJumping;
    public bool IsJumping
    {
        get { return _isJumping; }
        set { _isJumping = value; }
    }

    #endregion

    #endregion

    private void OnEnable()
    {
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 60;
        playerInput.actions.FindAction("Move").started += OnMovement;
        playerInput.actions.FindAction("Move").performed += OnMovement;
        playerInput.actions.FindAction("Move").canceled += OnMovement;

        playerInput.actions.FindAction("Jump").started += OnJump;
        playerInput.actions.FindAction("Jump").performed += OnJump;
        playerInput.actions.FindAction("Jump").canceled += OnJump;

        playerInput.actions.FindAction("Shoot").started += OnShoot;
        playerInput.actions.FindAction("Shoot").performed += OnShoot;
        playerInput.actions.FindAction("Shoot").canceled += OnShoot;

        _states = new CharStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        _isGrounded = true;

        MoveForce = DesiredMoveForce;

        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _movementSpeed = Rb.velocity.magnitude;


        if (Input.GetKey(KeyCode.P))
        {
            Debug.Log(_currentState.ToString());
        }

        CurrentMovement = (Orientation.forward * CurrentMovementInput.y).normalized + (Orientation.right * CurrentMovementInput.x).normalized;

        // PlayerAnimator.SetBool("OnGround", IsGrounded);

        IsGrounded = CheckGrounded();
        IsSloped = CheckSloped();
        // _upCheck = CheckUpCrouch();

        if (IsGrounded || IsSloped)
        {
            Rb.drag = GroundDrag;
        }
        else if (!IsSloped && !IsGrounded)
        {
            Rb.drag = 0;
        }

        SpeedControl();

        if (Mathf.Abs(DesiredMoveForce - LastDesiredMoveForce) > 0f && MoveForce != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoovMoov());
        }
        else
        {
            MoveForce = DesiredMoveForce;
        }
        LastDesiredMoveForce = DesiredMoveForce;

        _currentState.UpdateStates();
    }

    #region MonoBehaviours

    private void FixedUpdate()
    {
        // _currentState.FixedUpdateStates();
    }

    private void Lateupdate()
    {
        // _currentState.LateUpdateStates();
    }

    private void OnTriggerEnter(Collider other)
    {
        // _currentState.OnTriggerEnterStates(other);
    }

    private void OnTriggerExit(Collider other)
    {
        // _currentState.OnTriggerExitStates(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        // _currentState.OnCollisionEnterStates(other);
    }

    private void OnCollisionExit(Collision other)
    {
        // _currentState.OnCollisionExitStates(other);
    }

    #endregion

    #region InputVoids

    void OnMovement(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMove = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJump = context.ReadValueAsButton();
    }

    void OnShoot(InputAction.CallbackContext context)
    {
        _isShoot = context.ReadValueAsButton();
    }

    #endregion

    #region STUFF

    private bool CheckGrounded()
    {
        Vector3 characterPosition = transform.position;

        Vector3 sphereCenter = characterPosition + Vector3.down * sphereOffset;
        bool isOnGround = Physics.SphereCast(sphereCenter, sphereRadius, Vector3.down, out RaycastHit hit, sphereOffset + 0.1f, _groundLayer);

        if (isOnGround)
        {
            Vector3 rayStart = characterPosition;
            Vector3 rayDirection = hit.point - rayStart;
            float rayDistance = Vector3.Distance(hit.point, rayStart) + 0.001f;

            if (Physics.Raycast(rayStart, rayDirection, out RaycastHit rayHit, rayDistance, _groundLayer))
            {
                if (CheckSloped())
                {
                    return true;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    bool CheckSloped()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _playerHeight * 0.5f + 0.8f, _groundLayer))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < _maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        Debug.DrawRay(this.transform.position, Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized);
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }

    #endregion    

    // public bool CheckUpCrouch()
    // {
    //     if (Physics.Raycast(_colliders[1].transform.position, _colliders[1].transform.up, out _slideUpCheckHit, _upcheckLenght, _slideLayer))
    //     {
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

    private void SpeedControl()
    {
        if (IsSloped && !IsExitingSlope)
        {
            if (Rb.velocity.magnitude > MoveForce)
                Rb.velocity = Rb.velocity.normalized * MoveForce;
        }
        else
        {
            Vector3 flatVel = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);


            if (flatVel.magnitude > MoveForce)
            {
                Vector3 limitedVel = flatVel.normalized * MoveForce;
                Rb.velocity = new Vector3(limitedVel.x, Rb.velocity.y, limitedVel.z);
            }
        }
    }

    IEnumerator SmoovMoov()
    {
        float time = 0;
        float difference = Mathf.Abs(DesiredMoveForce - MoveForce);
        float startValue = MoveForce;

        while (time < difference)
        {
            MoveForce = Mathf.Lerp(startValue, DesiredMoveForce, time / difference);

            if (CheckSloped())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * SpeedIncreaseMultiplier * SlopeSpeedIncreaseMultiplier * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * SpeedIncreaseMultiplier;
            }

            yield return null;
        }

        MoveForce = DesiredMoveForce;
    }

}