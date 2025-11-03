using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public static class AnimatorRotate
    {
        public static void CreateRotateAnimator(this AnimatorController controller, RotateType type, string path)
        {

        }

        private static AnimatorStateMachine CreateSubState(this AnimatorController controller, string path, string charName, string animationName)
        {
            var name = Path.GetFileName(path);
            var subState = controller.layers[0].stateMachine.AddStateMachine(name);
            var clips = GetClips(path, $"{charName}_{animationName}");

            return subState;
        }
        
        private static AnimationClip[] GetClips(string path, string nameFilter)
        {
            var list = new List<AnimationClip>();
            var clipGUIDs = AssetDatabase.FindAssetGUIDs($"{nameFilter} t:AnimationClip", new[] { path });
            foreach (var guid in clipGUIDs)
            {
                var clip = AssetDatabase.LoadAssetByGUID<AnimationClip>(guid);
                list.Add(clip);
            }
            return list.ToArray();
        }
    }
}