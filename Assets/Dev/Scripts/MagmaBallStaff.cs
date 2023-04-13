using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dev
{
    public class MagmaBallStaff : ProjectileStaff<MagmaBall>
    {
        [SerializeField] private MagmaBallShard _magmaBallShardPrefab;
        [SerializeField] private float _shardsForcePower = 2f;
        [SerializeField] private float _shardsLifeTime = 2f;

        public override void StartShoot(float power)
        {
            if (SpawnedProjectile == null)
            {
                Vector3 origin = _shootPoint.position;

               // Debug.Log($"{Object.InputAuthority}: Spawn Projectile \n Has state authority {Object.HasStateAuthority}, Input authority {Object.HasInputAuthority}");

                SpawnProjectile(origin, Quaternion.identity);
            }
            else
            {
                SpawnedProjectile.transform.localScale = Vector3.one * (power + 1);
            }
        }   

        public override void Shoot(Vector3 origin, Vector3 direction, float power = 1)
        {
            //Debug.Log($"{Object.InputAuthority}: Shoot \n Has state authority {Object.HasStateAuthority}, Input authority {Object.HasInputAuthority}");
                
            if(Object.HasStateAuthority == false) return;
            
            RPC_SetParent(SpawnedProjectile, null);
            
            var setupContext = new WeaponAmmonSetupContext();
            setupContext.Direction = direction;
            setupContext.Power = power;
            setupContext.Force = _forcePower;

            var magmaBall = SpawnedProjectile.GetComponent<MagmaBall>();
            
            magmaBall.Collider.enabled = true;
            magmaBall.Rigidbody.Rigidbody.isKinematic = false;
            
            AmmoDestroyTimer = TickTimer.CreateFromSeconds(Runner, _explosionTime);
            
            magmaBall.Setup(setupContext);
        }

        protected override void OnProjectileBeforeSpawned(MagmaBall projectile)
        {
            //Debug.Log($"{Object.InputAuthority}: Projectile before spawned \nHas state authority {Object.HasStateAuthority}, Input authority {Object.HasInputAuthority}");

            projectile.Collider.enabled = false;
            projectile.Rigidbody.Rigidbody.isKinematic = true;

            SpawnedProjectile = projectile.Object;
        }

        protected override void DestroyAmmo(WeaponAmmo magmaBall)
        {
            int shardsAmount = Random.Range(3, 7);  

            for (int i = 0; i < shardsAmount; i++)
            {
                WeaponAmmonSetupContext setupContext = new WeaponAmmonSetupContext();
                setupContext.Direction = Random.insideUnitCircle;
                setupContext.Force = _shardsForcePower;

                Vector3 spawnPos = magmaBall.transform.position + setupContext.Direction * 1.05f;

                SpawnAdditionalProjectile(_magmaBallShardPrefab, spawnPos, Quaternion.identity,
                    setupContext, (runner, o) =>
                    {
                        o.GetComponent<MagmaBallShard>().DeathTimer = TickTimer.CreateFromSeconds(Runner, _shardsLifeTime);
                    });  
            }

            base.DestroyAmmo(magmaBall);
        }
    }
}