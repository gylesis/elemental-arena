﻿using Fusion;
using UnityEngine;

namespace Dev
{
    public class MagmaBallStaff : ProjectileStaff<MagmaBall>
    {
        [SerializeField] private MagmaBallShard _magmaBallShardPrefab;
        [SerializeField] private float _shardsForcePower = 2f;
        [SerializeField] private float _shardsLifeTime = 2f; 
        
        public override void Shoot(Vector3 origin, Vector3 direction)
        {
            var setupContext = new WeaponAmmonSetupContext();
            setupContext.Direction = direction;
            setupContext.Force = _forcePower;

            SpawnProjectile(origin, Quaternion.identity, setupContext);
        }

        protected override void OnProjectileBeforeSpawned(MagmaBall projectile, WeaponAmmonSetupContext setupContext)
        {
            projectile.Setup(setupContext);
        }

        protected override void DestroyAmmo(MagmaBall projectile)
        {
            int shardsAmount = Random.Range(3, 7);

            for (int i = 0; i < shardsAmount; i++)
            {
                WeaponAmmonSetupContext setupContext = new WeaponAmmonSetupContext();
                setupContext.Direction = Random.insideUnitCircle;
                setupContext.Force = _shardsForcePower;

                Vector3 spawnPos = projectile.transform.position + setupContext.Direction * 1.05f;

                SpawnAdditionalProjectile(_magmaBallShardPrefab, spawnPos, Quaternion.identity,
                    setupContext, (runner, o) =>
                    {
                        o.GetComponent<MagmaBallShard>().DeathTimer = TickTimer.CreateFromSeconds(Runner, _shardsLifeTime);
                    });  
            }

            base.DestroyAmmo(projectile);
        }
    }
}