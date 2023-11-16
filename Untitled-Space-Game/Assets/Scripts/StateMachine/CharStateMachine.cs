using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.Animations;

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

    [SerializeField] LineRenderer _grappleLr;
    public LineRenderer GrappleLr
    {
        get { return _grappleLr; }
        set { _grappleLr = value; }
    }

    [SerializeField] Transform _aimTransform;

    [SerializeField] Transform _playerCam;
    public Transform PlayerCam
    {
        get { return _playerCam; }
    }

    [SerializeField] Material _speedLines;


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

    [Header("Sliding")]
    #region Sliding

    [SerializeField] float _lowestSlideSpeed;
    public float LowestSlideSpeed
    {
        get { return _lowestSlideSpeed; }
    }

    [SerializeField] bool _upCheck;
    public bool UpCheck
    {
        get { return _upCheck; }
        set { _upCheck = value; }
    }
    [SerializeField] LayerMask _slideLayer;

    [SerializeField] float _upcheckLenght;

    [SerializeField] RaycastHit _slideUpCheckHit;

    #endregion

    [Header("WallRunning")]
    #region WallRunning

    [SerializeField] bool _isWalled;
    public bool IsWalled
    {
        get { return _isWalled; }
        set { _isWalled = value; }
    }

    [SerializeField] bool _wallRight;
    public bool WallRight
    {
        get { return _wallRight; }
        set { _wallRight = value; }
    }
    [SerializeField] RaycastHit _rightWallHit;
    public RaycastHit RightWallHit
    {
        get { return _rightWallHit; }
    }

    [SerializeField] bool _wallLeft;
    public bool WallLeft
    {
        get { return _wallLeft; }
        set { _wallLeft = value; }
    }
    [SerializeField] RaycastHit _leftWallHit;
    public RaycastHit LeftWallHit
    {
        get { return _leftWallHit; }
    }


    [SerializeField] float _wallLeftRight;
    public float WallLeftRight
    {
        get { return _wallLeftRight; }
    }

    [SerializeField] LayerMask _wallLayer;

    [SerializeField] float _wallCheckDistance;
    public float WallCheckDistance
    {
        get { return _wallCheckDistance; }
    }

    [SerializeField] Vector3 _wallNormal;
    public Vector3 WallNormal
    {
        get { return _wallNormal; }
        set { _wallNormal = value; }
    }

    [SerializeField] Vector3 _wallForward;
    public Vector3 WallForward
    {
        get { return _wallForward; }
        set { _wallForward = value; }
    }

    [SerializeField] RaycastHit _wallAngleHit;
    public RaycastHit WallAngleHit
    {
        get { return _wallAngleHit; }
    }

    [SerializeField] float _maxWallAngle;
    public float MaxWallAngle
    {
        get { return _maxWallAngle; }
    }

    [SerializeField] float _wallRunDownForce;
    public float WallRunDownForce
    {
        get { return _wallRunDownForce; }
        set { _wallRunDownForce = value; }
    }
    // [SerializeField] bool _isWallAngle;
    // public bool IsWallAngle
    // {
    //     get { return _isWallAngle; }
    // }

    [SerializeField] bool _canStartWallTimer;
    public bool CanStartWallTimer
    {
        get { return _canStartWallTimer; }
        set { _canStartWallTimer = value; }
    }

    [SerializeField] float _maxWallClingTime;
    public float MaxWallClingTime
    {
        get { return _maxWallClingTime; }
    }

    [SerializeField] float _wallClingTime;
    public float WallClingTime
    {
        get { return _wallClingTime; }
        set { _wallClingTime = value; }
    }

    [SerializeField] Transform _currentWall;
    public Transform CurrentWall
    {
        get { return _currentWall; }
        set { _currentWall = value; }
    }

    [SerializeField] Transform _previousWall;
    public Transform PreviousWall
    {
        get { return _previousWall; }
        set { _previousWall = value; }
    }

    #endregion

    [Header("Grappling")]
    #region Grappling

    [SerializeField] bool _isGrappled;
    public bool IsGrappled
    {
        get { return _isGrappled; }
        set { _isGrappled = value; }
    }

    [SerializeField] float _grappleLenght;
    public float GrappleLenght
    {
        get { return _grappleLenght; }
    }

    [SerializeField] float _grappleDetectionRadius;

    [SerializeField] LayerMask _grappleLayer;

    [SerializeField] RaycastHit _grappleHit;
    public RaycastHit GrappleHit
    {
        get { return _grappleHit; }
    }

    [SerializeField] Vector3 _grappleDirection;
    public Vector3 GrappleDirection
    {
        get { return _grappleDirection; }
        set { _grappleDirection = value; }
    }

    [SerializeField] int _grappleHooks;
    public int GrappleHooks
    {
        get { return _grappleHooks; }
        set { _grappleHooks = value; }
    }

    [SerializeField] float _grappleDelay;
    public float GrappleDelay
    {
        get { return _grappleDelay; }
        set { _grappleDelay = value; }
    }

    [SerializeField] float _maxGrappleDelay;
    public float MaxGrappleDelay
    {
        get { return _maxGrappleDelay; }
    }

    [SerializeField] bool _finishedGrapple;
    public bool FinishedGrapple
    {
        get { return _finishedGrapple; }
        set { _finishedGrapple = value; }
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

    [Header("Vaulting")]
    #region Vaulting

    [SerializeField] RaycastHit _vaultHit;
    public RaycastHit VaultHit
    {
        get { return _vaultHit; }
    }

    [SerializeField] RaycastHit _vaultUnHit;
    public RaycastHit VaultUnHit
    {
        get { return _vaultUnHit; }
    }

    [SerializeField] float _vaultCheckDistance;
    public float VaultCheckDistance
    {
        get { return _vaultCheckDistance; }
    }

    [SerializeField] LayerMask _vaultLayer;
    public LayerMask VaultLayer
    {
        get { return _vaultLayer; }
    }


    [SerializeField] bool _vaultLow;
    public bool VaultLow
    {
        get { return _vaultLow; }
    }

    [SerializeField] bool _vaultMedium;
    public bool VaultMedium
    {
        get { return _vaultMedium; }
    }

    [SerializeField] bool _vaultHigh;
    public bool VaultHigh
    {
        get { return _vaultHigh; }
    }

    [SerializeField] float _lowVaultOffset;
    public float LowVaultOffset
    {
        get { return _lowVaultOffset; }
    }

    [SerializeField] float _mediumVaultOffset;
    public float MediumVaultOffset
    {
        get { return _mediumVaultOffset; }
    }

    [SerializeField] float _highVaultOffset;
    public float HighVaultOffset
    {
        get { return _highVaultOffset; }
    }

    [SerializeField] Transform _vaultObj;
    public Transform VaultObj
    {
        get { return _vaultObj; }
    }

    [SerializeField] bool _isVaulted;
    public bool IsVaulted
    {
        get { return _isVaulted; }
        set { _isVaulted = value; }
    }

    #endregion

    [Header("Inputs")]
    #region Inputs

    [SerializeField] bool _isMove;
    public bool IsMove
    {
        get { return _isMove; }
    }

    [SerializeField] bool _isSlide;
    public bool IsSlide
    {
        get { return _isSlide; }
    }

    [SerializeField] bool _isJump;
    public bool IsJump
    {
        get { return _isJump; }
    }

    [SerializeField] bool _isAim;
    public bool IsAim
    {
        get { return _isAim; }
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

    [SerializeField] float _slideSpeed;
    public float SlideSpeed
    {
        get { return _slideSpeed; }
    }

    [SerializeField] float _slideSpeedDecrease;
    public float SlideSpeedDecrease
    {
        get { return _slideSpeedDecrease; }
    }

    [SerializeField] float _slopeSlideSpeed;
    public float SlopeSlideSpeed
    {
        get { return _slopeSlideSpeed; }
    }

    [SerializeField] float _wallRunSpeed;
    public float WallRunSpeed
    {
        get { return _wallRunSpeed; }
    }

    [SerializeField] float _grappleSpeed;
    public float GrappleSpeed
    {
        get { return _grappleSpeed; }
    }

    [SerializeField] Vector3 _grapplePoint;
    public Vector3 GrapplePoint
    {
        get { return _grapplePoint; }
    }

    #endregion

    [Header("Speed Multipliers")]
    #region Speed Multipliers

    [SerializeField] float _strafeSpeedMultiplier;
    public float StrafeSpeedMultiplier
    {
        get { return _strafeSpeedMultiplier; }
    }

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

    [SerializeField] bool _isSliding;
    public bool IsSliding
    {
        get { return _isSliding; }
        set { _isSliding = value; }
    }

    [SerializeField] bool _isWallRunning;
    public bool IsWallRunning
    {
        get { return _isWallRunning; }
        set { _isWallRunning = value; }
    }

    [SerializeField] bool _isGrappling;
    public bool IsGrappling
    {
        get { return _isGrappling; }
        set { _isGrappling = value; }
    }


    [SerializeField] bool _isJumping;
    public bool IsJumping
    {
        get { return _isJumping; }
        set { _isJumping = value; }
    }

    #endregion

    #endregion

    [SerializeField] bool _isForced;
    public bool IsForced
    {
        get { return _isForced; }
        set { _isForced = value; }
    }

    [SerializeField] float _extraForce;
    public float ExtraForce
    {
        get { return _extraForce; }
        set { _extraForce = value; }
    }

    [SerializeField] float _forceSlowDownRate;
    public float ForceSlowDownRate
    {
        get { return _forceSlowDownRate; }
        set { _forceSlowDownRate = value; }
    }

    [SerializeField] bool _hasDied;
    public bool HasDied
    {
        get { return _hasDied; }
        set { _hasDied = value; }
    }

    [SerializeField] float _newFov;
    [SerializeField] float _oldFov;


    [SerializeField] float _fovThreshold;
    [SerializeField] bool _fovTimer;
    [SerializeField] float _maxFov;


    private void Awake()
    {
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 60;
        playerInput.actions.FindAction("Move").started += OnMovement;
        playerInput.actions.FindAction("Move").performed += OnMovement;
        playerInput.actions.FindAction("Move").canceled += OnMovement;

        playerInput.actions.FindAction("Slide").started += OnSlide;
        playerInput.actions.FindAction("Slide").performed += OnSlide;
        playerInput.actions.FindAction("Slide").canceled += OnSlide;

        playerInput.actions.FindAction("Jump").started += OnJump;
        playerInput.actions.FindAction("Jump").performed += OnJump;
        playerInput.actions.FindAction("Jump").canceled += OnJump;

        playerInput.actions.FindAction("Aim").started += OnAim;
        playerInput.actions.FindAction("Aim").performed += OnAim;
        playerInput.actions.FindAction("Aim").canceled += OnAim;

        playerInput.actions.FindAction("Shoot").started += OnShoot;
        playerInput.actions.FindAction("Shoot").performed += OnShoot;
        playerInput.actions.FindAction("Shoot").canceled += OnShoot;

        _states = new CharStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        _isGrounded = true;

        MoveForce = DesiredMoveForce;

        WallClingTime = MaxWallClingTime;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _playerCam = FindObjectOfType<Camera>().transform;
    }

    private void Update()
    {
        _movementSpeed = Rb.velocity.magnitude;
        if (_movementSpeed < 3 && _speedLines != null)
        {

            _speedLines.SetFloat("_Alpha", 0);
            // _cinemachineWalk.m_Lens.FieldOfView = Mathf.Lerp(_oldFov, 60, 1);

        }
        else if (_speedLines != null)
        {
            _speedLines.SetFloat("_Alpha", 0.5f);

            SmoothFov(MoveForce);
        }

        _upCheck = CheckUpSlide();


        if (Input.GetKey(KeyCode.P))
        {
            Debug.Log(_currentState.ToString());
        }

        VaultCheck();

        if (_isShoot)
        {
            CheckForGrapple();
        }

        CurrentMovement = (Orientation.forward * CurrentMovementInput.y).normalized + (Orientation.right * CurrentMovementInput.x).normalized;

        _currentState.UpdateStates();

        IsGrounded = CheckGrounded();
        PlayerAnimator.SetBool("OnGround", IsGrounded);

        IsSloped = CheckSloped();

        CheckForWall();
        CheckWallDirection();
        // _isWallAngle = CheckWallAngle();

        if (IsAim)
        {
            // _cinemachineAim.enabled = true;
            // _cinemachineWalk.LookAt = _aimTransform;
            // _cinemachineWalk.Follow = _aimTransform;
        }
        else
        {
            // _cinemachineAim.enabled = false;
            // _cinemachineWalk.LookAt = this.transform;
            // _cinemachineWalk.Follow = this.transform;


        }

        if (CanStartWallTimer)
        {
            WallClingTime -= Time.deltaTime;
        }


        if (IsGrounded || IsSloped)
        {
            Rb.drag = GroundDrag;
        }
        else if (!IsSloped && !IsGrounded)
        {
            Rb.drag = 0;
        }

        HandleStrafeSpeed();
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
    }

    #region MonoBehaviours

    private void FixedUpdate()
    {
        _currentState.FixedUpdateStates();
    }

    private void Lateupdate()
    {
        _currentState.LateUpdateStates();
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

    void OnSlide(InputAction.CallbackContext context)
    {
        _isSlide = context.ReadValueAsButton();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJump = context.ReadValueAsButton();
    }

    void OnAim(InputAction.CallbackContext context)
    {
        _isAim = context.ReadValueAsButton();
    }

    void OnShoot(InputAction.CallbackContext context)
    {
        _isShoot = context.ReadValueAsButton();
    }

    #endregion

    #region STUFF

    public void VaultCheck()
    {
        Vector3 offsetLow = this.transform.position + new Vector3(0, -_lowVaultOffset, 0);
        if (Physics.Raycast(offsetLow, Orientation.forward, out _vaultHit, _vaultCheckDistance, _vaultLayer))
        {
            print("low");
            _vaultObj = _vaultHit.transform;
            _vaultLow = true;

            Vector3 offsetMedium = this.transform.position + new Vector3(0, _mediumVaultOffset, 0);
            if (Physics.Raycast(offsetMedium, Orientation.forward, out _vaultHit, _vaultCheckDistance, _vaultLayer))
            {
                print("high");
                _vaultMedium = true;

                Vector3 offsetHigh = this.transform.position + new Vector3(0, _highVaultOffset, 0);

                // if (Physics.Raycast(offsetHigh, Orientation.forward, out _vaultHit, _vaultCheckDistance, _vaultLayer))
                // {

                // }
            }
            else
            {
                _vaultMedium = false;
            }
        }
        else
        {
            _vaultLow = false;
        }
    }

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

    private void CheckForWall()
    {
        _wallRight = (Physics.Raycast(transform.position, Orientation.right, out _rightWallHit, WallCheckDistance, _wallLayer));


        if (_wallRight)
        {
            _wallLeftRight = 1;
        }
        _wallLeft = (Physics.Raycast(transform.position, -Orientation.right, out _leftWallHit, WallCheckDistance, _wallLayer));


        if (_wallLeft)
        {
            _wallLeftRight = 0;
        }

        if (WallRight || WallLeft)
        {
            IsWalled = true;
        }
        else if (!WallRight && !WallLeft)
        {
            IsWalled = false;
        }
    }

    public void CheckWallDirection()
    {
        WallNormal = WallRight ? RightWallHit.normal : LeftWallHit.normal;

        WallForward = Vector3.Cross(WallNormal, transform.up);
    }

    public bool CheckWallAngle()
    {
        if (Physics.Raycast(transform.position, PlayerObj.forward, out _wallAngleHit, _playerHeight * 0.5f + 1f, _wallLayer))
        {
            Debug.Log("HIT WALL");
            float angle = Vector3.Angle(PlayerObj.forward, _wallAngleHit.normal);
            return angle < _maxWallAngle && angle != 0;
        }

        return false;
    }

    public void CheckForGrapple()
    {
        if (Physics.SphereCast(_playerCam.position, _grappleDetectionRadius, _playerCam.forward, out _grappleHit, GrappleLenght, _grappleLayer))
        {
            _isGrappled = true;
            _grapplePoint = GrappleHit.point;
        }
        else
        {
            _isGrappled = false;
        }
    }

    public Vector3 GetWallJumpDirection(Vector3 inputDir)
    {
        Vector3 newInputDir = new Vector3(inputDir.x *= 4, 0, inputDir.z *= 4);
        newInputDir *= _jumpForce;
        return newInputDir;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        Debug.DrawRay(this.transform.position, Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized);
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Orientation.forward, Color.black);

        if (WallRight)
        {
            Debug.DrawRay(transform.position, Orientation.right, Color.green);
        }
        else
        {
            Debug.DrawRay(transform.position, Orientation.right, Color.red);
        }

        if (WallLeft)
        {
            Debug.DrawRay(transform.position, -Orientation.right, Color.green);
        }
        else
        {
            Debug.DrawRay(transform.position, -Orientation.right, Color.red);
        }
    }


    #endregion

    public void SmoothFov(float movingspeed)
    {
        _newFov = 60 + movingspeed;

        if (_newFov > _oldFov + 3)
        {
            // _cinemachineWalk.m_Lens.FieldOfView = Mathf.Lerp(_oldFov, _newFov, 1);
            _oldFov = _newFov;
        }
        else if (_oldFov > _newFov - 3)
        {
            // _cinemachineWalk.m_Lens.FieldOfView = Mathf.Lerp(_oldFov, _newFov, 1);
            _oldFov = _newFov;
        }
    }

    // IEnumerator SmoothFovTime()
    // {
    //     // yield return new WaitForSeconds(0.5f);
    //     // _cinemachineWalk.m_Lens.FieldOfView = Mathf.Lerp(_oldFov, _oldFov + 1, 1);
    //     // _fovTimer = false;

    // }

    public bool CheckUpSlide()
    {
        if (Physics.Raycast(_colliders[1].transform.position, _colliders[1].transform.up, out _slideUpCheckHit, _upcheckLenght, _slideLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void HandleStrafeSpeed()
    {
        if (_currentMovementInput.y == 1)
        {
            _strafeSpeedMultiplier = 1.0f; // Full speed when moving forward
        }
        else
        {
            // Adjust strafe speed based on forward input
            _strafeSpeedMultiplier = Mathf.Lerp(0.5f, 1.0f, Mathf.Abs(_currentMovementInput.y));
        }
    }

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

            if (IsForced && flatVel.magnitude > ExtraForce)
            {
                Vector3 limitedVel = flatVel.normalized * ExtraForce;
                Rb.velocity = new Vector3(limitedVel.x, Rb.velocity.y, limitedVel.z);
            }
            else if (IsForced && flatVel.magnitude <= 1)
            {
                ExtraForce = 0;
            }

            if (IsForced)
            {
                ExtraForce -= _forceSlowDownRate * Time.deltaTime;

                if (ExtraForce <= MoveForce)
                {
                    IsForced = false;
                    ExtraForce = 0;
                }
            }

            if (!IsForced && flatVel.magnitude > MoveForce)
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