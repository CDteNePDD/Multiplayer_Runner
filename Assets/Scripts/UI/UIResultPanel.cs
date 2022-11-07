using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerRunner
{
    public class UIResultPanel : MonoBehaviour
    {
        [SerializeField] private GameObject m_Panel;

        [SerializeField] private Text m_TextWinner;

        private void Start()
        {
            NetworkSessionManager.Match.StartMatch += OnStartMatch;
            NetworkSessionManager.Match.EndMatch += OnEndMatch;
        }

        private void OnDestroy()
        {
            NetworkSessionManager.Match.StartMatch -= OnStartMatch;
            NetworkSessionManager.Match.EndMatch -= OnEndMatch;
        }

        private void OnEndMatch()
        {
            m_Panel.SetActive(true);
            m_TextWinner.text = MatchController.Instance.NameWinner;
        }

        private void OnStartMatch()
        {
            m_Panel.SetActive(false);
        }
    }
}
