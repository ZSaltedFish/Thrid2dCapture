using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using UnityEditor.Graphs;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    [RequireComponent(typeof(ScreenShoot))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CameraControl))]
    public class AnimatorWatcher : MonoBehaviour
    {
        public ActionType[] ActType;
        public AnimationClip[] Clips;
        public ScreenShoot Shoot;
        public bool NoOutputModel;

        private GenJson _genJson;
        private PlayableController _playable;
        private RotateController _rotate;
        private int _currentClip = 0;
        private bool _isFinished;
        private bool _start = false;
        private bool _hasOutput = false;

        private ActionType[] _playableActionTypes;
        private AnimationClip[] _playableClips;

        public void Start()
        {
            if (Clips.Length == 0) return;
            Shoot = GetComponent<ScreenShoot>();
            _playable = new PlayableController(GetComponent<Animator>());
            _rotate = new RotateController(gameObject);
            _currentClip = 0;

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
                var jsonPath = CreateJsonFile();
                CreateAnimationClip();

#if UNITY_EDITOR
                var controller = new AnimatorController();
                var path = Path.Combine(Shoot.AssetRootPath, $"{name}.controller");
                AssetDatabase.CreateAsset(controller, path);
                controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

                var rootSM = new AnimatorStateMachine
                {
                    name = "Base Layer",
                    hideFlags = HideFlags.HideInHierarchy
                };
                AssetDatabase.AddObjectToAsset(rootSM, controller);

                var baseLayer = new AnimatorControllerLayer
                {
                    name = "Base Layer",
                    defaultWeight = 1.0f,
                    stateMachine = rootSM
                };
                controller.layers = new[] { baseLayer };
                AnimatorRotate.CreateRotateAnimator(controller, jsonPath);

                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(path);
                AssetDatabase.Refresh();

#endif
            }
        }

        private string CreateJsonFile()
        {
            var genJson = new GenJson()
            {
                ActionJsons = new ActionJson[_playableClips.Length]
            };

            var rotateTypes = (Enum.GetValues(typeof(RotateType)) as RotateType[]).ToList();
            rotateTypes.Remove(RotateType.End);
            for (var i = 0; i < _playableClips.Length; ++i)
            {
                var actType = _playableActionTypes[i];
                var clip = _playableClips[i];

                if (!clip) continue;
                var actionJson = new ActionJson()
                {
                    Type = actType
                };

                var rotateActionsList = new List<RotateActionJson>();
                foreach (var rotateType in rotateTypes)
                {
                    var rotateActionJson = new RotateActionJson()
                    {
                        RotateType = rotateType,
                        Path = $"{Shoot.AssetRootPath}/{name}_{clip.name}_{rotateType}/{name}_{clip.name}_{rotateType}.anim"
                    };
                    rotateActionsList.Add(rotateActionJson);
                }
                actionJson.RotateActions = rotateActionsList.ToArray();
                genJson.ActionJsons[i] = actionJson;
            }

            var serObj = JsonConvert.SerializeObject(genJson, Formatting.Indented);
            var jsonPath = Path.Combine(Shoot.SavePath, "ReadJson.json");
            File.WriteAllText(jsonPath, serObj);
            _genJson = genJson;
            return jsonPath;
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

        private void CreateAnimationClip()
        {
            var savePath = Shoot.SavePath;
            savePath = savePath[savePath.IndexOf("Assets")..];
#if UNITY_EDITOR
            AssetDatabase.Refresh();
            EditorApplication.isPlaying = false;
#endif
            foreach (var clip in _playableClips)
            {
                var subDires = Directory.GetDirectories(savePath, $"{name}_{clip.name}_*", SearchOption.TopDirectoryOnly);
                foreach (var imagePath in subDires)
                {
                    var clipName = $"{Path.GetFileName(imagePath)}.anim";
                    var clipPath = Path.Combine(imagePath, clipName);
                    SpriteSetting.SetPathTextue2Sprite(imagePath);
                    var isLoop = IsLoopingCheck(imagePath);
                    AnimationMaker.CreateAndSaveClip(clipPath, imagePath, isLoop);
                }
            }

#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        private bool IsLoopingCheck(string path)
        {
            var rePath = path.Replace('\\', '/').ToLower()[..path.LastIndexOf('_')];
            foreach (var action in _genJson.ActionJsons)
            {
                if (action.RotateActions == null || action.RotateActions.Length == 0) continue;
                if (!action.RotateActions[0].Path.Replace('\\', '/').ToLower().StartsWith(rePath)) continue;
                var actionType = action.Type;

                return actionType switch
                {
                    ActionType.Move or ActionType.Idle => true,
                    _ => false,
                };
            }

            throw new NotImplementedException($"No match with \"{rePath}\"");
        }
    }
}