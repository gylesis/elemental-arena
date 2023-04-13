using System;
using Fusion;
using UniRx;
using UnityEngine;

namespace Dev
{
    public class EarthArmorStaffWeapon : ArmorStaffWeapon
    {
        [SerializeField] private float _composeDuration = 0.5f;

        public override bool AllowToShoot => !HasArmor;

        [Networked] private NetworkBool HasArmor { get; set; }

        private Player _player;
        private Armor _armor;

        public override void Shoot(Vector3 origin, Vector3 direction, float power = 1)
        {
            if (Object.HasStateAuthority)
            {
                NetworkObject networkObject = Runner.GetPlayerObject(Object.InputAuthority);
                _player = networkObject.GetComponent<Player>();

                _armor = Runner.Spawn(_armorPrefab, Vector3.zero, Quaternion.identity, Object.InputAuthority,
                    (OnBeforeSpawned));

                HasArmor = true;
                
                void OnBeforeSpawned(NetworkRunner runner, NetworkObject obj)
                {
                    var armor = obj.GetComponent<Armor>();

                    armor.RPC_SetParent(_player.ArmorParent);
                    armor.RPC_SetLocalPos(Vector3.zero);

                    armor.RPC_DoScale(_composeDuration);
                }
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority)
            {
                if (CooldownTimer.Expired(Runner))
                {
                    DestroyArmor();
                    CooldownTimer = TickTimer.None;
                }
            }
        }

        private void DestroyArmor()
        {
            _armor.RPC_DoScale(0.5f, 0);

            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe((l =>
            {
                HasArmor = false;
                Runner.Despawn(_armor.Object);
            }));
        }
    }
}