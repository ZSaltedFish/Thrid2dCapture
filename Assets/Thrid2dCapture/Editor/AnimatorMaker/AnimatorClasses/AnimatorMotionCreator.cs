using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Animations;
using UnityEditor.Overlays;

namespace com.knight.thrid2dcapture
{
    public class AnimatorMotionCreator
    {
        private ActionMotions _idleMotion = new(ActionType.Idle);
        private ActionMotions _moveMotion = new(ActionType.Move);
        private ActionMotions _dieMotion = new(ActionType.Die);
        private ActionMotions _hitMotion = new(ActionType.Hit);
        private List<ActionMotions> _attacksMotion = new();

        private AnimatorStateMachine _rootMachine;

        public AnimatorMotionCreator(AnimatorController ctrl, string jsonPath)
        {
            var json = JsonConvert.DeserializeObject<GenJson>(File.ReadAllText(jsonPath));
            _idleMotion.CreateState(ctrl, json);
            _moveMotion.CreateState(ctrl, json);
            _dieMotion.CreateState(ctrl, json);
            _hitMotion.CreateState(ctrl, json);

            var attackMotion = new ActionMotions(ActionType.Attack);
            attackMotion.CreateState(ctrl, json);
            _attacksMotion.Add(attackMotion);

            for (var i = (int)ActionType.SpecialAttack; i < (int)ActionType.Skill3; ++i)
            {
                var actionJson = json.ActionJsons.First(t => t.Type == (ActionType)i);
                if (actionJson == null) continue;

                var motion = new ActionMotions((ActionType)i);
                motion.CreateState(ctrl, json);
                _attacksMotion.Add(motion);
            }

            _rootMachine = ctrl.layers[0].stateMachine;
        }

        #region Idle处理
        private void ExecuteIdle()
        {
            IdleInput();
            AttackToIdle();
        }

        private void IdleInput()
        {
            var input = _rootMachine.AddAnyStateTransition(_idleMotion.ActionStateMachine);
            input.AddCondition(AnimatorConditionMode.If, 1, ActionType.Idle.ToString());
            input.canTransitionToSelf = false;
            input.hasFixedDuration = false;
            input.duration = 0;
        }

        private void AttackToIdle()
        {
            foreach (var action in _attacksMotion)
            {
                for (var i = 0; i < (int)RotateType.End; ++i)
                {
                    var rotate = (RotateType)i;
                    var state = action[rotate];

                    var transition = state.AddTransition(_idleMotion.ActionStateMachine);
                    transition.hasExitTime = true;
                    transition.exitTime = 0;
                    transition.duration = 0;
                    transition.hasFixedDuration = false;
                }
            }
        }
        #endregion

        #region Move处理
        private void ExecuteMove()
        {
            MoveInput();
            AttackToMove();
        }

        private void MoveInput()
        {
            var input = _rootMachine.AddAnyStateTransition(_moveMotion.ActionStateMachine);
            input.AddCondition(AnimatorConditionMode.If, 1, ActionType.Move.ToString());
            input.canTransitionToSelf = false;
            input.hasFixedDuration = false;
            input.duration = 0;
        }

        private void AttackToMove()
        {
            foreach (var action in _attacksMotion)
            {
                for (var i = 0; i < (int)RotateType.End; ++i)
                {
                    var rotate = (RotateType)i;
                    var state = action[rotate];

                    var transition = state.AddTransition(_moveMotion.ActionStateMachine);
                    transition.hasExitTime = true;
                    transition.exitTime = 0;
                    transition.duration = 0;
                    transition.hasFixedDuration = false;
                }
            }
        }
        #endregion

        #region Die处理
        private void ExecuteDie()
        {
            DieInput();
        }

        private void DieInput()
        {
            var input = _rootMachine.AddAnyStateTransition(_dieMotion.ActionStateMachine);
            input.AddCondition(AnimatorConditionMode.If, 1, ActionType.Die.ToString());
            input.canTransitionToSelf = false;
            input.hasFixedDuration = false;
            input.duration = 0;
        }
        #endregion

        #region Hit处理
        private void ExecuteHit()
        {
            HitInput();
        }

        private void HitInput()
        {
            //throw new NotImplementedException();
            // TO-DO: Hit逻辑待补充
        }
        #endregion

        #region Attack处理
        private void ExecuteAttack()
        {
            AttackInput();
        }

        private void AttackInput()
        {
            foreach (var action in _attacksMotion)
            {
                FromIdleInput(action);
                FromMoveInput(action);
            }

            for (var i = 0; i < _attacksMotion.Count; ++i)
            {
                for (var j = 0; j < _attacksMotion.Count; ++j)
                {
                    if (i == j) continue;

                    var fromAction = _attacksMotion[i];
                    var toAction = _attacksMotion[j];

                    for (var k = 0; k < (int)RotateType.End; ++k)
                    {
                        var rotate = (RotateType)k;
                        var fromState = fromAction[rotate];
                        var toState = toAction[rotate];

                        var transition = fromState.AddTransition(toState);
                        transition.AddCondition(AnimatorConditionMode.If, 1, toAction.ActionType.ToString());
                        transition.hasExitTime = false;
                        transition.duration = 0;
                        transition.hasFixedDuration = false;
                    }
                }
            }
        }

        private void FromIdleInput(ActionMotions action)
        {
            for (var i = 0; i < (int)RotateType.End; ++i)
            {
                var rotate = (RotateType)i;
                var state = action[rotate];

                var transition = _idleMotion[rotate].AddTransition(state);
                transition.AddCondition(AnimatorConditionMode.If, 1, action.ActionType.ToString());
                transition.hasExitTime = false;
                transition.duration = 0;
                transition.hasFixedDuration = false;
            }
        }

        private void FromMoveInput(ActionMotions action)
        {
            for (var i = 0; i < (int)RotateType.End; ++i)
            {
                var rotate = (RotateType)i;
                var state = action[rotate];

                var transition = _moveMotion[rotate].AddTransition(state);
                transition.AddCondition(AnimatorConditionMode.If, 1, action.ActionType.ToString());
                transition.hasExitTime = false;
                transition.duration = 0;
                transition.hasFixedDuration = false;
            }
        }
        #endregion
    }
}
