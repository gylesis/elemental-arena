﻿using Fusion;
using UnityEngine;
using Zenject;

namespace Dev
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private PlayersSpawner _playersSpawner;
        [SerializeField] private NetworkCallbacks _networkCallbacks;
        [SerializeField] private Scoreboard _scoreboard;
        [SerializeField] private FxContainer _fxContainer;
        
        public override void InstallBindings()
        {
            var networkRunner = gameObject.AddComponent<NetworkRunner>();
            networkRunner.ProvideInput = true;

            Container.BindInterfacesAndSelfTo<EntryPoint>().FromSubContainerResolve().ByMethod((container =>
            {
                var sceneManagerDefault = gameObject.AddComponent<NetworkSceneManagerDefault>();

                container.Bind<NetworkSceneManagerDefault>().FromInstance(sceneManagerDefault).AsSingle();

                container.BindInterfacesAndSelfTo<EntryPoint>().AsSingle();
            })).AsSingle();

            Container.Bind<FxManager>().AsSingle().NonLazy();
            Container.Bind<FxContainer>().FromInstance(_fxContainer).AsSingle();
            
            Container.Bind<NetworkRunner>().FromInstance(networkRunner).AsSingle();

            Container.Bind<PlayersSpawner>().FromInstance(_playersSpawner).AsSingle();
            Container.Bind<NetworkCallbacks>().FromInstance(_networkCallbacks).AsSingle();
            Container.Bind<Scoreboard>().FromInstance(_scoreboard).AsSingle();
        }
    }
}