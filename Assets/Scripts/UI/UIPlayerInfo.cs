using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerRunner
{
    public class UIPlayerInfo : MonoBehaviour
    {
        [SerializeField] private Text m_TextNickName;
        [SerializeField] private Text m_TextFrags;

        private int netId;
        public int NetId => netId;

        public void SetPlayer(string nickname, int netId)
        {
            m_TextNickName.text = nickname;
            this.netId = netId;
        }

        public void SetFrag(int frag)
        {
            m_TextFrags.text = frag.ToString();
        }
    }
}
