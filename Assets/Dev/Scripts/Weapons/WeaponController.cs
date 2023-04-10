using Dev.Weapons.Guns;
using Dev.Weapons.View;
using Fusion;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace Dev.Weapons
{
    public class WeaponController : NetworkBehaviour
    {
        [SerializeField] private Weapon[] _weapons;

        private WeaponUiView _weaponUiView;
        private Player _player;

        public bool AllowToShoot { get; set; } = true;
        [Networked] [CanBeNull] public Weapon CurrentWeapon { get; set; }

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
                var shootDelay = CurrentWeapon.ShootDelay;

                if (shootDelay == 0)
                {
                    Shoot(originPos, direction);
                }
                else if (CurrentWeapon.ShootDelayTimer.IsRunning == false)
                {
                    CurrentWeapon.ShootDelayTimer = TickTimer.CreateFromSeconds(Runner, shootDelay);
                }
            }
        }

        public void TryToFireClickedUp(Vector3 originPos, Vector3 direction)
        {
            var shootDelay = CurrentWeapon.ShootDelay;

            if (shootDelay == 0) return;

            AllowToShoot = CurrentWeapon.CooldownTimer.ExpiredOrNotRunning(Runner);

            if (AllowToShoot)
            {
                var power = CurrentWeapon.ShootDelayTimer.RemainingTime(Runner) / shootDelay;

                Shoot(originPos, direction, 1 - power.Value);
            }
        }

        private void Shoot(Vector3 originPos, Vector3 direction, float power = 1)
        {
            var cooldown = CurrentWeapon.Cooldown;

            //Debug.Log($"Power {power}");

            CurrentWeapon.Shoot(originPos, direction, power);
            CurrentWeapon.CooldownTimer = TickTimer.CreateFromSeconds(Runner, cooldown);
            CurrentWeapon.ShootDelayTimer = TickTimer.None;

            _weaponUiView.ShootReloadView(cooldown, cooldown);
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasInputAuthority == false) return;

            if (CurrentWeapon == null) return;

            if (CurrentWeapon.ShootDelayTimer.IsRunning)
            {
                var power = CurrentWeapon.ShootDelayTimer.RemainingTime(Runner) / CurrentWeapon.ShootDelay;

                var powerValue = 1 - power.Value;
                CurrentWeapon.StartShoot(powerValue);
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

            if (CurrentWeapon == chosenWeapon) return;

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
            return;

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