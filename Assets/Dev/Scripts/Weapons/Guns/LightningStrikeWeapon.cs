using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Dev.Weapons.Guns
{
    public class LightningStrikeWeapon : HitscanWeapon
    {
        [Range(0.01f, 0.9f)] [SerializeField] private float _affordableDotValue = 0.4f;

        [SerializeField] private float _deathTime = 1.5f;
        [SerializeField] private float _preparationTime = 0.5f;
        [SerializeField] private float _strikeExplosionTimer = 2f;
        [SerializeField] private float _explosionForcePower = 10f;
        
        [SerializeField] private LightningStrikeAmmo _lightningStrikeAmmoPrefab;

        private List<LagCompensatedHit> _hits = new List<LagCompensatedHit>();
        private LightningStrikeAmmo _lightningStrikeAmmo;
        [Networked] private TickTimer PreparationTimer { get; set; }
        
        public override void Shoot(Vector3 origin, Vector3 direction, float power = 1)
        {
            Runner.LagCompensation.RaycastAll(origin, direction, _lenght, Object.InputAuthority, _hits, _layerMask);

            var hitsCount = _hits.Count;

           // Debug.Log($"Hits count {hitsCount}");

            bool hasTarget = false;

            if (hitsCount != 0)
            {
                foreach (LagCompensatedHit hit in _hits)
                {
                    if (hasTarget) break;

                    GameObject hitGameObject = hit.GameObject;

                    //Debug.Log($"{hitGameObject}", hitGameObject);
                        
                    var tryGetComponent = hitGameObject.TryGetComponent<CustomTag>(out var tag);

                    if (tryGetComponent == false) continue;

                    var dot = Vector3.Dot(hit.Normal, Vector3.up);

                  //  Debug.Log($"{dot}");
                    
                   // if (dot < _affordableDotValue) continue;

                    //if (tag.TagType != (TagType.Bounds | TagType.Platform)) continue;

                    hasTarget = true;

                    //Debug.Log($"Hit");

                    ShootLightning(hit.Point);
                }
            }
        }
        
        private void ShootLightning(Vector3 pos)
        {
            LightningStrikeAmmo lightningStrikeAmmo = Runner.Spawn(_lightningStrikeAmmoPrefab, pos, Quaternion.identity, Object.InputAuthority,
                (((runner, obj) =>
                {
                    _lightningStrikeAmmo = obj.GetComponent<LightningStrikeAmmo>();

                    var ammonSetupContext = (new WeaponAmmonSetupContext());
                    ammonSetupContext.Direction = Vector3.down;
                    ammonSetupContext.Force = 1;
                    ammonSetupContext.Power = 1;

                    PreparationTimer = TickTimer.CreateFromSeconds(Runner, _preparationTime);
                    AmmoDestroyTimer = TickTimer.CreateFromSeconds(Runner, _deathTime);

                    _lightningStrikeAmmo.Setup(ammonSetupContext, _preparationTime, _strikeExplosionTimer, _explosionForcePower);
                })));

            if (Object.HasStateAuthority)
            {
                RPC_SetPos(lightningStrikeAmmo.Object, pos);
            }
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (Object.HasStateAuthority)
            {
                if (PreparationTimer.Expired(Runner))
                {
                    PreparationTimer = TickTimer.None;
                    _lightningStrikeAmmo.RPC_Explode();
                }
                
                if (AmmoDestroyTimer.Expired(Runner))
                {
                    AmmoDestroyTimer = TickTimer.None;
                    
                    DestroyAmmo(_lightningStrikeAmmo);
                }
            }
        }
    }
}