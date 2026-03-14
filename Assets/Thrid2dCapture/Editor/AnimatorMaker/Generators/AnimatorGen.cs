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

            if (!_genJson.ExtensionGen)
            {
                var rootSM = new AnimatorStateMachine
                {
                    name = "Base Layer",
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
            var creator = new AnimatorMotionCreator(ctrl, _genJson);
            if (!_genJson.ExtensionGen) 
            {
                creator.Execute();
            }
            else
            {
                var rootSM = ctrl.layers[0].stateMachine;
                EditorUtility.SetDirty(rootSM);
                EditorUtility.SetDirty(ctrl);
                AssetDatabase.SaveAssets();
            }
        }
    }
}