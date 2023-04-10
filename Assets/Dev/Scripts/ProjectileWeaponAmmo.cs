using System.Collections.Generic;
using Fusion;
using UniRx;
using UnityEngine;

namespace Dev
{
    [RequireComponent(typeof(NetworkRigidbody2D))]
    [RequireComponent(typeof(Collider))]
    public abstract class ProjectileWeaponAmmo<TProjectileType> : WeaponAmmo where TProjectileType : WeaponAmmo
    {
        [SerializeField] protected NetworkRigidbody2D _rigidbody;

        public NetworkRigidbody2D Rigidbody => _rigidbody;

        public Subject<TProjectileType> Collision { get; set; } = new Subject<TProjectileType>();
        
        public void Explode(LayerMask layerMask, float explosionRadius)
        {
            var overlapSphere = OverlapSphere(transform.position, explosionRadius, layerMask, out var hits);

            bool hasTarget = false;

            if (overlapSphere)
            {
                foreach (LagCompensatedHit hit in hits)
                {
                    if (hasTarget) break;

                    //Debug.Log($"Hit {hit.GameObject.name}");

                    var player = hit.GameObject.GetComponent<Player>();

                    PlayerRef owner = Object.InputAuthority;
                    PlayerRef target = player.Object.InputAuthority;

                    if (target == owner) continue;

                    hasTarget = true;
                }
            }
        }

        
        
    }
}