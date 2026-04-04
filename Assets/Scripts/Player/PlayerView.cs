using System;
using UnityEngine;

namespace Player
{
    public class PlayerView : MonoBehaviour, IPlayerView
    {
        [SerializeField] private GameObject _interactObject;

        public bool InteactionAvailable { get; private set; }
        
        public void ShowInteraction(bool status)
        {
            _interactObject.gameObject.SetActive(status);
        }

        private void Update()
        {
            InteactionAvailable = _interactObject.gameObject.activeInHierarchy;
        }
    }
}