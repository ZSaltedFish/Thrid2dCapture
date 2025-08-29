using UnityEngine;

namespace com.knight.thrid2dcapture
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorWatcher : MonoBehaviour
    {
        public AnimationClip[] Clips;

        private PlayableController _playable;
        private RotateController _rotate;
        private int _currentClip = 0;
        private bool _isFinished;

        public void Start()
        {
            if (Clips.Length == 0) return;
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

        public void OnDestroy()
        {
            _playable?.Dispose();
        }
    }
}