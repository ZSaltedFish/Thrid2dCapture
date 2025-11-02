using System.IO;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    [RequireComponent(typeof(ScreenShoot))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CameraControl))]
    public class AnimatorWatcher : MonoBehaviour
    {
        public AnimationClip[] Clips;
        public ScreenShoot Shoot;

        private PlayableController _playable;
        private RotateController _rotate;
        private int _currentClip = 0;
        private bool _isFinished;
        private bool _start = false;

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

        public void StartCatch()
        {
            _start = true;
        }

        public void Update()
        {
            if (!_start) return;
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
                    _ = _playable.GoNextFrame();
                }
            }

            if (_isFinished)
            {
                Debug.Log("AnimatorWatcher Finish Capture, To create AnimationClip");
                CreateAnimationClip();
            }
        }

        public void LateUpdate()
        {
            if (!_start) return;
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

        private void CreateAnimationClip()
        {
            var savePath = Shoot.SavePath;
            savePath = savePath[savePath.IndexOf("Assets")..];
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            foreach (var clip in Clips)
            {
                var subDires = Directory.GetDirectories(savePath, $"{name}_{clip.name}_*", SearchOption.TopDirectoryOnly);
                Debug.Log($"Now create {clip}");
                foreach (var imagePath in subDires)
                {
                    var clipName = $"{Path.GetFileName(imagePath)}.anim";
                    var clipPath = Path.Combine(imagePath, clipName);
                    SpriteSetting.SetPathTextue2Sprite(imagePath);
                    AnimationMaker.CreateAndSaveClip(clipPath, imagePath);
                }
            }
        }
    }
}