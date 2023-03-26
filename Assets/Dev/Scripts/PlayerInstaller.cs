using UnityEngine;
using Zenject;

namespace Dev
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Player>().FromComponentOnRoot().AsSingle();
        }
    }
}