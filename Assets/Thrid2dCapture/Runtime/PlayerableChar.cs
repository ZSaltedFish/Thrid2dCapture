using UnityEngine;

namespace com.knight.thrid2dcapture
{
    [RequireComponent(typeof(Renderer))]
    public class PlayerableChar : MonoBehaviour
    {
        public const string ASSET_INDEX_NAME = "_ArrayIndex";
        public const string BASE_TEXTURE_ARRAY_NAME = "_BaseColorArray";
        public const string MASK_TEXTURE_ARRAY_NAME = "_MaskArray";
        public const string TEAM_COLOR_NAME = "_TeamColor";
#if THRID2DCAPTURE
        public int ArrayIndex;
        public Texture2DArray BaseTextureArray;
        public Texture2DArray MaskTextureArray;
        public Color TeamColor = Color.white;
#else
        [HideInInspector] public int ArrayIndex;
        [HideInInspector] public Texture2DArray BaseTextureArray;
        [HideInHierarchy] public Texture2DArray MaskTextureArray;
        [HideInInspector] public Color TeamColor = Color.white;
#endif
        
        private MaterialPropertyBlock _propBlock;
        private Renderer _renderer;

        public void Start()
        {
            _propBlock = new ();
            _renderer = GetComponent<Renderer>();
        }

        public void LateUpdate()
        {
            if (!_renderer) return;

            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetFloat(ASSET_INDEX_NAME, ArrayIndex);
            _propBlock.SetTexture(BASE_TEXTURE_ARRAY_NAME, BaseTextureArray);
            _propBlock.SetTexture(MASK_TEXTURE_ARRAY_NAME, MaskTextureArray);
            _propBlock.SetColor(TEAM_COLOR_NAME, TeamColor);
            _renderer.SetPropertyBlock(_propBlock);
        }
    }
}