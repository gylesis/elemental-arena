using System.Collections.Generic;
using Dev.CommonControllers;
using Fusion;
using UnityEngine;

namespace Dev
{
    public abstract class WeaponAmmo : NetworkContext
    {
        [SerializeField] private LayerMask _playerLayer;    
        
        protected WeaponAmmonSetupContext Context;

        public virtual void Setup(WeaponAmmonSetupContext setupContext)
        {
            Context = setupContext;
        }

        protected bool OverlapSphere(Vector3 pos, float radius, LayerMask layerMask, out List<LagCompensatedHit> hits)
        {
            hits = new List<LagCompensatedHit>();

            Runner.LagCompensation.OverlapSphere(pos, radius, Object.InputAuthority,
                hits, layerMask);

            return hits.Count > 0;
        }
        
        protected void ExplodePlayer(Vector3 pos, float radius, float explosionForcePower, bool needToCheckWalls = false)
        {
            var overlapSphere = OverlapSphere(pos, radius, _playerLayer, out var hits);

            Debug.Log($"Hits count {hits.Count}");
            
            if (overlapSphere)
            {
                foreach (LagCompensatedHit hit in hits)
                {
                    Debug.Log($"Hit {hit.GameObject.name}", hit.GameObject);
                    
                    var player = hit.GameObject.GetComponent<Player>();

                    PlayerRef owner = Object.InputAuthority;
                    PlayerRef target = player.Object.InputAuthority;

                    if (target == owner) continue;

                    player.Damaged?.Invoke(owner, target);

                    ApplyForceToPlayer(player, explosionForcePower);
                }
            }
        }

        protected void ApplyForceToPlayer(Player player, float forcePower)
        {
            var forceDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 1f));
            forceDirection.Normalize();

            Debug.DrawRay(player.transform.position, forceDirection * 2, Color.blue, 5f);

            player.Rigidbody.Rigidbody.AddForce(forceDirection * forcePower, ForceMode2D.Impulse);
        }
        
    }
}