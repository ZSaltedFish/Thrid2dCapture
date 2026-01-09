using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Animations;

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
            _idleMotion.CreateStateWithoutTransition(ctrl, json);
            _moveMotion.CreateStateWithoutTransition(ctrl, json);
            _dieMotion.CreateStateWithoutTransition(ctrl, json);
            _hitMotion.CreateStateWithoutTransition(ctrl, json);

            var attackMotion = new ActionMotions(ActionType.Attack);
            attackMotion.CreateState(ctrl, json);
            _attacksMotion.Add(attackMotion);

            for (var i = (int)ActionType.SpecialAttack; i < (int)ActionType.Skill3; ++i)
            {
                var actionJson = json.ActionJsons.FirstOrDefault(t => t.Type == (ActionType)i);
                if (actionJson == null) continue;

                var motion = new ActionMotions((ActionType)i);
                motion.CreateState(ctrl, json);
                _attacksMotion.Add(motion);
            }

            _rootMachine = ctrl.layers[0].stateMachine;
            AnimatorRotate.TryCreateParam(ctrl);
        }

        public void Execute()
        {
            ExecuteIdle();
            ExecuteMove();
            ExecuteHit();
            ExecuteDie();
            ExecuteAttack();
        }

        #region Idle处理
        private void ExecuteIdle()
        {
            IdleInput();
            AttackToIdle();
        }

        private void IdleInput()
        {
            for (var i = 0; i < (int)RotateType.End; ++i)
            {
                var rotate = (RotateType)i;
                var rotateMotion = _idleMotion[rotate];
                var transition = _rootMachine.AddAnyStateTransition(rotateMotion);
                transition.canTransitionToSelf = false;
                transition.exitTime = 0;
                transition.duration = 0;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.If, 1, ActionType.Idle.ToString());
                transition.AddCondition(AnimatorConditionMode.Equals, (int)rotate, ActionMotions.ROTATE_NAME);
            }
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
            for (var i = 0; i < (int)RotateType.End; ++i)
            {
                var rotate = (RotateType)i;
                var rotateMotion = _moveMotion[rotate];
                var transition = _rootMachine.AddAnyStateTransition(rotateMotion);
                transition.canTransitionToSelf = false;
                transition.exitTime = 0;
                transition.duration = 0;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.If, 1, ActionType.Move.ToString());
                transition.AddCondition(AnimatorConditionMode.Equals, (int)rotate, ActionMotions.ROTATE_NAME);
            }
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
            for (var i = 0; i < (int)RotateType.End; ++i)
            {
                var rotate = (RotateType)i;
                var rotateMotion = _dieMotion[rotate];
                var transition = _rootMachine.AddAnyStateTransition(rotateMotion);
                transition.canTransitionToSelf = false;
                transition.exitTime = 0;
                transition.duration = 0;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.If, 1, ActionType.Die.ToString());
                transition.AddCondition(AnimatorConditionMode.Equals, (int)rotate, ActionMotions.ROTATE_NAME);
            }
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
