using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
// using TMPro;
// using Unity.VisualScripting;
// using UnityEngine.UI;
// using UnityEngine.Animations;

public class CharStateMachine : MonoBehaviour
// , IDataPersistence
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

    // [SerializeField] Animator _playerAnimator;
    // public Animator PlayerAnimator
    // {
    //     get { return _playerAnimator; }
    // }

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

    [Header("Inputs")]
    #region Inputs

    [SerializeField] bool _isMove;
    public bool IsMove
    {
        get { return _isMove; }
    }

    [SerializeField] Vector2 _isCam;
    public Vector2 IsCam
    {
        get { return _isCam; }
    }

    [SerializeField] bool _isRun;
    public bool IsRun
    {
        get { return _isRun; }
    }

    [SerializeField] bool _isJump;
    public bool IsJump
    {
        get { return _isJump; }
    }

    [SerializeField] bool _isCrouch;
    public bool IsCrouch
    {
        get { return _isCrouch; }
    }

    [SerializeField] bool _isShoot;
    public bool IsShoot
    {
        get { return _isShoot; }
    }

    [SerializeField] float _isHotbar;
    public float IsHotbar
    {
        get { return _isHotbar; }
        set { _isHotbar = value; }
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
        // set { _isGrounded = value; }
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
        // set { _isSloped = value; }
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

    [Header("Running")]
    #region Running

    [SerializeField] float _maxStamina;
    public float MaxStamina
    {
        get { return _maxStamina; }
        set { _maxStamina = value; }
    }

    [SerializeField] float _stamina;
    public float Stamina
    {
        get { return _stamina; }
        set { _stamina = value; }
    }

    [SerializeField] float _staminaDecreaseRate;
    public float StaminaDecreaseRate
    {
        get { return _staminaDecreaseRate; }
    }

    [SerializeField] float _staminaIncreaseRate;
    public float StaminaIncreaseRate
    {
        get { return _staminaIncreaseRate; }
    }

    [SerializeField] bool _decreaseStamina;
    public bool DecreaseStamina
    {
        get { return _decreaseStamina; }
        set { _decreaseStamina = value; }
    }

    #endregion

    [Header("Exhaust")]
    #region Exhaust

    [SerializeField] float _maxExhaustTime;
    public float MaxExhastTime
    {
        get { return _maxExhaustTime; }
    }

    [SerializeField] float _exhaustTime;
    public float ExhaustTime
    {
        get { return _exhaustTime; }
        set { _exhaustTime = value; }
    }

    #endregion

    [Header("Jumping")]
    #region Jumping

    [SerializeField] float _jumpForce;
    public float JumpForce
    {
        get { return _jumpForce; }
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

    [Header("Crouching")]
    #region Crouching

    [SerializeField] bool _crouchUp;
    public bool CrouchUp
    {
        get { return _crouchUp; }
    }

    private RaycastHit _crouchUpHit;
    private float _crouchUpLenght;

    #endregion

    [Header("Speeds")]
    #region Speeds

    [SerializeField] float _walkSpeed;
    public float WalkSpeed
    {
        get { return _walkSpeed; }
    }

    [SerializeField] float _runSpeed;
    public float RunSpeed
    {
        get { return _runSpeed; }
    }

    [SerializeField] float _exhaustSpeed;
    public float ExhaustSpeed
    {
        get { return _exhaustSpeed; }
    }

    [SerializeField] float _crouchSpeed;
    public float CrouchSpeed
    {
        get { return _crouchSpeed; }
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

    [Header("MultiTool")]
    #region MultiTool

    [SerializeField] float _camDistance;

    [SerializeField] float _gatherTime;

    [SerializeField] float _toolRange;
    [SerializeField] RaycastHit _toolHit;
    [SerializeField] LayerMask _gatherMask;

    [SerializeField] GameObject _InteractPanel;
    [SerializeField] TMP_Text _interactableTxt;


    [SerializeField] float _interactableRadius;
    [SerializeField] RaycastHit _interactableHit;
    [SerializeField] float _interactableRange;
    [SerializeField] LayerMask _interactableMask;


    #endregion

    #endregion

    // public void LoadData(GameData data)
    // {
    //     this.Rb.position = data.playerPosition;
    //     this.transform.rotation = data.playerRotation;
    // }

    // public void SaveData(ref GameData data)
    // {
    //     data.playerPosition = this.transform.position;
    //     data.playerRotation = this.transform.rotation;
    // }

    private void OnEnable()
    {
        // DontDestroyOnLoad(this);
        playerInput.actions.FindAction("Move").started += OnMovement;
        playerInput.actions.FindAction("Move").performed += OnMovement;
        playerInput.actions.FindAction("Move").canceled += OnMovement;

        playerInput.actions.FindAction("Jump").started += OnJump;
        playerInput.actions.FindAction("Jump").performed += OnJump;
        playerInput.actions.FindAction("Jump").canceled += OnJump;

        playerInput.actions.FindAction("Run").started += OnRun;
        playerInput.actions.FindAction("Run").performed += OnRun;
        playerInput.actions.FindAction("Run").canceled += OnRun;

        playerInput.actions.FindAction("Crouch").started += OnCrouch;
        playerInput.actions.FindAction("Crouch").performed += OnCrouch;
        playerInput.actions.FindAction("Crouch").canceled += OnCrouch;

        playerInput.actions.FindAction("Hotbar").started += OnHotbar;
        playerInput.actions.FindAction("Hotbar").performed += OnHotbar;
        playerInput.actions.FindAction("Hotbar").canceled += OnHotbar;

        playerInput.actions.FindAction("Shoot").started += OnShoot;
        playerInput.actions.FindAction("Shoot").performed += OnShoot;
        playerInput.actions.FindAction("Shoot").canceled += OnShoot;

        playerInput.actions.FindAction("Camera").performed += OnCam;
        playerInput.actions.FindAction("Camera").canceled += OnCam;

        _states = new CharStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        _isGrounded = true;
        _stamina = _maxStamina;

        MoveForce = DesiredMoveForce;

        _camDistance = Vector3.Distance(_playerCam.transform.position, _playerObj.transform.position);

        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _movementSpeed = Rb.velocity.magnitude;


        // if (_isHotbar < 0)
        // {
        //     Debug.Log("scroll up");
        // }
        // if (IsHotbar > 0)
        // {
        //     Debug.Log("scroll down");
        // }

        if (_isShoot)
        {
            CheckTool();
        }

        if (Input.GetKey(KeyCode.O))
        {
            Debug.Log(_currentState.ToString());
        }

        CurrentMovement = (Orientation.forward * CurrentMovementInput.y).normalized + (Orientation.right * CurrentMovementInput.x).normalized;

        // PlayerAnimator.SetBool("OnGround", IsGrounded);

        _isGrounded = CheckGrounded();
        _isSloped = CheckSloped();
        _crouchUp = CheckCrouchUp();

        if (_decreaseStamina)
        {
            _stamina -= _staminaDecreaseRate * Time.deltaTime;
        }
        else if (_stamina < _maxStamina)
        {
            _stamina += _staminaIncreaseRate * Time.deltaTime;
        }
        else if (_stamina > _maxStamina)
        {
            _stamina = _maxStamina;
        }

        if (IsGrounded || IsSloped)
        {
            Rb.drag = GroundDrag;
        }
        else if (!IsSloped && !IsGrounded)
        {
            Rb.drag = 0;
        }

        SpeedControl();

        if (Mathf.Abs(DesiredMoveForce - LastDesiredMoveForce) > 4f && MoveForce != 0)
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
        _currentState.FixedUpdateStates();
    }

    private void LateUpdate()
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

    void OnCrouch(InputAction.CallbackContext context)
    {
        _isCrouch = context.ReadValueAsButton();
    }

    void OnRun(InputAction.CallbackContext context)
    {
        _isRun = context.ReadValueAsButton();
    }

    void OnHotbar(InputAction.CallbackContext context)
    {
        _isHotbar = context.ReadValue<float>();
    }

    void OnShoot(InputAction.CallbackContext context)
    {
        _isShoot = context.ReadValueAsButton();
    }

    void OnCam(InputAction.CallbackContext context)
    {
        _isCam = context.ReadValue<Vector2>();
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

    private void CheckTool()
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }
        if (InventoryManager.Instance.GetSelectedItem() == null)
        {
            return;
        }
        if (InventoryManager.Instance.GetSelectedItem().name == "Pickaxe")
        {
            GatherTool();
        }
    }

    private void WeaponTool()
    {
        // TODO - make weapon variables
        // float rayDistance = _toolRange += Vector3.Distance(_playerCam.transform.position, _playerObj.transform.position);
        // if (Physics.Raycast(_playerCam.position, Vector3.forward, out _toolHit, rayDistance, _gatherMask))
        // {

        // }
    }

    private void GatherTool()
    {

        if (Physics.Raycast(_playerCam.position, Vector3.forward, out _toolHit, _toolRange + _camDistance, _gatherMask))
        {
            if (_gatherTime == -1)
            {
                _gatherTime = _toolHit.transform.GetComponent<ResourceVein>().Resource.mineDuration;
            }
            else
            {
                _gatherTime -= Time.deltaTime;

                if (_gatherTime <= 0)
                {
                    InventoryManager.Instance.AddItem(_toolHit.transform.GetComponent<ResourceVein>().Resource.item.itemID, _toolHit.transform.GetComponent<ResourceVein>().Resource.recourceAmount);
                    _gatherTime = _toolHit.transform.GetComponent<ResourceVein>().Resource.mineDuration;
                }
            }
        }
    }

    public bool CheckCrouchUp()
    {
        if (Physics.Raycast(_colliders[1].transform.position, _colliders[1].transform.up, out _crouchUpHit, _crouchUpLenght))
        {
            return true;
        }
        else
        {
            return false;
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

    private void CheckInteractable()
    {
        if (Physics.SphereCast(_playerCam.transform.position, _interactableRadius, Vector3.forward, out _interactableHit, _interactableRange + _camDistance, _interactableMask))
        {
            if (_interactableHit.transform.GetComponent<DroppedItem>())
            {
                _interactableTxt.text = "Press (E) to pick up " + _interactableHit.transform.GetComponent<DroppedItem>().amount + " " +
                _interactableHit.transform.GetComponent<DroppedItem>().item.name;
                _InteractPanel.SetActive(true);
            }
        }
        else
        {
            _InteractPanel.SetActive(false);
        }
    }

}