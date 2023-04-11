using System;
using Dev.CommonControllers;
using Dev.Infrastructure;
using Dev.Weapons;
using Fusion;
using UniRx;
using UnityEngine;

namespace Dev
{
    public class Player : NetworkContext
    {
        [SerializeField] private SpriteRenderer _bodySprite;
        [SerializeField] private NetworkRigidbody2D _rigidbody;
        [SerializeField] private float _speed = 2f;
        [SerializeField] private float _jumpPower = 2;
        [SerializeField] private float _jumpCooldown = 1;
        [SerializeField] private NetworkObject _weaponParent;
        [SerializeField] private Animator _animator;
        [SerializeField] private WeaponController _weaponController;
        [SerializeField] private PlayerHand _playerHand;
        [SerializeField] private PlayerChosenElementsState _elementsState;

        public PlayerChosenElementsState ElementsState => _elementsState;

        private Vector3 _mousePos;
        public NetworkRigidbody2D Rigidbody => _rigidbody;
        public NetworkObject WeaponParent => _weaponParent;
        public WeaponController WeaponController => _weaponController;

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

        private static readonly int JumpName = Animator.StringToHash("Jump");
        private static readonly int Fall = Animator.StringToHash("Fall");
        private WeaponCrafter _weaponCrafter;

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

                if (inputData.FireDown)
                {
                    Vector3 direction = (_mousePos - _weaponController.CurrentWeapon.ShootPoint.position).normalized;
                    Vector3 origin = _weaponController.CurrentWeapon.ShootPoint.position;

                    _weaponController.TryToFire(origin, direction);
                }

                if (inputData.FireUp)
                {
                    Vector3 direction = (_mousePos - _weaponController.CurrentWeapon.ShootPoint.position).normalized;
                    Vector3 origin = _weaponController.CurrentWeapon.ShootPoint.position;

                    _weaponController.TryToFireClickedUp(origin, direction);
                }
                else if (_weaponController.CurrentWeapon != null &&
                         _weaponController.CurrentWeapon.ShootDelayTimer.Expired(Runner))
                {
                    Vector3 direction = (_mousePos - _weaponController.CurrentWeapon.ShootPoint.position).normalized;
                    Vector3 origin = _weaponController.CurrentWeapon.ShootPoint.position;

                    _weaponController.TryToFireClickedUp(origin, direction);
                }

                if (inputData.ToCraft)
                {
                    if (_weaponCrafter == null)
                    {
                        _weaponCrafter = FindObjectOfType<WeaponCrafter>();
                    }

                    _weaponCrafter.TryCraft(Object.InputAuthority);
                }

                if (AllowToMove == false) return;

                if (Mathf.Approximately(inputData.Horizontal, 0))
                {
                    RPC_WalkAnimation(false);
                }
                else
                {
                    RPC_WalkAnimation(true);
                }

                Vector2 originVelocity = _rigidbody.Rigidbody.velocity;
                originVelocity.x = inputData.Horizontal * _speed * Runner.DeltaTime;

                _rigidbody.Rigidbody.velocity = originVelocity;
            }
        }

        private void SetPointerDirection(NetworkInputData inputData)
        {
            _mousePos = inputData.MousePos;
            Vector3 pointerRight = (_playerHand.Pointer.transform.position - _mousePos).normalized;
            _playerHand.Pointer.right = pointerRight * -Mathf.Sign(_animator.transform.localScale.x);
            PointerDirection = pointerRight;
        }

        private void Jump()
        {
            var jumpDirection = new Vector2(0, 1);

            jumpDirection.Normalize();

            _rigidbody.Rigidbody.AddForce(jumpDirection * _jumpPower, ForceMode2D.Impulse);

            JumpTimer = TickTimer.CreateFromSeconds(Runner, _jumpCooldown);

            RPC_JumpAnimation();
        }

        [Rpc]
        private void RPC_WalkAnimation(bool walk)
        {
            _animator.SetBool("Walk", walk);
        }

        [Rpc]
        private void RPC_JumpAnimation()
        {
            _animator.SetTrigger(JumpName);

            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe((l => { _animator.SetTrigger(Fall); }));
        }

        public override void Render()
        {
            if (HasStateAuthority == false)
            {
                _playerHand.Pointer.right = PointerDirection;
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

            var sign = Mathf.Sign(PointerDirection.x);

            Vector3 transformLocalScale = _bodySprite.transform.localScale;

            /*
            if (sign == 1)
            {
                transformLocalScale.x = 1;
            }
            else if(sign == -1)
            {
                transformLocalScale.x = -1;
            }*/

            transformLocalScale.x = -sign;

            _bodySprite.transform.localScale = transformLocalScale;
        }

        private static void OnChangeColor(Changed<Player> changed)
        {
            // changed.Behaviour._bodySprite.color = changed.Behaviour.Color;
        }
    }
}