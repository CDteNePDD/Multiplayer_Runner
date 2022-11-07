using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerRunner
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {
        private const float SENSIVE_START_MOVEMENT = 0.2f;

        [SerializeField]
        private float m_AccelerationRate;

        [SerializeField]
        private float m_DistanceSprint;

        [Header("Speed")]
        [SerializeField]
        private float m_SpeedRun;
        [SerializeField]
        private float m_SpeedSprint;

        [HideInInspector]
        public Vector3 DirectionControl;

        public bool IsRun => DirectionControl.magnitude >= SENSIVE_START_MOVEMENT;
        public bool IsSprint => isSprint;
        public Player Owner => owner;

        private bool isSprint;

        private ThirdPersonCamera thirdPersonCamera;
        private Player owner;

        private CharacterController characterController;
        private Vector3 directionMovement;
        private float distanceSprint;

        #region Public
        public void Init(Player player, ThirdPersonCamera camera)
        {
            owner = player;
            thirdPersonCamera = camera;
        }

        public void Sprint()
        {
            isSprint = true;
            distanceSprint = 0;
        }

        public void UnSprint()
        {
            isSprint = false;
            distanceSprint = m_DistanceSprint;
        }
        #endregion

        #region Unity_API
        private void Start()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            if (thirdPersonCamera == null) return;

            if(owner.hasAuthority || owner.netIdentity.connectionToClient == null)
            {
                DistanceUpdate();
                Move();
            }
        }
        #endregion

        #region Private
        private void DistanceUpdate()
        {
            if (distanceSprint >= m_DistanceSprint) return;
            distanceSprint += characterController.velocity.magnitude * Time.fixedDeltaTime;
            if (distanceSprint >= m_DistanceSprint)
            {
                isSprint = false;
            }
        }

        private void Move()
        {
            if(isSprint)
            {
                SprintUpdate();
            }
            else if(IsRun)
            {
                RunUpdate();
            }
        }

        private void RunUpdate()
        {
            DirectionControl = DirectionCorrectByCamera();

            Debug.DrawRay(Vector3.zero, DirectionControl * 3, Color.red);

            directionMovement = Vector3.MoveTowards(directionMovement, DirectionControl,
                                                    m_AccelerationRate * Time.fixedDeltaTime);

            Debug.DrawRay(Vector3.zero, directionMovement.normalized * 3, Color.blue);

            transform.forward = directionMovement.normalized;

            characterController.Move(directionMovement * m_SpeedRun * Time.fixedDeltaTime);
        }

        private void SprintUpdate()
        {
            characterController.Move(transform.forward * m_SpeedSprint * Time.fixedDeltaTime);
        }

        private Vector3 DirectionCorrectByCamera()
        {
            Vector3 cameraPosition = thirdPersonCamera.transform.position;
            cameraPosition.y = transform.position.y;
            Vector3 cameralook = (transform.position - cameraPosition).normalized;
            float anlgeDirection = Vector3.SignedAngle(cameralook, Vector3.forward, Vector3.up);

            return Quaternion.AngleAxis(-anlgeDirection, Vector3.up) * DirectionControl;
        }
        #endregion
    }
}
