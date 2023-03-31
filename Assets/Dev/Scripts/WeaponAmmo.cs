using Fusion;

namespace Dev
{
    public abstract class WeaponAmmo : NetworkBehaviour
    {
        protected WeaponAmmonSetupContext Context;

        public virtual void Setup(WeaponAmmonSetupContext setupContext)
        {
            Context = setupContext;
        }
        
    }
}