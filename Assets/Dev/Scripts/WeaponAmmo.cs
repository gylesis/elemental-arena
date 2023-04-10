using System.Collections.Generic;
using Dev.CommonControllers;
using Fusion;
using UnityEngine;

namespace Dev
{
    public abstract class WeaponAmmo : NetworkContext
    {
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
    }
}