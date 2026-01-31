using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    [RequireComponent(typeof(ScreenShoot))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CameraControl))]
    public class AnimatorWatcher : MonoBehaviour
    {
        public const string GEN_KEY = "Gen_key";
        public const string JSON_PATH_NAME_KEY = "JSON_PATH_NAME_KEY";

        public ActionType[] ActType;
        public AnimationClip[] Clips;
        public ScreenShoot Shoot;
        public bool NoOutputModel;

        private PlayableController _playable;
        private RotateController _rotate;
        private CameraControl _cameraControl;
        private int _currentClip = 0;
        private bool _isFinished;
        private bool _start = false;
        private bool _hasOutput = false;

        private ActionType[] _playableActionTypes;
        private AnimationClip[] _playableClips;

        public void Start()
        {
            if (Clips.Length == 0) return;
            _cameraControl = GetComponent<CameraControl>();
            Shoot = GetComponent<ScreenShoot>();
            _playable = new PlayableController(GetComponent<Animator>());
            _rotate = new RotateController(gameObject);
            _currentClip = 0;
            Shoot.Size = new Vector2(_cameraControl.Width, _cameraControl.Height);
            var actTypes = new List<ActionType>();
            var animations = new List<AnimationClip>();
            for (var i = 0; i < Clips.Length; ++i)
            {
                if (!Clips[i]) continue;
                animations.Add(Clips[i]);
                actTypes.Add(ActType[i]);
            }

            _playableClips = animations.ToArray();
            _playableActionTypes = actTypes.ToArray();

            _playable.InitClip(_playableClips[0]);
            _playable.SetToStart();
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
                if (_currentClip < _playableClips.Length - 1)
                {
                    ++_currentClip;
                    _playable.InitClip(_playableClips[_currentClip]);
                    _playable.SetToStart();
                }
                else
                {
                    _currentClip = 0;
                    _isFinished = !_rotate.GetNextRotate();
                    _playable.InitClip(_playableClips[_currentClip]);
                    _playable.SetToStart();
                }
            }
            else
            {
                _hasOutput = true;
            }

            if (NoOutputModel) return;

            if (_isFinished)
            {
                Debug.Log("AnimatorWatcher Finish Capture, To create AnimationClip");
#if UNITY_EDITOR
                var jsonObj = new JsonGen(_playableClips, _playableActionTypes, Shoot.AssetRootPath, name, _cameraControl.Width, _cameraControl.Height, 30);
                _ = jsonObj.GenerateJson();
                EditorApplication.isPlaying = false;
                AssetDatabase.Refresh();

                PlayerPrefs.SetInt(GEN_KEY, 1);
                PlayerPrefs.SetString(JSON_PATH_NAME_KEY, jsonObj.JsonPath);
                PlayerPrefs.Save();

#endif
            }
        }

        public void LateUpdate()
        {
            if (!_hasOutput) return;
            if (NoOutputModel) return;
            if (!_start) return;
            if (_isFinished) return;
            if (!Shoot) return;

            var charName = name;
            var animName = _playableClips[_currentClip].name;
            var index = _playable.CurrentIndex;
            var rotate = _rotate.CurrentIndex.ToString();

            if (index == -1)
            {
                Debug.LogError($"{charName} {animName} {index} {rotate}");
                _isFinished = false;
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
            }
            Shoot.OutputShoot(charName, rotate, animName, index);
            _hasOutput = false;
        }

        public void OnDestroy()
        {
            _playable?.Dispose();
        }
    }
}