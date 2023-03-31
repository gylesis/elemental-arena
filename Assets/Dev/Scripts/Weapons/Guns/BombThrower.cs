using System.Collections.Generic;
using Dev.CommonControllers;
using Dev.Weapons.Ammo;
using Fusion;
using UniRx;
using UnityEngine;

namespace Dev.Weapons.Guns
{
    public abstract class BombThrower<T> : ThrowableWeapon where T : Bomb
    {
        [SerializeField] protected T _bombPrefab;

        [SerializeField] protected float _bombExplosionModifier = 30f;

        [SerializeField] protected float _deathTime = 5f;
        [SerializeField] protected float _throwPower = 20;

        protected List<NetworkObject> _bombs = new List<NetworkObject>();

        protected virtual void OnBombBeforeSpawned(Bomb bomb)
        {
            _bombs.Add(bomb.Object);

            bomb.ToDestroy.Take(1).Subscribe(OnBombToDestroy);
        }

        private void OnBombToDestroy(Bomb bomb)
        {
            RPC_OnBombDespawnForAll(bomb);
            OnBombDespawn(bomb);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            foreach (NetworkObject networkObject in _bombs)
            {
                runner.Despawn(networkObject);
            }

            _bombs.Clear();
        }

        protected void OnBombDespawn(Bomb bomb)
        {
            _bombs.Remove(bomb.Object);

            Runner.Despawn(bomb.Object);
        }

        protected virtual void RPC_OnBombDespawnForAll(Bomb bomb)
        {
            FxManager.Instance.PlayEffect<BombExplosionEffect>(bomb.transform.position);
        }
        
    }
}