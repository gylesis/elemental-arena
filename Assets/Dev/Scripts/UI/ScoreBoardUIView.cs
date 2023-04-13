using Fusion;
using TMPro;
using UnityEngine;

namespace Dev.UI
{
    public class ScoreBoardUIView : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _score;
        
        public void UpdateScore(int score)
        {
            _score.text = $"Player {Object.InputAuthority.PlayerId}: {score}";
        }
        
    }
}