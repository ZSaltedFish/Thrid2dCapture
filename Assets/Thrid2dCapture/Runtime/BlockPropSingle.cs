using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class BlockPropSingle : StateMachineBehaviour
    {
        public const string BaseColorArrayProp = "_BaseColorArray";
        public const string MaskArrayProp = "_MaskArray";
        public const string AnimCountProp = "_AnimationCount";

        public Texture2DArray ColorArray;
        public Texture2DArray MaskArray;
        public int AnimCount;

        private MaterialPropertyBlock _propBlock;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _propBlock ??= new MaterialPropertyBlock();

            var renderer = animator.GetComponent<Renderer>();
            renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetTexture(BaseColorArrayProp, ColorArray);
            _propBlock.SetTexture(MaskArrayProp, MaskArray);
            _propBlock.SetFloat(AnimCountProp, AnimCount);
            renderer.SetPropertyBlock(_propBlock);
        }
    }
}