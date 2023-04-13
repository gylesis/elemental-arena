using System.Collections.Generic;
using Dev.CommonControllers;
using Dev.PlayerLogic;
using Fusion;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure
{
    public class PlayersSpawner : NetworkContext
    {
        [SerializeField] private NetworkObject _playerPrefab;

        [Networked, Capacity(4)]
        private int PlayersCount { get; set; }

        private NetworkRunner _networkRunner;

        private Dictionary<PlayerRef, Player> _players = new Dictionary<PlayerRef, Player>();

        public Subject<Player> Spawned { get; } = new Subject<Player>();

        [Inject]
        public void Init(NetworkRunner networkRunner)
        {
            _networkRunner = networkRunner;
        }
        
        public Player SpawnPlayer(PlayerRef playerRef)
        {
            var playersLength = PlayersCount;
            
            var playerNetObj = _networkRunner.Spawn(_playerPrefab, Vector2.zero + Vector2.right * playersLength, quaternion.identity, playerRef);
            var player = playerNetObj.GetComponent<Player>();

            Runner.SetPlayerObject(playerRef, playerNetObj);

            player.Rigidbody.Rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;

            var playerName = $"Player №{playerNetObj.InputAuthority.PlayerId}";
                
            player.RPC_SetName(playerName);
            
            PlayersCount++;

            RPC_OnPlayerSpawnedInvoke(player);
            
            _players.Add(playerRef,player);

            return player;
        }

        [Rpc]
        private void RPC_OnPlayerSpawnedInvoke(Player player)
        {
            Spawned.OnNext(player);
        }
        
        public void PlayerLeft(PlayerRef playerRef)
        {
            Player player = _players[playerRef];
            
            Runner.Despawn(player.Object);

            _players.Remove(playerRef);

            PlayersCount--;
        }

        public bool TryGetPlayer(PlayerRef playerRef, out Player player)
        {
            player = null;
            
            foreach (var keyValuePair in _players)
            {
                if (keyValuePair.Key == playerRef)
                {
                    player = keyValuePair.Value;
                    return true;
                }
            }


            return false;
        }
        
    }
}