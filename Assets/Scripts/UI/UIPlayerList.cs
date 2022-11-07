using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerRunner
{
    public class UIPlayerList : MonoBehaviour
    {
        [SerializeField] private UIPlayerInfo m_PlayerInfoPrefab;

        private List<UIPlayerInfo> allPlayerInfo = new List<UIPlayerInfo>();

        private void Start()
        {
            PlayerList.UpdateList += OnUpdateList;
            Player.FragChanged += OnFragChanged;
        }

        private void OnDestroy()
        {
            PlayerList.UpdateList -= OnUpdateList;
            Player.FragChanged += OnFragChanged;
        }

        private void OnUpdateList(List<PlayerData> players)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            allPlayerInfo.Clear();

            for (int i = 0; i < players.Count; i++)
            {
                UIPlayerInfo playerInfo = Instantiate(m_PlayerInfoPrefab, transform);
                playerInfo.SetPlayer(players[i].Nickname, players[i].NetId);
                allPlayerInfo.Add(playerInfo);
            }
        }

        private void OnFragChanged(PlayerData data, int frags)
        {
            for(int i = 0; i < allPlayerInfo.Count; i++)
            {
                if(allPlayerInfo[i].NetId == data.NetId)
                {
                    allPlayerInfo[i].SetFrag(frags);
                    break;
                }
            }
        }
    }
}
