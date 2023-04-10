using UnityEngine;

namespace Dev
{
    public class PlayerHand : MonoBehaviour
    {
        [SerializeField] private Transform _pointer;
        [SerializeField] private Transform _staffPoint;

        public Transform Pointer => _pointer;

        public Transform StaffPoint => _staffPoint;
    }
}