using UnityEngine;

namespace Dev
{
    public class MagmaBall : ProjectileWeaponAmmo<MagmaBall>
    {

        public override void Setup(WeaponAmmonSetupContext setupContext)
        {
            base.Setup(setupContext);
            
            _rigidbody.Rigidbody.AddForce(setupContext.Direction * setupContext.Force * Runner.DeltaTime, ForceMode2D.Impulse);
        }
    }
}