using UnityEngine;

namespace Dev
{
    public class MagmaBall : ProjectileWeaponAmmo<MagmaBall>
    {
        [SerializeField] private Collider2D _collider;
        public Collider2D Collider => _collider;

        public override void Setup(WeaponAmmonSetupContext setupContext)
        {
            base.Setup(setupContext);

            _rigidbody.Rigidbody.AddForce(setupContext.Direction * setupContext.Force * Runner.DeltaTime, ForceMode2D.Impulse);
        }
    }
}