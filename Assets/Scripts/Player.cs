using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace MultiplayerRunner
{
    [System.Serializable]
    public class PlayerData
    {
        public string Nickname;
        public int NetId;
        public NetworkIdentity Identity;

        public PlayerData(string nickname, int netId, NetworkIdentity identity)
        {
            Nickname = nickname;
            NetId = netId;
            Identity = identity;
        }
    }

    public static class PlayerDataExtension
    {
        public static void WritePlayerData(this NetworkWriter writer, PlayerData data)
        {
            writer.WriteString(data.Nickname);
            writer.WriteInt(data.NetId);
            writer.WriteNetworkIdentity(data.Identity);
        }

        public static PlayerData ReadPlayerData(this NetworkReader reader)
        {
            return new PlayerData(reader.ReadString(), reader.ReadInt(), reader.ReadNetworkIdentity());
        }
    }

    public class Player : NetworkBehaviour
    {
        public static event UnityAction<PlayerData, int> FragChanged;

        [SerializeField] private Runner m_RunnerPrefab;

        public Runner ActiveRunner { set; get; }

        private PlayerData data;
        public PlayerData Data => data;

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

        public override void OnStartClient()
        {
            base.OnStartClient();

            if(hasAuthority)
            {
                data = new PlayerData(
                    NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname,
                    (int)netId,
                    netIdentity);

                CmdAddPlayer(data);
                CmdUpdateData(data);

                CmdSpawnRunner();
                CmdSetFrag(data, 0);
            }
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            PlayerList.Instance.SvRemovePlayer(data);
        }

        [Command]
        private void CmdUpdateData(PlayerData data)
        {
            this.data = data;
        }

        [Command]
        private void CmdAddPlayer(PlayerData data)
        {
            PlayerList.Instance.SvAddPlayer(data);
        }

        #region Frags
        [Command]
        public void CmdSetFrag(PlayerData data, int frag)
        {
            SvSetFrag(data, frag);
        }

        [Server]
        private void SvSetFrag(PlayerData data, int frag)
        {
            FragChanged?.Invoke(data, frag);

            RpcSetFrag(data, frag);
        }

        [ClientRpc]
        private void RpcSetFrag(PlayerData data, int frag)
        {
            FragChanged?.Invoke(data, frag);
        }
        #endregion

        #region Spawn
        [Command]
        private void CmdSpawnRunner()
        {
            SvSpawnRunner();
        }

        [Server]
        private void SvSpawnRunner()
        {
            if (ActiveRunner != null) return;

            GameObject spawnRunner = Instantiate(m_RunnerPrefab.gameObject,
                                            PlayerSpawner.Instance.GetRandomPoints(),
                                            Quaternion.identity);
            NetworkServer.Spawn(spawnRunner, netIdentity.connectionToClient);

            ActiveRunner = spawnRunner.GetComponent<Runner>();

            RpcSpawnRunner(ActiveRunner.netIdentity);
        }

        [ClientRpc]
        private void RpcSpawnRunner(NetworkIdentity runner)
        {
            ActiveRunner = runner.GetComponent<Runner>();

            if(ActiveRunner != null && ActiveRunner.hasAuthority 
                && ThirdPersonCamera.Instance != null)
            {
                ThirdPersonCamera.Instance.SetTarget(ActiveRunner.transform);
                ActiveRunner.Init(this, ThirdPersonCamera.Instance);
            }
        }
        #endregion

        #region UnSpawn
        [Command]
        private void CmdUnSpawnRunner(NetworkIdentity identity)
        {
            SvUnSpawnRunner(identity);
        }

        [Server]
        private void SvUnSpawnRunner(NetworkIdentity identity)
        {
            if(identity.TryGetComponent<Player>(out var player))
            {
                if(player.ActiveRunner != null)
                {
                    player.ActiveRunner.Deinit();
                    NetworkServer.Destroy(player.ActiveRunner.gameObject);
                }
                player.ActiveRunner = null;
            }
        }
        #endregion

        private void OnEndMatch()
        {
            if (hasAuthority == false) return;

            CmdUnSpawnRunner(netIdentity);
        }

        private void OnStartMatch()
        {
            if (hasAuthority == false) return;

            CmdSpawnRunner();
            CmdSetFrag(data, 0);
        }
    }
}
