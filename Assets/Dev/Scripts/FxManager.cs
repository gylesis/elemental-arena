using System;
using Fusion;
using UniRx;
using UnityEngine;

namespace Dev
{
    public class FxManager 
    {
        private FxContainer _fxContainer;
        private NetworkRunner _runner;

        public static FxManager Instance { get; set; }

        public FxManager(FxContainer fxContainer, NetworkRunner runner)
        {
            _runner = runner;
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
                T effect = _runner.Spawn(effectPrefab, pos, Quaternion.identity, _runner.LocalPlayer);

                Observable.Timer(TimeSpan.FromSeconds(4)).Subscribe((l =>
                {
                    _runner.Despawn(effect.Object);
                }));
                
            }
            
        }
        
    }
}