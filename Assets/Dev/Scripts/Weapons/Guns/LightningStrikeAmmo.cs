using DG.Tweening;
using Fusion;
using UnityEngine;

namespace Dev.Weapons.Guns
{
    public class LightningStrikeAmmo : WeaponAmmo
    {
        [SerializeField] private Transform _lightningBolt;
        [SerializeField] private Transform _lightningStrike;

        private float _timePreparationToExplode;
        private float _explosionRadius;
        private float _forcePower;

        public virtual void Setup(WeaponAmmonSetupContext setupContext, float preparationTime, float explosionRadius,
            float forcePower)
        {
            Context = setupContext;

            RPC_SetInitValues(preparationTime, explosionRadius, forcePower);
        }

        [Rpc]
        private void RPC_SetInitValues(float preparationTime, float explosionRadius, float forcePower)
        {
            _forcePower = forcePower;
            _timePreparationToExplode = preparationTime;
            _explosionRadius = explosionRadius;
        }

        public override void Spawned()
        {
            _lightningStrike.gameObject.SetActive(false);

            _lightningBolt.gameObject.SetActive(false);
            _lightningBolt.transform.localScale = Vector3.one * Mathf.Epsilon;
            _lightningBolt.gameObject.SetActive(true);

            _lightningBolt.transform.DOScale(1, _timePreparationToExplode);
        }

        [Rpc]
        public void RPC_Explode()
        {
            _lightningStrike.transform.localScale = Vector3.one * Mathf.Epsilon;
            _lightningStrike.gameObject.SetActive(true);

            _lightningStrike.DOScale(1, 0.2f).OnComplete((OnLightningStrikeExplode));

            void OnLightningStrikeExplode()
            {
                if (Object.HasStateAuthority)
                {
                    ExplodePlayer(transform.position, _explosionRadius, _forcePower);
                }
            }
        }
    }
}