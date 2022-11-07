using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerRunner
{
    public class CharacterInputController : MonoBehaviour
    {
        [SerializeField] 
        private CharacterMovement m_CharacterMovement;

        private ThirdPersonCamera thirdPersonCamera;

        private bool isShowCursor;

        public void Init(ThirdPersonCamera camera)
        {
            thirdPersonCamera = camera;
            isShowCursor = false;
            VisualCursor();
        }

        private void Update()
        {
            if (m_CharacterMovement.Owner == null) return;

            if(m_CharacterMovement.Owner.hasAuthority 
                && m_CharacterMovement.Owner.isLocalPlayer 
                && thirdPersonCamera != null)
            {
                UpdateInputControl();
            }
        }

        private void UpdateInputControl()
        {
            m_CharacterMovement.DirectionControl = new Vector3(Input.GetAxis("Horizontal"),
                                                                0,
                                                                Input.GetAxis("Vertical"));
            thirdPersonCamera.RotateControl = new Vector2(Input.GetAxis("Mouse X"),
                                                            Input.GetAxis("Mouse Y"));

            if (Input.GetMouseButtonDown(0))
            {
                m_CharacterMovement.Sprint();
            }

            if(Input.GetKeyDown(KeyCode.V))
            {
                isShowCursor ^= true;
                VisualCursor();
            }
        }

        private void VisualCursor()
        {
            Cursor.lockState = isShowCursor ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isShowCursor;
        }
    }
}
