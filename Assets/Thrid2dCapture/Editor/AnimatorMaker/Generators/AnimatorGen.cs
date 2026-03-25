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
            AnimatorController ctrl;
            var path = _genJson.ControllerPath;
            if (!_genJson.ExtensionGen)
            {
                ctrl = new AnimatorController();
                AssetDatabase.CreateAsset(ctrl, path);
            }
            ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

            AnimatorStateMachine rootSM;
            if (!_genJson.ExtensionGen)
            {
                rootSM = new AnimatorStateMachine
                {
                    name = "Base Layer"
                };

                AssetDatabase.AddObjectToAsset(rootSM, ctrl);

                var baseLayer = new AnimatorControllerLayer
                {
                    name = "Base Layer",
                    defaultWeight = 1.0f,
                    stateMachine = rootSM
                };

                ctrl.layers = new[] { baseLayer };
            }
            else
            {
                rootSM = AssetDatabase.LoadAssetAtPath<AnimatorStateMachine>(path);
            }
            var creator = new SingleAnimatorMotionCreator(ctrl, _genJson);
            creator.Execute(rootSM);
            if (!_genJson.ExtensionGen)
            {
                creator.AutoConnect(rootSM);
            }

            if (_genJson.ExtensionGen)
            {
                EditorUtility.SetDirty(rootSM);
                EditorUtility.SetDirty(ctrl);
                AssetDatabase.SaveAssets();
            }
        }
    }
}