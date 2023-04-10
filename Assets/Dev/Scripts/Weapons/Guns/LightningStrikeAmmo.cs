using Fusion;
using UnityEngine;

namespace Dev.Weapons.Guns
{
    public class LightningStrikeAmmo : WeaponAmmo
    {
        [SerializeField] private ParticleSystem _preparationParticles;
        [SerializeField] private ParticleSystem _lightningParticles;
            
        public override void Spawned()
        {
            _preparationParticles.gameObject.SetActive(true);
            _lightningParticles.gameObject.SetActive(false);
        }

        [Rpc]
        public void RPC_Explode()
        {
            _preparationParticles.gameObject.SetActive(false);
            _lightningParticles.gameObject.SetActive(true);
        }
       
    }
}