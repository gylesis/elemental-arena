using Dev.Weapons.Guns;
using Fusion;
using UnityEngine;

namespace Dev
{
    public abstract class ProjectileStaff<TProjectileType> : StaffWeapon where TProjectileType : WeaponAmmo
    {
        [SerializeField] private TProjectileType _projectilePrefab;

        [SerializeField] protected float _forcePower = 5f;
        [SerializeField] protected float _explosionRadius = 2f;
        [SerializeField] protected float _explosionTime = 2f;

        [Networked] protected NetworkObject _spawnedProjectile { get; set; }

        protected void SpawnProjectile(Vector3 pos, Quaternion rotation)
        {
            TProjectileType projectileType = Runner.Spawn(_projectilePrefab, pos, rotation,
                Object.InputAuthority, (runner, o) =>
                {
                    TProjectileType projectile = o.GetComponent<TProjectileType>();

                    RPC_SetParent(projectile.Object, Object);
                    
                    OnProjectileBeforeSpawned(projectile);
                });

            if (Object.HasStateAuthority)
            {
                _spawnedProjectile = projectileType.Object;
            }
        }
        protected void SpawnAdditionalProjectile<TProjectile>(TProjectile projectilePrefab, Vector3 pos,
            Quaternion rotation,
            WeaponAmmonSetupContext weaponAmmonSetupContext, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null)
            where TProjectile : WeaponAmmo
        {
            Runner.Spawn(projectilePrefab, pos, rotation,
                Object.InputAuthority, (runner, o) =>
                {
                    TProjectile projectile = o.GetComponent<TProjectile>();

                    projectile.Setup(weaponAmmonSetupContext);

                    onBeforeSpawned?.Invoke(runner, o);
                });
        }

        // protected abstract void OnProjectileCollided();

        protected abstract void OnProjectileBeforeSpawned(TProjectileType projectile);

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            
            if (Object.HasStateAuthority == false) return;

            if (DestroyTimer.Expired(Runner))
            {
                DestroyAmmo(_spawnedProjectile.GetComponent<TProjectileType>());
            }
        }

       
    }
}