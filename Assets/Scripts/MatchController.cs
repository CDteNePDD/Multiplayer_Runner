using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerRunner
{
    public class MatchController : NetworkBehaviour
    {
        public static MatchController Instance;

        public event UnityAction StartMatch;
        public event UnityAction EndMatch;

        [SerializeField] private float m_DelayBeforeStartMatch = 3;

        [SerializeField] private float m_AmountFragForWin = 3;

        private string nameWinner;
        public string NameWinner => nameWinner;

        private bool isSvActiveMatch;

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }

        private void Start()
        {
            nameWinner = "";
            isSvActiveMatch = true;
            Player.FragChanged += OnFragChanged;
        }

        private void OnDestroy()
        {
            Player.FragChanged -= OnFragChanged;
        }

        private void OnFragChanged(PlayerData data, int frag)
        {
            if(frag >= m_AmountFragForWin)
            {
                SvEndMatch(data.Nickname);
            }
        }

        [Server]
        private void SvEndMatch(string nameWinner)
        {
            if (isSvActiveMatch == false) return;
            isSvActiveMatch = false;

            this.nameWinner = nameWinner;

            if(isServer)
            {
                StartCoroutine(StartEventMatchWithDelay(m_DelayBeforeStartMatch));
                
            }

            if(isServerOnly)
            {
                EndMatch?.Invoke();
            }

            RpsMatchEnd(nameWinner);
        }

        [ClientRpc]
        private void RpsMatchEnd(string nameWinner)
        {
            this.nameWinner = nameWinner;

            if(isClient)
            {
                EndMatch?.Invoke();
            }
        }

        [Server]
        private void SvMatchStart()
        {
            nameWinner = "";
            isSvActiveMatch = true;

            RpsMatchStart();
        }

        [ClientRpc]
        private void RpsMatchStart()
        {
            nameWinner = "";
            StartMatch?.Invoke();
        }

        private IEnumerator StartEventMatchWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            SvMatchStart();
        }
    }
}
