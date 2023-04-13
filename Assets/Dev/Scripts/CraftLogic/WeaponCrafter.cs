using Dev.CommonControllers;
using Dev.Infrastructure;
using Fusion;
using Zenject;

namespace Dev.CraftLogic
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

        public void TryCraft(PlayerRef playerRef)
        {   
            //Debug.Log($"TryCraft");
            var tryGetPlayer = _playersSpawner.TryGetPlayer(playerRef, out var player);

            if (tryGetPlayer == false) return;

            var craftContext = new WeaponCraftContext();

            craftContext.Runner = Runner;
            craftContext.FirstElement = player.ElementsState.GetElement(1);
            craftContext.SecondElement = player.ElementsState.GetElement(2);
            craftContext.WeaponSpawnParent = player.WeaponParent;
            craftContext.Player = player;
                            
            _weaponCraftStation.TryCraft(craftContext);
        }
        
    }
}