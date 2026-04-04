using System;
using Player;
using UnityEngine;
using Zenject;

namespace Furniture
{
    [RequireComponent(typeof(Collider2D))]
    public class FearDecreaseArea : MonoBehaviour
    {
        private IPlayerView _playerView;
        
        [Inject]
        public void Construct(IPlayerView playerView)
        {
            _playerView = playerView;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("collision entered");
            _playerView.ShowInteraction(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _playerView.ShowInteraction(false);
        }
    }
}