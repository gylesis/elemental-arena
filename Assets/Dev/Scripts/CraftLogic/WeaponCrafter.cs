using Dev.CommonControllers;
using Dev.Infrastructure;
using UnityEngine;
using Zenject;

namespace Dev
{
    public class WeaponCrafter : NetworkContext
    {
        private WeaponCraftStation _weaponCraftStation;
        private PlayersSpawner _playersSpawner;

        [Inject]
        private void Init(WeaponCraftStation weaponCraftStation, PlayersSpawner playersSpawner)
        {
            _playersSpawner = playersSpawner;
            _weaponCraftStation = weaponCraftStation;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData inputData))
            {
                if (inputData.CraftWeaponKeyCode1 || inputData.CraftWeaponKeyCode2 ||inputData.CraftWeaponKeyCode3 ||inputData.CraftWeaponKeyCode4)
                {
                    Debug.Log($"Craft");
                    var tryGetPlayer = _playersSpawner.TryGetPlayer(Object.InputAuthority, out var player);

                    if (tryGetPlayer)
                    {
                        var craftContext = new WeaponCraftContext();
                        craftContext.Runner = Runner;
                        craftContext.FirstElement = ElementType.Earth;
                        craftContext.SecondElement = ElementType.Fire;
                        craftContext.WeaponSpawnParent = player.WeaponParent;
                        craftContext.Player = player;
                            
                        _weaponCraftStation.Craft(craftContext);
                    }
                }
            }
        }
    }
    
    
}