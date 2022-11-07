using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerRunner
{
    public class PlayerList : NetworkBehaviour
    {
        public static PlayerList Instance;

        public static UnityAction<List<PlayerData>> UpdateList;

        private List<PlayerData> allPlayerData = new List<PlayerData>();

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }

        [Server]
        public void SvAddPlayer(PlayerData data)
        {
            allPlayerData.Add(data);

            RpsClearAllPlayer();

            foreach (var player in allPlayerData)
            {
                RpcAddPlayer(player);
            }
        }

        [ClientRpc]
        private void RpcAddPlayer(PlayerData data)
        {
            if(isClient == false || isServer == false) //is not Host
            {
                allPlayerData.Add(data);
            }
            UpdateList?.Invoke(allPlayerData);
        }

        [ClientRpc]
        private void RpsClearAllPlayer()
        {
            if (isServer) return;
            allPlayerData.Clear();
        }

        [Server]
        public void SvRemovePlayer(PlayerData data)
        {
            for(int i = 0; i < allPlayerData.Count; i++)
            {
                if(allPlayerData[i].NetId == data.NetId )
                {
                    allPlayerData.RemoveAt(i);
                }
            }

            RpsClearAllPlayer();

            foreach (var player in allPlayerData)
            {
                RpcAddPlayer(player);
            }
        }
    }
}
