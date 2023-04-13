﻿using Dev.CommonControllers;
using Dev.Weapons.Ammo;
using Fusion;
using UnityEngine;

namespace Dev.Weapons.Guns
{
    public abstract class Weapon : NetworkContext
    {
        [SerializeField] protected Transform _shootPoint;
        [SerializeField] protected Transform _view;

        [SerializeField] protected float _cooldown = 1f;
        [SerializeField] protected float _damage = 10;
        [SerializeField] protected float _shootDelay = 0;
        [Networked] public TickTimer CooldownTimer { get; set; }
        [Networked] public TickTimer ShootDelayTimer { get; set; }
        [Networked] protected TickTimer AmmoDestroyTimer { get; set; }
        
        
        public virtual bool AllowToShoot => CooldownTimer.ExpiredOrNotRunning(Runner);

        public float Cooldown => _cooldown;
        public float Damage => _damage;
        public float ShootDelay => _shootDelay;
        public Transform ShootPoint => _shootPoint;
        public WeaponData WeaponData { get; private set; }
        
        public void Init(WeaponData weaponData)
        {
            WeaponData = weaponData;
        }
        
        public virtual void StartShoot(float power) { }
        public abstract void Shoot(Vector3 origin, Vector3 direction, float power = 1);

        public virtual void OnChosen() { }

        public virtual void SetViewState(bool isActive)
        {
            _view.gameObject.SetActive(isActive);
        }
        
        protected virtual void DestroyAmmo(WeaponAmmo ammon)
        {
            AmmoDestroyTimer = TickTimer.None;
            Runner.Despawn(ammon.Object);
        }

    }

}