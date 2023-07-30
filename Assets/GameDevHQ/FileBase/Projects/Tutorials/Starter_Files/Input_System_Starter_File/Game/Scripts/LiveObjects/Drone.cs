using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Game.Scripts.UI;
using UnityEngine.InputSystem;

namespace Game.Scripts.LiveObjects
{
    public class Drone : MonoBehaviour
    {
        private enum Tilt
        {
            NoTilt, Forward, Back, Left, Right
        }

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private float _speed = 5f;
        private bool _inFlightMode = false;
        [SerializeField]
        private Animator _propAnim;
        [SerializeField]
        private CinemachineVirtualCamera _droneCam;
        [SerializeField]
        private InteractableZone _interactableZone;
        

        public static event Action OnEnterFlightMode;
        public static event Action onExitFlightmode;

        private PlayerInputActions _input;

        private void OnEnable()
        {
            InteractableZone.OnZoneInteractionComplete += EnterFlightMode;
        }

        private void Start()
        {
            _input = new PlayerInputActions();
        }

        private void EnterFlightMode(InteractableZone zone)
        {
            _input.Player.Disable();
            _input.Drone.Enable();
            if (_inFlightMode != true && zone.GetZoneID() == 4) // drone Scene
            {
                _propAnim.SetTrigger("StartProps");
                _droneCam.Priority = 11;
                _inFlightMode = true;
                OnEnterFlightMode?.Invoke();
                UIManager.Instance.DroneView(true);
                _interactableZone.CompleteTask(4);
            }
        }

        private void ExitFlightMode()
        {
            _input.Drone.Disable();
            _input.Player.Enable();
            _droneCam.Priority = 9;
            _inFlightMode = false;
            UIManager.Instance.DroneView(false);            
        }

        private void Update()
        {
            if (_inFlightMode)
            {
                CalculateTilt();
                CalculateMovementUpdate();

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _inFlightMode = false;
                    onExitFlightmode?.Invoke();
                    ExitFlightMode();
                }
            }
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(transform.up * (9.81f), ForceMode.Acceleration);
            if (_inFlightMode)
                CalculateMovementFixedUpdate();
        }

        private void CalculateMovementUpdate()
        {
            if (/*Input.GetKey(KeyCode.LeftArrow)*/Keyboard.current.aKey.isPressed)
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y -= _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }
            if (/*Input.GetKey(KeyCode.RightArrow)*/Keyboard.current.dKey.isPressed)
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y += _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }
        }

        private void CalculateMovementFixedUpdate()
        {
            
            if (/*Input.GetKey(KeyCode.Space)*/Keyboard.current.spaceKey.isPressed)
            {
                _rigidbody.AddForce(transform.up * _speed, ForceMode.Acceleration);
            }
            if (/*Input.GetKey(KeyCode.V)*/Keyboard.current.vKey.isPressed)
            {
                _rigidbody.AddForce(-transform.up * _speed, ForceMode.Acceleration);
            }
        }

        private void CalculateTilt()
        {
            if (/*Input.GetKey(KeyCode.A)*/Keyboard.current.aKey.isPressed) 
                transform.rotation = Quaternion.Euler(00, transform.localRotation.eulerAngles.y, 30);

            else if (/*Input.GetKey(KeyCode.D)*/Keyboard.current.dKey.isPressed)
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, -30);

            else if (/*Input.GetKey(KeyCode.W)*/Keyboard.current.wKey.isPressed)
                transform.rotation = Quaternion.Euler(30, transform.localRotation.eulerAngles.y, 0);

            else if (/*Input.GetKey(KeyCode.S)*/Keyboard.current.sKey.isPressed)
                transform.rotation = Quaternion.Euler(-30, transform.localRotation.eulerAngles.y, 0);

            else 
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
        }

        private void OnDisable()
        {
            InteractableZone.OnZoneInteractionComplete -= EnterFlightMode;
        }
    }
}
