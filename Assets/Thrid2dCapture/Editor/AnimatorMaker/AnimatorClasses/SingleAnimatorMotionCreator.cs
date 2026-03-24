using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class SingleAnimatorMotionCreator
    {
        private GenJson _json;

        public SingleAnimatorMotionCreator(AnimatorController ctrl, GenJson json)
        {
            if (!json.ExtensionGen)
            {
                AnimatorRotate.TryCreateParam(ctrl);
            }
            _json = json;
        }

        private void CreateState(AnimatorStateMachine mainMachine, SingleActionJson json)
        {
            var x = 100 * (int)json.Type;
            var y = 100 * (int)json.Type;
            var clipPath = json.AnimationClipPath;
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            if (!clip)
            {
                Debug.LogError($"No Clip({clipPath})");
                return;
            }

            var state = mainMachine.AddState($"{clip.name}_state", new Vector3(x, y, 0));
            state.motion = clip;

            AddBehaviour(state, json);
        }

        public void Execute(AnimatorStateMachine machine)
        {
            foreach (var singleAnimator in _json.SingleActionJsons)
            {
                CreateState(machine, singleAnimator);
            }
        }

        private void AddBehaviour(AnimatorState state, SingleActionJson json)
        {
            var blockPropSingle = state.AddStateMachineBehaviour<BlockPropSingle>();
            blockPropSingle.ColorArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(json.BaseColorTextureArrayPath);
            blockPropSingle.MaskArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(json.MaskTextureArrayPath);
            blockPropSingle.AnimCount = json.FrameCount;
        }
    }
}
