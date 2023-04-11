using Dev.CommonControllers;
using Dev.Static_Data;
using Fusion;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private PlayersSpawner _playersSpawner;
        [SerializeField] private NetworkCallbacks _networkCallbacks;
        [SerializeField] private Scoreboard _scoreboard;
        [SerializeField] private FxContainer _fxContainer;
        [SerializeField] private FxManager _fxManager;
        [SerializeField] private WeaponCraftRecipesContainer _weaponCraftRecipesContainer;
        [SerializeField] private WeaponsContainer _weaponsContainer;
        
        public override void InstallBindings()
        {
            var networkRunner = gameObject.AddComponent<NetworkRunner>();
            networkRunner.ProvideInput = true;

            Container.Bind<WeaponCraftRecipesContainer>().FromInstance(_weaponCraftRecipesContainer).AsSingle();
            Container.Bind<WeaponsContainer>().FromInstance(_weaponsContainer).AsSingle();
            Container.Bind<WeaponCraftStation>().AsSingle();
            Container.Bind<InputListenerDispatcher>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<EntryPoint>().FromSubContainerResolve().ByMethod((container =>
            {
                var sceneManagerDefault = gameObject.AddComponent<NetworkSceneManagerDefault>();

                container.Bind<NetworkSceneManagerDefault>().FromInstance(sceneManagerDefault).AsSingle();

                container.BindInterfacesAndSelfTo<EntryPoint>().AsSingle();
            })).AsSingle();

            Container.Bind<FxManager>().FromInstance(_fxManager).AsSingle().NonLazy();
            Container.Bind<FxContainer>().FromInstance(_fxContainer).AsSingle();
            
            Container.Bind<NetworkRunner>().FromInstance(networkRunner).AsSingle();

            Container.Bind<PlayersSpawner>().FromInstance(_playersSpawner).AsSingle();
            Container.Bind<NetworkCallbacks>().FromInstance(_networkCallbacks).AsSingle();
            Container.Bind<Scoreboard>().FromInstance(_scoreboard).AsSingle();
        }
    }
}