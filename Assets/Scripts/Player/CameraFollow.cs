using Cinemachine;
using Zenject;

namespace Player
{
    public class CameraFollow : IInitializable
    {
        private Character _player;
        private CinemachineVirtualCamera _virtualCamera;

        public CameraFollow(Character player, CinemachineVirtualCamera virtualCamera)
        {
            _player = player;
            _virtualCamera = virtualCamera;
        }

        public void Initialize()
        {
            _virtualCamera.Follow = _player.transform;
        }
    }
}