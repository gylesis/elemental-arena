using Dev.Weapons.Ammo;
using UnityEngine;

namespace Dev.Weapons.Guns
{
    public class StickyBombThrower : BombThrower<StickyBomb>
    {
        [SerializeField] protected float _expandTime = 1;

        public override void Shoot(Vector3 origin, Vector3 direction)
        {
            Runner.Spawn(_bombPrefab, origin, Quaternion.identity,
                Object.InputAuthority, (runner, o) =>
                {
                    var bomb = o.GetComponent<StickyBomb>();

                    OnBombBeforeSpawned(bomb);

                    bomb.Init(_expandTime, _deathTime, _damage, direction * _throwPower, _bombExplosionModifier);
                });
        }

       
    }
}