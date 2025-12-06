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

        private AnimatorStateMachine _actionState;
        private readonly Dictionary<RotateType, AnimatorState> _rotate2State = new();

        public ActionMotions(ActionType type)
        {
            ActionType = type;
        }

        public void CreateState(AnimatorController controller, GenJson json)
        {
            var mainMachine = controller.layers[0].stateMachine;

            var x = (int)ActionType * 100f;
            var y = (int)ActionType * 100f;
            _actionState = mainMachine.AddStateMachine(ActionType.ToString(), new Vector3(x, y, 0));

            var actionJson = json.ActionJsons.First(t => t.Type == ActionType);
            if (actionJson == null)
            {
                Debug.Log($"No Action {ActionType} in Json file");
                return;
            }

            foreach (var rotateAction in actionJson.RotateActions)
            {
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(rotateAction.Path);
                if (!clip)
                {
                    Debug.LogError($"No Rotate {rotateAction.Path}");
                    continue;
                }
                var state = _actionState.AddState($"{clip.name}_state", GetPosition(rotateAction.RotateType));
                state.motion = clip;
                _rotate2State.Add(rotateAction.RotateType, state);

                var transition = _actionState.AddEntryTransition(state);
                transition.AddCondition(AnimatorConditionMode.Equals, (int)rotateAction.RotateType, ROTATE_NAME);
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
    }
}
