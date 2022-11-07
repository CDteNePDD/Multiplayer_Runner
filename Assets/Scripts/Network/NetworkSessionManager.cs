using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MultiplayerRunner
{
    public class NetworkSessionManager : NetworkManager
    {
        public static NetworkSessionManager Instance => singleton as NetworkSessionManager;

        [SerializeField] private MatchController matchController;
        public static MatchController Match => Instance.matchController;
    }
}
