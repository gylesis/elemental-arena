using Dev.PlayerLogic;
using Fusion;

namespace Dev.CraftLogic
{
    public struct WeaponCraftContext
    {
        public NetworkRunner Runner;
        public NetworkObject WeaponSpawnParent;
        public ElementType FirstElement;
        public ElementType SecondElement;
        public Player Player;
    }
}