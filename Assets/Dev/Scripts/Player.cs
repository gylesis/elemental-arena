using System;
using Dev.Infrastructure;
using Dev.Weapons;
using Fusion;
using UnityEngine;

namespace Dev
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private SpriteRenderer _bodySprite;
        [SerializeField] private NetworkRigidbody2D _rigidbody;
        [SerializeField] private float _speed = 2f;
        [SerializeField] private float _jumpPower = 2;
        [SerializeField] private float _jumpCooldown = 1;

        [SerializeField] private WeaponController _weaponController;

        [SerializeField] private Transform _pointer;

        private Vector3 _mousePos;

        public NetworkRigidbody2D Rigidbody => _rigidbody;

        [Networked(OnChanged = nameof(OnChangeColor))]
        public Color Color { get; set; }

        [Networked] private TickTimer JumpTimer { get; set; }
        [Networked] private Vector3 PointerDirection { get; set; }

        [HideInInspector] [Networked] public NetworkBool AllowToMove { get; set; } = true;
        
        public Action<PlayerRef, PlayerRef> Damaged { get; set; }

        private KeyCode[] _keyCodes =
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
        };
        
        public override void Spawned() // wrong, need to do it locally
        {
            if (HasInputAuthority)
            {
                Color = Color.red;

               // _weaponController.WeaponChanged.Subscribe((OnWeaponChanged));
            }
            else
            {
                Color = Color.blue;
            }
        }

        public override void FixedUpdateNetwork()
        {
            var hasInput = GetInput(out NetworkInputData inputData);

            if (hasInput)
            {
                SetPointerDirection(inputData);

                var allowToJump = JumpTimer.ExpiredOrNotRunning(Runner);

                if (inputData.Jump && allowToJump)
                {
                    Jump();
                }
                
                if (inputData.Fire)
                {
                    Vector3 direction = (_mousePos - transform.position).normalized;

                    Vector3 origin = _pointer.position + direction * 1.2f;

                    _weaponController.TryToFire(origin, direction);
                }
                
                if(AllowToMove == false) return;

                Vector2 originVelocity = _rigidbody.Rigidbody.velocity;
                originVelocity.x = inputData.Horizontal * _speed * Runner.DeltaTime;

                _rigidbody.Rigidbody.velocity = originVelocity;
            }
        }

        private void SetPointerDirection(NetworkInputData inputData)
        {
            _mousePos = inputData.MousePos;
            Vector3 pointerRight = (_pointer.transform.position - _mousePos).normalized;
            _pointer.right = pointerRight;
            PointerDirection = pointerRight;
        }

        private void Jump()
        {
            var jumpDirection = new Vector2(0, 1);

            jumpDirection.Normalize();

            _rigidbody.Rigidbody.AddForce(jumpDirection * _jumpPower, ForceMode2D.Impulse);

            JumpTimer = TickTimer.CreateFromSeconds(Runner, _jumpCooldown);
        }

        public override void Render()
        {
            if (HasStateAuthority == false)
            {
                _pointer.right = PointerDirection;
            }

            if (HasInputAuthority)
            {
                for (int i = 0; i < _keyCodes.Length; i++)
                {
                    if (Input.GetKeyDown(_keyCodes[i]))
                    {
                        int numberPressed = i + 1;
                        _weaponController.RPC_ChooseWeapon(numberPressed);
                    }
                }
                
            }

        }

        private static void OnChangeColor(Changed<Player> changed)
        {
            changed.Behaviour._bodySprite.color = changed.Behaviour.Color;
        }
    }
}