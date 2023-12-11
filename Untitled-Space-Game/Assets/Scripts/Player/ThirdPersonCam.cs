using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
    [SerializeField]
    private Transform _orientation,
    _player,
    _camHolder,
    _playerObj,
    _camTarget;

    [SerializeField]
    private float _rotationSpeed;

    [SerializeField]
    private CharStateMachine _stateMachine;

    [Header("Mouse Settings")]
    [SerializeField] float _mouseSensitivity;
    [SerializeField] float _minXRotation;
    [SerializeField] float _maxXRotation;

    Vector3 inputDir;

    float _xRotation;
    float _yRotation;


    private void Start()
    {
        // DontDestroyOnLoad(_camHolder);
        _stateMachine = FindObjectOfType<CharStateMachine>();

        _orientation = _stateMachine.Orientation;
        _player = _stateMachine.transform;
        _playerObj = _stateMachine.PlayerObj;
    }

    void Update()
    {
        // Vector3 viewDir = _player.position - new Vector3(transform.position.x, _player.position.y, transform.position.z);
        _orientation.forward = _playerObj.forward.normalized;

        float mouseY = _stateMachine.IsCam.y * _mouseSensitivity;
        float mouseX = _stateMachine.IsCam.x * _mouseSensitivity;

        _yRotation += mouseX;
        _xRotation -= mouseY;

        _xRotation = Mathf.Clamp(_xRotation, -_minXRotation, _maxXRotation);

        _camTarget.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);

        _playerObj.transform.rotation = Quaternion.Euler(0, _yRotation, 0);
    }
}
