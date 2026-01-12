using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class ActionMotions
    {
        public const string ROTATE_NAME = "RotateParam";
        public ActionType ActionType { get; private set; }
        public AnimatorState this[RotateType type] => _rotate2State[type];
        public AnimatorStateMachine ActionStateMachine => _actionState;

        private ActionJson _actJson;
        private AnimatorStateMachine _actionState;
        private readonly Dictionary<RotateType, AnimatorState> _rotate2State = new();
        private string _baseColorPath;
        private string _maskPath;

        public ActionMotions(ActionType type, GenJson json)
        {
            ActionType = type;
            _actJson = json.ActionJsons.First(t => t.Type == ActionType);
            _baseColorPath = _actJson.BaseColorTextureArrayPath;
            _maskPath = _actJson.MaskTextureArrayPath;
        }

        public void CreateState(AnimatorController controller)
        {
            var mainMachine = controller.layers[0].stateMachine;

            var x = (int)ActionType * 100f;
            var y = (int)ActionType * 100f;
            _actionState = mainMachine.AddStateMachine(ActionType.ToString(), new Vector3(x, y, 0));
            AddBehaviour();

            var actionJson = _actJson;
            if (actionJson == null)
            {
                Debug.Log($"No Action {ActionType} in Json file");
                return;
            }

            for (var i = 0; i < actionJson.AnimationClipPaths.Length; ++i)
            {
                var clipPath = actionJson.AnimationClipPaths[i];
                var rotateType = (RotateType)i;
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
                if (!clip)
                {
                    Debug.LogError($"No Rotate {clipPath}");
                    continue;
                }

                var state = _actionState.AddState($"{clip.name}_state", GetPosition(rotateType));
                state.motion = clip;
                _rotate2State.Add(rotateType, state);

                var transition = _actionState.AddEntryTransition(state);
                transition.AddCondition(AnimatorConditionMode.Equals, i, ROTATE_NAME);
            }
        }

        public void CreateStateWithoutTransition(AnimatorController controller)
        {
            var mainMachine = controller.layers[0].stateMachine;

            var x = (int)ActionType * 100f;
            var y = (int)ActionType * 100f;
            _actionState = mainMachine.AddStateMachine(ActionType.ToString(), new Vector3(x, y, 0));
            AddBehaviour();

            var actionJson = _actJson;
            if (actionJson == null)
            {
                Debug.Log($"No Action {ActionType} in Json file");
                return;
            }

            for (var i = 0; i < actionJson.AnimationClipPaths.Length; ++i)
            {
                var clipPath = actionJson.AnimationClipPaths[i];
                var rotateType = (RotateType)i;
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
                if (!clip)
                {
                    Debug.LogError($"No Rotate {clipPath}");
                    continue;
                }

                var state = _actionState.AddState($"{clip.name}_state", GetPosition(rotateType));
                state.motion = clip;
                _rotate2State.Add(rotateType, state);
            }
        }

        private static Vector3 GetPosition(RotateType rotateType)
        {
            var positionIndex = (int)rotateType;
            var rta = Mathf.PI * .5f;
            var step = Mathf.PI * .125f;
            var dist = 500f;

            var nRta = rta - positionIndex * step;
            var x = Mathf.Cos(nRta) * dist;
            var y = Mathf.Sin(nRta) * dist;

            return new Vector3(x, y, 0);
        }

        private void AddBehaviour()
        {
            var blockPropState = _actionState.AddStateMachineBehaviour<BlockPropState>();
            blockPropState.BaseColorArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(_baseColorPath);
            blockPropState.MaskArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(_maskPath);
        }
    }
}
