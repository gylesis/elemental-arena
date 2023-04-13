using System;
using Dev.Fx;
using UnityEngine;

namespace Dev.Static_Data
{
    [CreateAssetMenu(menuName = "StaticData/FxContainer", fileName = "FxContainer", order = 0)]
    public class FxContainer : ScriptableObject
    {
        [SerializeField] private Effect[] _effects;

        public bool TryGetEffect<T>(out T foundEffect) where T : Effect
        {
            Type effectType = typeof(T);

            foundEffect = null;

            foreach (Effect effect in _effects)
            {
                if (effect.GetType() == effectType)
                {
                    foundEffect = (T)effect;
                    return true;
                }
            }

            return false;
        }
    }
}