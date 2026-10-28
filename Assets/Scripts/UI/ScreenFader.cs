using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField] private Image _overlay;
        [SerializeField, Min(0f)] private float _duration;

        public async UniTask FadeInAsync()
        {
            await _overlay.DOFade(1f, _duration)
                .SetLink(gameObject)
                .AsyncWaitForCompletion();
        }

        public async UniTask FadeOutAsync()
        {
            await _overlay.DOFade(0f, _duration)
                .SetLink(gameObject)
                .AsyncWaitForCompletion();
        }
    }
}
