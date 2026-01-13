using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class BlockPropState : StateMachineBehaviour
    {
        public const string BaseColorArrayProp = "_BaseColorArray";
        public const string MaskArrayProp = "_MaskArray";
        public Texture2DArray BaseColorArray;
        public Texture2DArray MaskArray;

        private MaterialPropertyBlock _propBlock;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_propBlock == null)
            {
                _propBlock = new MaterialPropertyBlock();
            }

            var renderer = animator.GetComponent<Renderer>();
            renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetTexture(BaseColorArrayProp, BaseColorArray);
            _propBlock.SetTexture(MaskArrayProp, MaskArray);
            renderer.SetPropertyBlock(_propBlock);
        }
    }
}