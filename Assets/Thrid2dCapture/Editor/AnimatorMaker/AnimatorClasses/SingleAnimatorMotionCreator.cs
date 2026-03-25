using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.knight.thrid2dcapture
{
    public class SingleAnimatorMotionCreator
    {
        private GenJson _json;
        private Dictionary<ActionType, AnimatorState> _stateDict = new();
        private ActionType[] _attackTypes = { ActionType.Attack, ActionType.SpecialAttack, ActionType.Skill1, ActionType.Skill2, ActionType.Skill3 };

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
            _stateDict[json.Type] = state;
        }

        public void Execute(AnimatorStateMachine machine)
        {
            foreach (var singleAnimator in _json.SingleActionJsons)
            {
                CreateState(machine, singleAnimator);
            }
        }

        public void AutoConnect(AnimatorStateMachine mainMachine)
        {
            if (_stateDict.Count == 0)
            {
                Debug.LogWarning($"No State, Ignore Connect");
                return;
            }

            IdleConnect(mainMachine);
            MoveConnect(mainMachine);
            DeadConnect(mainMachine);
            AttackConnect(mainMachine);
        }

        private void IdleConnect(AnimatorStateMachine mainMachine)
        {
            if (!_stateDict.TryGetValue(ActionType.Idle, out var idleState)) return;
            var attackList = _attackTypes;

            var anyTran = mainMachine.AddAnyStateTransition(idleState);
            anyTran.canTransitionToSelf = false;
            anyTran.exitTime = 0;
            anyTran.duration = 0;
            anyTran.hasFixedDuration = false;
            anyTran.AddCondition(AnimatorConditionMode.If, 1, ActionType.Idle.ToString());

            foreach (var attack in attackList)
            {
                if (!_stateDict.TryGetValue(attack, out var state)) continue;
                var transition = state.AddTransition(idleState);
                transition.hasExitTime = true;
                transition.exitTime = 0;
                transition.duration = 0;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.If, 1, ActionType.Idle.ToString());
            }
        }

        private void MoveConnect(AnimatorStateMachine mainMachine)
        {
            if (!_stateDict.TryGetValue(ActionType.Move, out var moveState)) return;
            var attackList = _attackTypes;

            var anyTran = mainMachine.AddAnyStateTransition(moveState);
            anyTran.canTransitionToSelf = false;
            anyTran.exitTime = 0;
            anyTran.duration = 0;
            anyTran.hasFixedDuration = false;
            anyTran.AddCondition(AnimatorConditionMode.If, 1, ActionType.Move.ToString());

            foreach (var attack in attackList)
            {
                if (!_stateDict.TryGetValue(attack, out var state)) continue;
                var transition = state.AddTransition(moveState);
                transition.hasExitTime = true;
                transition.exitTime = 0;
                transition.duration = 0;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.If, 1, ActionType.Move.ToString());
            }
        }

        private void DeadConnect(AnimatorStateMachine mainMachine)
        {
            if (!_stateDict.TryGetValue(ActionType.Dead, out var deadState)) return;
            var attackList = _attackTypes;

            var anyTran = mainMachine.AddAnyStateTransition(deadState);
            anyTran.canTransitionToSelf = false;
            anyTran.exitTime = 0;
            anyTran.duration = 0;
            anyTran.hasFixedDuration = false;
            anyTran.AddCondition(AnimatorConditionMode.If, 1, ActionType.Dead.ToString());
        }

        private void AttackConnect(AnimatorStateMachine mainMachine)
        {
            var attackList = _attackTypes;
            if (!_stateDict.TryGetValue(ActionType.Idle, out var idleState))
            {
                Debug.LogError("No Idle. Ignore");
                return;
            }

            if (!_stateDict.TryGetValue(ActionType.Move, out var moveState))
            {
                Debug.LogError("No Move. Ignore");
                return;
            }

            foreach (var attack in attackList)
            {
                if (!_stateDict.TryGetValue(attack, out var curState)) continue;
                var idleTrans = idleState.AddTransition(curState);
                idleTrans.AddCondition(AnimatorConditionMode.If, 1, attack.ToString());
                idleTrans.hasExitTime = false;
                idleTrans.duration = 0;
                idleTrans.hasFixedDuration = false;

                var moveTrans = moveState.AddTransition(curState);
                moveTrans.AddCondition(AnimatorConditionMode.If, 1, attack.ToString());
                moveTrans.hasExitTime = false;
                moveTrans.duration = 0;
                moveTrans.hasFixedDuration = false;

                foreach (var subAttack in attackList)
                {
                    if (subAttack == attack) continue;
                    if (!_stateDict.TryGetValue(subAttack, out var subState)) continue;

                    var subTran = curState.AddTransition(subState);
                    subTran.AddCondition(AnimatorConditionMode.If, 1, subAttack.ToString());
                    subTran.hasExitTime = false;
                    subTran.duration = 0;
                    subTran.hasFixedDuration = false;
                }
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
