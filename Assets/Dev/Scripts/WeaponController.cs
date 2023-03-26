using Fusion;
using UniRx;
using UnityEngine;

namespace Dev
{
    public class WeaponController : NetworkBehaviour
    {
        [SerializeField] private Weapon[] _weapons;

        private WeaponUiView _weaponUiView;
        private Player _player;
        
        public bool AllowToShoot { get; set; } = true;
        public Weapon CurrentWeapon { get; set; }

        public Subject<Weapon> WeaponChanged { get; } = new Subject<Weapon>();

        public override void Spawned()
        {
            _weaponUiView = FindObjectOfType<WeaponUiView>();
            
            if (Object.HasInputAuthority)
            {
                RPC_ChooseWeapon(1);
            }

            RPC_SelectViewWeapon();
        }

        public void TryToFire(Vector3 originPos, Vector3 direction)
        {
            AllowToShoot = CurrentWeapon.CooldownTimer.ExpiredOrNotRunning(Runner);

            if (AllowToShoot)
            {
                var cooldown = CurrentWeapon.Cooldown;

                CurrentWeapon.Shoot(originPos, direction);
                CurrentWeapon.CooldownTimer = TickTimer.CreateFromSeconds(Runner, cooldown);

                if (Object.HasInputAuthority)
                {
                    _weaponUiView.ShootReloadView(cooldown, cooldown);
                }
            }
        }

        private void OnWeaponChanged(Weapon weapon)
        {
            float time;
            var maxCooldown = weapon.Cooldown;

            if (weapon.CooldownTimer.ExpiredOrNotRunning(Runner) == false)
            {
                var remainingTime = weapon.CooldownTimer.RemainingTime(Runner);
                time = remainingTime.Value;
            }
            else
            {
                time = 0;
            }

            _weaponUiView.ShootReloadView(time, maxCooldown);

        }

        [Rpc]
        public void RPC_ChooseWeapon(int index)
        {
            var weaponIndex = Mathf.Clamp(index - 1, 0, _weapons.Length - 1);
            
            Weapon chosenWeapon = _weapons[weaponIndex];

            if(CurrentWeapon == chosenWeapon) return;
            
            CurrentWeapon = chosenWeapon;
            CurrentWeapon.OnChosen();
           
            WeaponChanged.OnNext(CurrentWeapon);
            OnWeaponChanged(chosenWeapon);
            
            RPC_SelectViewWeapon();
            
            if (Object.HasInputAuthority)
            {
                Debug.Log($"Chosen weapon is {chosenWeapon.name}");
            }
        }

        [Rpc]
        private void RPC_SelectViewWeapon()
        {
            foreach (Weapon weapon in _weapons)
            {
                if (weapon == CurrentWeapon)
                {
                    weapon.SetViewState(true);
                    continue;
                }

                weapon.SetViewState(false);
            }
   
        }
        
        
    }
}