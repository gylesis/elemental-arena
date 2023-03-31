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

        private TProjectileType _projectileType;

        [Networked] protected TickTimer ExplosionTimer { get; set; }

        protected void SpawnProjectile(Vector3 pos, Quaternion rotation,
            WeaponAmmonSetupContext weaponAmmonSetupContext)
        {
            _projectileType = Runner.Spawn(_projectilePrefab, pos, rotation,
                Object.InputAuthority, (runner, o) =>
                {
                    TProjectileType projectile = o.GetComponent<TProjectileType>();

                    OnProjectileBeforeSpawned(projectile, weaponAmmonSetupContext);
                });

            ExplosionTimer = TickTimer.CreateFromSeconds(Runner, _explosionTime);
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

        protected abstract void OnProjectileBeforeSpawned(TProjectileType projectile,
            WeaponAmmonSetupContext setupContext);

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority == false) return;

            if (ExplosionTimer.Expired(Runner))
            {
                DestroyAmmo(_projectileType);
            }
        }

        protected virtual void DestroyAmmo(TProjectileType projectile)
        {
            ExplosionTimer = TickTimer.None;
            Runner.Despawn(projectile.Object);
        }
    }
}