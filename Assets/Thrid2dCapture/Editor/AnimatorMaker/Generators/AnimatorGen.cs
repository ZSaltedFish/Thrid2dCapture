using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class AnimatorGen
    {
        private readonly GenJson _genJson;
        public AnimatorGen(GenJson json)
        {
            _genJson = json;
        }

        public void Generate()
        {
            var ctrl = new AnimatorController();
            var path = _genJson.ControllerPath;
            AssetDatabase.CreateAsset(ctrl, path);
            ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

            var rootSM = new AnimatorStateMachine
            {
                name = "Base Layer",
                hideFlags = HideFlags.HideInHierarchy,
            };

            AssetDatabase.AddObjectToAsset(rootSM, ctrl);

            var baseLayer = new AnimatorControllerLayer
            {
                name = "Base Layer",
                defaultWeight = 1.0f,
                stateMachine = rootSM
            };

            ctrl.layers = new[] { baseLayer };
            var creator = new AnimatorMotionCreator(ctrl, _genJson);
            creator.Execute();
        }
    }
}