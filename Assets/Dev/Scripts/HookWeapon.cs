using Fusion;
using UnityEngine;

namespace Dev
{
    public class HookWeapon : HitscanWeapon
    {
        [SerializeField] private float _hookPower = 5f;
        [SerializeField] private LineRenderer _lineRendererPrefab;

        [SerializeField] private float _hookDuration = 3f;

        private Vector3 _hookPoint;
        [Networked] private float OriginGravityScale { get; set; }

        private LineRenderer _spawnedLineRenderer;

        [Networked] private TickTimer HookTimer { get; set; }
        [Networked] private NetworkBool IsHooked { get; set; }

        private Player _hookedPlayer;


        public override void Shoot(Vector3 origin, Vector3 direction)
        {
            var raycast = Runner.LagCompensation.Raycast(origin, (Vector2)direction, _lenght, Object.InputAuthority,
                out var hit, _layerMask);

            if (raycast)
            {
                RPC_SetPlayer(Runner.GetPlayerObject(Object.InputAuthority).GetComponent<Player>());

                _hookedPlayer.AllowToMove = false;
                
                Vector3 playerPos = _hookedPlayer.transform.position;
                Vector2 directionToPoint = (_hookPoint - playerPos).normalized;

                _hookPoint = hit.Point + (playerPos - hit.Point).normalized * _hookedPlayer.transform.localScale.x;

                Vector3[] points = new Vector3[] { origin, hit.Point };

                RPC_SpawnLineRenderer(points);

                _hookedPlayer.Rigidbody.Rigidbody.gravityScale = 0;

                IsHooked = true;

                HookTimer = TickTimer.CreateFromSeconds(Runner, _hookDuration);

                _hookedPlayer.Rigidbody.Rigidbody.velocity = Vector2.zero;

               // _hookedPlayer.Rigidbody.Rigidbody.AddForce(directionToPoint * _hookPower, ForceMode2D.Impulse);
            }
        }

        [Rpc]
        private void RPC_SetPlayer(Player player)
        {
            if (_hookedPlayer == null)
            {
                _hookedPlayer = player;
                OriginGravityScale = _hookedPlayer.Rigidbody.Rigidbody.gravityScale;
            }
        }

        [Rpc]
        private void RPC_SpawnLineRenderer(Vector3[] points)
        {
            if (_spawnedLineRenderer == null)
            {
                _spawnedLineRenderer = Instantiate(_lineRendererPrefab);

                _spawnedLineRenderer.positionCount = points.Length;
                _spawnedLineRenderer.SetPositions(points);
            }
        }

        public override void OnChosen()
        {
            
        }

        [Rpc]
        private void RPC_DestroyLineRenderer()
        {
            if (_spawnedLineRenderer != null)
            {
                Destroy(_spawnedLineRenderer.gameObject);
                _spawnedLineRenderer = null;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (IsHooked)
            {
                if (Object.HasStateAuthority)
                {
                    if (HookTimer.ExpiredOrNotRunning(Runner))
                    {
                        //_hookedPlayer.Rigidbody.Rigidbody.velocity = Vector2.zero;
                        _hookedPlayer.Rigidbody.Rigidbody.angularVelocity = 0;
                        _hookedPlayer.Rigidbody.Rigidbody.gravityScale = OriginGravityScale;
                    
                        IsHooked = false;

                        RPC_DestroyLineRenderer();

                        _hookedPlayer.AllowToMove = true;
                    }
                    else
                    {
                        MovePlayerTowardsHookPoint();
                    }
                }
            }

            if (GetInput<NetworkInputData>(out var input))
            {
                if (input.Jump && IsHooked)
                {
                    HookTimer = TickTimer.None;
                }
            }
            
        }

        private void MovePlayerTowardsHookPoint()
        {
            Vector3 direction = (_hookPoint - _hookedPlayer.transform.position);
            Vector2 velocity;

            if (direction.magnitude > 0.4f)
            {
                // _hookedRigidbody.transform.position += direction.normalized * _hookPower * Runner.DeltaTime;
                velocity = direction.normalized * _hookPower * Runner.DeltaTime;
            }
            else
            {
                velocity = Vector2.zero;
            }

            _hookedPlayer.Rigidbody.Rigidbody.velocity = velocity;
        }
    }
}