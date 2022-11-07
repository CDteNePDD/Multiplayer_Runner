using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerRunner
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        public static ThirdPersonCamera Instance;

        [SerializeField]
        private float m_Sensetive;

        [SerializeField]
        private float m_MinLimitY;

        [SerializeField]
        private float m_MaxLimitY;

        [SerializeField]
        private float m_Distance;

        [SerializeField]
        private float m_MinDistance;

        [SerializeField]
        private Vector3 m_Offset;

        [SerializeField]
        private float m_DistanceOffsetFromCollisionHit;

        [SerializeField]
        private float m_DistanceLerpRate;

        [HideInInspector]
        public Vector2 RotateControl;

        private Transform m_Target;

        private float deltaRotationX;
        private float deltaRotationY;

        private float currentDistance;

        public void SetTarget(Transform target)
        {
            m_Target = target;
            transform.LookAt(m_Target);

        }

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;
        }

        private void Update()
        {
            if (m_Target == null) return;

            deltaRotationX += RotateControl.x * m_Sensetive;
            deltaRotationY += RotateControl.y * -m_Sensetive;

            deltaRotationY = ClampAngle(deltaRotationY, m_MinLimitY, m_MaxLimitY);

            Quaternion finalRotation = Quaternion.Euler(deltaRotationY, deltaRotationX, 0);
            Vector3 finalPosition = m_Target.position - (finalRotation * Vector3.forward * m_Distance);
            finalPosition = AddLocalOffset(finalPosition);

            float targetDistance = m_Distance;

            RaycastHit hit;

            Debug.DrawLine(m_Target.position + new Vector3(0, m_Offset.y, 0), finalPosition, Color.red);

            if (Physics.Linecast(m_Target.position + new Vector3(0, m_Offset.y, 0), finalPosition, out hit) == true)
            {
                float distanceToHit = Vector3.Distance(m_Target.position + new Vector3(0, m_Offset.y, 0), hit.point);

                if (hit.transform != m_Target)
                {
                    if (distanceToHit < m_Distance)
                        targetDistance = distanceToHit - m_DistanceOffsetFromCollisionHit;
                }
            }

            currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, Time.deltaTime * m_DistanceLerpRate);
            currentDistance = Mathf.Clamp(currentDistance, m_MinDistance, m_Distance);

            // Correct camera position
            finalPosition = m_Target.position - (finalRotation * Vector3.forward * currentDistance);

            // Apply transform
            transform.rotation = finalRotation;
            transform.position = finalPosition;
            transform.position = AddLocalOffset(transform.position);
        }

        private Vector3 AddLocalOffset(Vector3 position)
        {
            Vector3 result = position;
            result += new Vector3(0, m_Offset.y, 0);
            result += transform.right * m_Offset.x;
            result += transform.forward * m_Offset.z;

            return result;
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
            {
                angle += 360;
            }
            if (angle > 360)
            {
                angle -= 360;
            }
            return Mathf.Clamp(angle, min, max);
        }
    }
}
