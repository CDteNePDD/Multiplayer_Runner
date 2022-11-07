using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerRunner
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimationState : MonoBehaviour
    {
        [SerializeField] 
        private CharacterMovement m_CharacterMovement;

        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (m_CharacterMovement.Owner == null) return;

            if (m_CharacterMovement.Owner.hasAuthority || m_CharacterMovement.Owner.netIdentity.connectionToClient == null)
            {
                UpdateParameter();
            }
        }

        private void UpdateParameter()
        {
            animator.SetBool("IsRun", m_CharacterMovement.IsRun);
            animator.SetBool("IsSprint", m_CharacterMovement.IsSprint);
        }
    }
}
