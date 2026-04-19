using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Sprite[] _sprites;
    private float _frameDuration;
    private int _currentFrame;
    private float _timer;
    private bool _playing;

    public void Play(Sprite[] sprites, float frameDuration)
    {
        _sprites = sprites;
        _frameDuration = frameDuration;
        _currentFrame = 0;
        _timer = 0f;
        _playing = sprites != null && sprites.Length > 0;

        if (_playing)
        {
            if (!_image)
                _image.sprite = _sprites[0];

            if (!_spriteRenderer)
                _spriteRenderer.sprite = _sprites[0];
        }
    }

    public void Stop()
    {
        _playing = false;
    }

    private void Update()
    {
        if (!_playing) return;

        _timer += Time.deltaTime;
        if (_timer >= _frameDuration)
        {
            _timer -= _frameDuration;
            _currentFrame = (_currentFrame + 1) % _sprites.Length;

            if (!_image)
                _image.sprite = _sprites[_currentFrame];
            
            if(!_spriteRenderer)
                _spriteRenderer.sprite = _sprites[_currentFrame];
        }
    }
}