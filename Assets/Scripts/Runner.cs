using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MultiplayerRunner
{
    public class Runner : NetworkBehaviour
    {
        [SerializeField]
        private CharacterInputController m_CharacterInputController;

        [SerializeField]
        private CharacterMovement m_CharacterMovement;

        [SerializeField]
        private SkinMaterial m_SkinMaterial;
        public SkinMaterial SkinMaterial => m_SkinMaterial;

        [SerializeField]
        private float m_DurationInvul = 3;

        [SerializeField] private Timer timerInvul;
        public Timer TimerInvul => timerInvul;

        [HideInInspector]
        public bool IsInvulnerability;

        private int frags;

        public void Init(Player player, ThirdPersonCamera camera)
        {
            m_CharacterInputController.Init(camera);
            m_CharacterMovement.Init(player, camera);
            timerInvul = TimerCollector.Instance.CreateTimer(m_DurationInvul);
            timerInvul.TimeComleted += OnTimerInvulComleted;
        }

        public void Deinit()
        {
            timerInvul.TimeComleted -= OnTimerInvulComleted;
        }

        private void OnTimerInvulComleted()
        {
            CmdSetInvulnerability(netIdentity, false);
        }

        [Command]
        public void CmdSetInvulnerability(NetworkIdentity identity, bool isInvul)
        {
            SvSetInvulnerability(identity, isInvul);
        }

        [Server]
        public void SvSetInvulnerability(NetworkIdentity identity, bool isInvul)
        {
            if(identity.TryGetComponent<Runner>(out var runner))
            {
                runner.SkinMaterial.SetMaterial(isInvul);
                runner.IsInvulnerability = isInvul;
            }
            
            RpcSetInvulnerability(identity, isInvul);
        }

        [ClientRpc]
        private void RpcSetInvulnerability(NetworkIdentity identity, bool isInvul)
        {
            if (identity.TryGetComponent<Runner>(out var runner))
            {
                runner.SkinMaterial.SetMaterial(isInvul);
                runner.IsInvulnerability = isInvul;
                if (isInvul && runner.TimerInvul != null)
                {
                    runner.TimerInvul.Reset();
                }
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.TryGetComponent<Runner>(out var runner))
            {
                if(m_CharacterMovement.IsSprint && runner.IsInvulnerability == false)
                {
                    CmdSetInvulnerability(runner.netIdentity, true);
                    m_CharacterMovement.Owner.CmdSetFrag(m_CharacterMovement.Owner.Data, ++frags);
                }
            }

            m_CharacterMovement.UnSprint();
        }
    }
}
