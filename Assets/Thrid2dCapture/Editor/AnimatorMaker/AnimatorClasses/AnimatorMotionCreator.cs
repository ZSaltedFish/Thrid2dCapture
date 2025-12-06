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
    }
}
