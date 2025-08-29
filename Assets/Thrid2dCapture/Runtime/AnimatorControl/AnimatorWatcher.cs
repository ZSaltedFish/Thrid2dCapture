using UnityEngine;

namespace com.knight.thrid2dcapture
{
    [RequireComponent(typeof(ScreenShoot))]
    [RequireComponent(typeof(Animator))]
    public class AnimatorWatcher : MonoBehaviour
    {
        public AnimationClip[] Clips;
        public ScreenShoot Shoot;

        private PlayableController _playable;
        private RotateController _rotate;
        private int _currentClip = 0;
        private bool _isFinished;

        public void Start()
        {
            if (Clips.Length == 0) return;
            Shoot = GetComponent<ScreenShoot>();
            _playable = new PlayableController(GetComponent<Animator>());
            _rotate = new RotateController(gameObject);
            _currentClip = 0;

            _playable.InitClip(Clips[0]);
            _isFinished = false;
        }

        public void Update()
        {
            if (_isFinished) return;
            if (_playable == null) return;
            if (!_playable.GoNextFrame())
            {
                if (_currentClip < Clips.Length - 1)
                {
                    ++_currentClip;
                    _playable.InitClip(Clips[_currentClip]);
                }
                else
                {
                    _currentClip = 0;
                    _isFinished = !_rotate.GetNextRotate();
                    _playable.InitClip(Clips[_currentClip]);
                }
            }
        }

        public void LateUpdate()
        {
            if (_isFinished) return;
            if (!Shoot) return;

            var charName = name;
            var animName = Clips[_currentClip].name;
            var index = _playable.CurrentIndex;
            var rotate = _rotate.CurrentIndex.ToString();
            Shoot.OutputShoot(charName, rotate, animName, index);
        }

        public void OnDestroy()
        {
            _playable?.Dispose();
        }
    }
}