using System;
using System.Collections.Generic;
using Dev.CraftLogic;
using Dev.PlayerLogic;
using Dev.UI;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Dev.Infrastructure
{
    public class NetworkCallbacks : NetworkObject, INetworkRunnerCallbacks
    {
        private PlayersSpawner _playersSpawner;
        private Scoreboard _scoreboard;
        private InputListenerDispatcher _inputListenerDispatcher;

        [Inject]
        public void Init(PlayersSpawner playersSpawner, Scoreboard scoreboard, InputListenerDispatcher inputListenerDispatcher)
        {
            _inputListenerDispatcher = inputListenerDispatcher;
            _scoreboard = scoreboard;
            _playersSpawner = playersSpawner;
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"Player {player} joined the game!");

            if (Runner.IsServer)
            {
                Player spawnedPlayer = _playersSpawner.SpawnPlayer(player);

                _scoreboard.RPC_AddPlayer(spawnedPlayer);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"Player {player} has left!");

            if (Runner.IsServer)
            {
                _scoreboard.RPC_PlayerLeft(player);
                _playersSpawner.PlayerLeft(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var horizontal = Input.GetAxisRaw("Horizontal");

            var networkInput = new NetworkInputData();
            networkInput.Horizontal = horizontal;
            networkInput.Jump = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space);
            networkInput.FireDown = Input.GetMouseButton(0);
            networkInput.FireUp = Input.GetMouseButtonUp(0);
            
            
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            point.z = 0;

            networkInput.MousePos = point;

            networkInput.ToCraft = Input.GetKeyDown(KeyCode.C);
            
            input.Set(networkInput);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

        public void OnConnectedToServer(NetworkRunner runner) { }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
           // Debug.Log($"Disconnected from the server!");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
           // Debug.Log($"Connection request from {request.RemoteAddress}");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
          //  Debug.Log($"Scene load start");
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
          //  Debug.Log($"Scene load done");
        }
    }

    public struct NetworkInputData : INetworkInput
    {
        public float Horizontal;
        public float Vertical;
        public bool Jump;
        public bool FireDown;
        public bool FireUp;
        public Vector3 MousePos;

        public bool ToCraft;
    }
}   