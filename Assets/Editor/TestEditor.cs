using com.knight.thrid2dcapture;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets
{
    public class CustomGenTest : CustomGen
    {
        public override void RunAnimatorGen(SingleAnimatorMotionCreator gen, AnimatorStateMachine rootSM, AnimatorController controller)
        {
            controller.AddParameter("Dead_Trigger", AnimatorControllerParameterType.Trigger);

            var deadState = gen.StateDict[ActionType.Dead];
            rootSM.AddAnyStateTransition(deadState).AddCondition(AnimatorConditionMode.If, 0, "Dead_Trigger");
        }
    }
}