using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerView : MonoBehaviour, IPlayerView
    {
        [SerializeField] private GameObject _interactObject;
        [SerializeField] private TMP_Text _hintText;

        private bool _isSuppressed;
        private bool _wantShow;

        public bool InteactionAvailable { get; private set; }
        
        public void ShowInteraction(bool status)
        {
            _wantShow = status;
            if (_isSuppressed)
            {
                _interactObject.gameObject.SetActive(false);
                return;
            }
            _interactObject.gameObject.SetActive(status);
        }

        public void SetHintText(string text)
        {
            if (_hintText != null)
                _hintText.text = text;
        }

        public void SetInteractionSuppressed(bool suppressed)
        {
            _isSuppressed = suppressed;
            if (suppressed)
                _interactObject.gameObject.SetActive(false);
            else if (_wantShow)
                _interactObject.gameObject.SetActive(true);
        }

        private void Update()
        {
            InteactionAvailable = _interactObject.gameObject.activeInHierarchy;
        }
    }
}