using Fusion;
using UnityEngine;

namespace Dev.Weapons.Ammo
{
    public class MagmaBallShard : ProjectileWeaponAmmo<MagmaBallShard>
    {
        [Networked] public TickTimer DeathTimer { get; set; }
        
        public override void Setup(WeaponAmmonSetupContext setupContext)
        {
            base.Setup(setupContext);

            Quaternion rotation = Random.rotation;
            Vector3 eulerAngles = rotation.eulerAngles;
            eulerAngles.x = 0;
            eulerAngles.y = 0;
            rotation.eulerAngles = eulerAngles;
            
            transform.rotation = rotation;
            
            _rigidbody.Rigidbody.AddForce(setupContext.Direction * setupContext.Force * Runner.DeltaTime, ForceMode2D.Impulse);
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority == false) return;

            if (DeathTimer.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
        }
    }
}