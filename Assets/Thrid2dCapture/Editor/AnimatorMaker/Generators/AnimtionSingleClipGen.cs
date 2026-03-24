using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class AnimtionSingleClipGen
    {
        private readonly GenJson _genJson;

        public AnimtionSingleClipGen(GenJson json)
        {
            _genJson = json;
        }

        public void Generate()
        {
            foreach (var action in _genJson.SingleActionJsons)
            {
                CreateAnimationClip(action);
            }
            AssetDatabase.Refresh();
        }

        private void CreateAnimationClip(SingleActionJson action)
        {
            var actionCount = action.FrameCount;
            var animClipName = action.AnimName;
            var animPath = action.AnimationClipPath;

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
                    value = index
                };
                kfArray[index] = keyFrame;
            }

            var curve = new AnimationCurve(kfArray);
            clip.SetCurve("", typeof(Renderer), "material._ArrayIndex", curve);

            if (IsLoopingCheck(action.Type))
            {
                var settings = AnimationUtility.GetAnimationClipSettings(clip);
                settings.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(clip, settings);
                clip.wrapMode = WrapMode.Loop;
            }

            AssetDatabase.CreateAsset(clip, animPath);
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
