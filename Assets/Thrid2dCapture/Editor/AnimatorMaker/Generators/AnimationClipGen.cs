using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class AnimationClipGen
    {
        private readonly GenJson _genJson;

        public AnimationClipGen(GenJson json)
        {
            _genJson = json;
        }

        public void Generate()
        {
            foreach (var actJson in _genJson.ActionJsons)
            {
                CreateAnimationClip(actJson);
            }
            AssetDatabase.Refresh();
        }

        private void CreateAnimationClip(ActionJson actJson)
        {
            var actionCount = actJson.FrameCount;

            for (var i = 0; i < actJson.AnimationClipPaths.Length; ++i)
            {
                var rotatePath = actJson.AnimationClipPaths[i];
                var animClipName = Path.GetFileName(rotatePath);

                var rotateBaseIndex = i * actionCount;
                var clip = new AnimationClip()
                {
                    name = animClipName,
                    frameRate = _genJson.Rate
                };

                var kfArray = new Keyframe[actionCount];
                for (var index = 0; index < actionCount; ++index)
                {
                    var keyFrame = new Keyframe()
                    {
                        time = index * _genJson.Rate * 0.001f,
                        value = rotateBaseIndex + index
                    };
                    kfArray[index] = keyFrame;
                }
                var curve = new AnimationCurve(kfArray);
                clip.SetCurve("", typeof(Renderer), "material._ArrayIndex", curve);

                if (IsLoopingCheck(actJson.Type))
                {
                    var settings = AnimationUtility.GetAnimationClipSettings(clip);
                    settings.loopTime = true;
                    AnimationUtility.SetAnimationClipSettings(clip, settings);
                    clip.wrapMode = WrapMode.Loop;
                }

                AssetDatabase.CreateAsset(clip, rotatePath);
            }
        }

        private bool IsLoopingCheck(ActionType type)
        {
            return type switch
            {
                ActionType.Move or ActionType.Idle => true,
                _ => false,
            };
        }
    }
}