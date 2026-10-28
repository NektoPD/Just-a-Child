using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace UI
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField] private Image _overlay;
        [SerializeField] private float _duration;

        public async UniTask FadeInAsync()
        {
            await transform.DOMoveX(2, 10);
            //await _overlay.DOFade(1f, _duration);
        }

        public async UniTask FadeOutAsync()
        {
            await _overlay.DOFade(0f, _duration).ToUniTask();
        }
    }
    }
}
