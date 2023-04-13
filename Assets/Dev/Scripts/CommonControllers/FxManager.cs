using System;
using Dev.Static_Data;
using Fusion;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.CommonControllers
{
    public class FxManager : NetworkContext
    {
        private FxContainer _fxContainer;

        public static FxManager Instance { get; set; }

        [Inject]
        public void Init(FxContainer fxContainer)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            _fxContainer = fxContainer;
        }

        public void PlayEffect<T>(Vector3 pos) where  T: Effect
        {
            var tryGetEffect = _fxContainer.TryGetEffect<T>(out var effectPrefab);

            if (tryGetEffect)
            {
                T effect = Runner.Spawn(effectPrefab, pos, Quaternion.identity, Object.InputAuthority);
                
                effect.RPC_SetPos(pos);
                
                Observable.Timer(TimeSpan.FromSeconds(4)).Subscribe((l =>
                {
                    Runner.Despawn(effect.Object);
                }));
                
            }
            
        }
    }
}