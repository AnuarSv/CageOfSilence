﻿using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
        public float MoveSpeedRaw = 4.0f;
        public float SprintSpeedRaw = 6.0f;
        public float RotationSpeed = 1.0f;
        public float SpeedChangeRate = 10.0f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public float Gravity = -15.0f;
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;

        [Header("Crouching")]
        public float crouchSpeed = 1f;
        public float crouchTransitionSpeed = 6f;
        private bool _isCrouching = false;

        private float _cinemachineTargetPitch;
        private float MoveSpeed;
        private float SprintSpeed;
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float startHeight;
        private Vector3 startCenter;
        private Vector3 startCamPos;
        private float _crouchHeight;
        private Vector3 _crouchCenter;
        private Vector3 _crouchCamPos;


#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private bool CanStandUp()
        {
            Vector3 standCenter = transform.position + Vector3.up * (startHeight / 2f);
            float radius = _controller.radius * 1.003f;
            float height = startHeight;

            Vector3 bottom = standCenter + Vector3.down * (height / 2f - radius);
            Vector3 top = standCenter + Vector3.up * (height / 2f - radius);

            Collider[] hits = Physics.OverlapCapsule(bottom, top, radius, GroundLayers, QueryTriggerInteraction.Ignore);

            foreach (Collider hit in hits)
            {
                if (!hit.CompareTag("Reach"))
                {
                    return false;
                }
            }
            return true;
        }



        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies.");
#endif
            MoveSpeed = MoveSpeedRaw;
            SprintSpeed = SprintSpeedRaw;
            startHeight = _controller.height;
            startCenter = _controller.center;
            startCamPos = CinemachineCameraTarget.transform.localPosition;
            _crouchHeight = startHeight / 2f;
            _crouchCenter = new Vector3(startCenter.x, startCenter.y / 2f, startCenter.z);
            _crouchCamPos = new Vector3(startCamPos.x, startCamPos.y / 2f, startCamPos.z);
        }

        private void Update()
        {
            GroundedCheck();
            Move();
            ApplyGravity();
            Crouch();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void ApplyGravity()
        {
            if (Grounded && _verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
            else
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            if (_verticalVelocity < -40f)
            {
                _verticalVelocity = -40f;
            }
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            if (_input.move != Vector2.zero)
            {
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void Crouch()
        {
            if (_input.crouch)
            {
                _isCrouching = true;
            }
            else
            {
                if (CanStandUp())
                {
                    _isCrouching = false;
                }
            }

            float targetHeight = _isCrouching ? _crouchHeight : startHeight;
            Vector3 targetCenter = _isCrouching ? _crouchCenter : startCenter;
            Vector3 targetCamPos = _isCrouching ? _crouchCamPos : startCamPos;

            float previousHeight = _controller.height;
            _controller.height = Mathf.Lerp(_controller.height, targetHeight, Time.deltaTime * 10f);

            float heightDiff = _controller.height - previousHeight;
            transform.position += new Vector3(0, heightDiff / 2f, 0);

            _controller.center = Vector3.Lerp(_controller.center, targetCenter, Time.deltaTime * 10f);
            CinemachineCameraTarget.transform.localPosition = Vector3.Lerp(CinemachineCameraTarget.transform.localPosition, targetCamPos, Time.deltaTime * 10f);

            MoveSpeed = Mathf.Lerp(MoveSpeed, _isCrouching ? crouchSpeed : MoveSpeedRaw, Time.deltaTime * 10f);
            SprintSpeed = Mathf.Lerp(SprintSpeed, _isCrouching ? crouchSpeed : SprintSpeedRaw, Time.deltaTime * 10f);
        }



        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = Grounded ? transparentGreen : transparentRed;
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }
    }
}
