using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.LiveObjects
{
    public class Crate : MonoBehaviour
    {
        [SerializeField] private float _punchDelay;
        [SerializeField] private GameObject _wholeCrate, _brokenCrate;
        [SerializeField] private Rigidbody[] _pieces;
        [SerializeField] private BoxCollider _crateCollider;
        [SerializeField] private InteractableZone _interactableZone;
        private bool _isReadyToBreak = false;

        private List<Rigidbody> _brakeOff = new List<Rigidbody>();

        private PlayerInputActions _input;
        [SerializeField] private bool _holdPerformed = false;
        private bool _inZone = false;

        private void OnEnable()
        {
            InteractableZone.OnZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;
        }
       
        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {
            if (_isReadyToBreak == false && _brakeOff.Count > 0 && zone.GetZoneID() == 6)
            {
                _inZone = true;
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
            }

            
            if (_isReadyToBreak && zone.GetZoneID() == 6) //Crate zone            
            {
                /*
                if (_brakeOff.Count > 0 && _holdPerformed == false)
                {
                    return;
                }*/

                if(_brakeOff.Count == 0)
                {
                    _isReadyToBreak = false;
                    _crateCollider.enabled = false;
                    _interactableZone.CompleteTask(6);
                    Debug.Log("Completely Busted");
                }
            }
        }

        private void Start()
        {
            _brakeOff.AddRange(_pieces);
            _input = new PlayerInputActions();
            _input.Player.Enable();
            _input.Player.HoldInteract.performed += HoldInteract_performed;
            _input.Player.HoldInteract.started += HoldInteract_started;
            _input.Player.HoldInteract.canceled += HoldInteract_canceled;
        }

        private void HoldInteract_canceled(InputAction.CallbackContext context)
        {
            Debug.Log("Canceled");
            if (_brakeOff == null) { return; }
            if (_holdPerformed && _inZone)
            {
                BreakPart();
                BreakPart();
                BreakPart();
                StartCoroutine(PunchDelay());
            }
            _holdPerformed = false;
        }

        private void HoldInteract_started(InputAction.CallbackContext context)
        {
            Debug.Log("Started");
            if (_inZone)
            {
                BreakPart();
                StartCoroutine(PunchDelay());
            }
            _holdPerformed = false;
        }

        private void HoldInteract_performed(InputAction.CallbackContext context)
        {
            Debug.Log("Performed");
            _holdPerformed = true;
        }

        public void BreakPart()
        {
            Debug.Log("Breaking part");
            int rng = Random.Range(0, _brakeOff.Count);
            _brakeOff[rng].constraints = RigidbodyConstraints.None;
            _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
            _brakeOff.Remove(_brakeOff[rng]);
        }

        IEnumerator PunchDelay()
        {
            float delayTimer = 0;
            while (delayTimer < _punchDelay)
            {
                yield return new WaitForEndOfFrame();
                delayTimer += Time.deltaTime;
            }

            _interactableZone.ResetAction(6);
        }

        private void OnDisable()
        {
            InteractableZone.OnZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
        }
    }
}
