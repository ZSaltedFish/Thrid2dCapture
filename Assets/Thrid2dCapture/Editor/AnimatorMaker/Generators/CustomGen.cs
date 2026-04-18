using UnityEditor.Animations;

namespace com.knight.thrid2dcapture
{
    public abstract class CustomGen
    {
        public virtual void RunAnimatorGen(SingleAnimatorMotionCreator gen, AnimatorStateMachine rootSM, AnimatorController controller) { }
    }
}