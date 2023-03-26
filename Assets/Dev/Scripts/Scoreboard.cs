using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Dev
{
    public class Scoreboard : NetworkBehaviour
    {
        [SerializeField] private ScoreBoardUIView _scoreBoardUIViewPrefab;
        
        private Dictionary<PlayerRef, ScoreBoardUIView> Views { get; set; } =
            new Dictionary<PlayerRef, ScoreBoardUIView>();
        
        [Networked]
        private NetworkDictionary<PlayerRef, int> Scores { get; }

        public override async void Spawned()
        {
            if (HasStateAuthority == false)
            {
                await Task.Delay(500);
                
                var views = transform.GetComponentsInChildren<ScoreBoardUIView>(true);

                foreach (ScoreBoardUIView view in views)
                {
                    if (view.Object.InputAuthority.IsNone == false)
                    {
                        view.gameObject.SetActive(true);
                        PlayerRef playerRef = view.Object.InputAuthority;
                        Views.Add(playerRef, view);
                        
                        view.UpdateScore(Scores[playerRef]);
                    }
                }
            }
        }

        [Rpc]
        public void RPC_PlayerLeft(PlayerRef playerRef)
        {
            if (Object.HasStateAuthority)
            {
                Scores.Remove(playerRef);
            }

            var viewKey = Views.FirstOrDefault(x => x.Key == playerRef).Key;

            ScoreBoardUIView scoreBoardUIView = Views[viewKey];
            scoreBoardUIView.Object.RemoveInputAuthority();
            scoreBoardUIView.gameObject.SetActive(false);

            Views.Remove(viewKey);
        }
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_AddPlayer(Player player)
        {
            player.Damaged += RPC_Damaged;
            
            PlayerRef playerRef = player.Object.InputAuthority;

            Scores.Add(playerRef, 0);

            var views = transform.GetComponentsInChildren<ScoreBoardUIView>(true);

            ScoreBoardUIView scoreBoardUIView = views.First(x => x.HasInputAuthority == false);

            scoreBoardUIView.Object.AssignInputAuthority(playerRef);
            
            scoreBoardUIView.UpdateScore(0);
            scoreBoardUIView.gameObject.SetActive(true);
            
            Views.Add(playerRef, scoreBoardUIView);
        }

        [Rpc]
        public void RPC_Damaged(PlayerRef owner, PlayerRef target)
        {
            if (Object.HasStateAuthority)
            {
                Scores.Set(owner, Scores[owner] + 1);
            }

            var score = Scores[owner];

            ScoreBoardUIView view = Views[owner];
            view.UpdateScore(score);
        }
        
    }
}