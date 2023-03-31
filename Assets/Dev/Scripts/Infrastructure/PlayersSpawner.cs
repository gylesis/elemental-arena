using System.Collections.Generic;
using Fusion;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure
{
    public class PlayersSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject _playerPrefab;

        [Networked, Capacity(4)]
        private int PlayersCount { get; set; }

        private NetworkRunner _networkRunner;

        private Dictionary<PlayerRef, Player> _players = new Dictionary<PlayerRef, Player>();

        [Inject]
        public void Init(NetworkRunner networkRunner)
        {
            _networkRunner = networkRunner;
        }
        
        public Player SpawnPlayer(PlayerRef playerRef)
        {
            var playersLength = PlayersCount;
            
            var spawnedPlayer = _networkRunner.Spawn(_playerPrefab, Vector2.zero + Vector2.right * playersLength, quaternion.identity, playerRef);
            var player = spawnedPlayer.GetComponent<Player>();

            Runner.SetPlayerObject(playerRef, spawnedPlayer);

            player.Rigidbody.Rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
            
            spawnedPlayer.name = $"Player {spawnedPlayer.InputAuthority.PlayerId}";
            PlayersCount++;

            _players.Add(playerRef,player);

            return player;
        }

        public void PlayerLeft(PlayerRef playerRef)
        {
            Player player = _players[playerRef];
            
            Runner.Despawn(player.Object);

            _players.Remove(playerRef);

            PlayersCount--;
        }
    }
}