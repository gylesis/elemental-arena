using Dev.Weapons.Ammo;
using UnityEngine;

namespace Dev.Weapons.Guns
{
    public class ExplosiveBombThrower : BombThrower<ExplosiveBomb>
    {
        public override void Shoot(Vector3 origin, Vector3 direction)
        {
            Runner.Spawn(_bombPrefab, origin, Quaternion.identity,
                Object.InputAuthority, (runner, o) =>
                {
                    var bomb = o.GetComponent<ExplosiveBomb>();

                    OnBombBeforeSpawned(bomb);
                    bomb.Init(_deathTime, _damage, direction * _throwPower, _bombExplosionModifier);
                });
        }

    }
}