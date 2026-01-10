#if THRID2DCAPTURE
using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public static class TestShaderKeyworkd
    {
        public const string RENDER_MASK_KEYWORD = "_MASKSWITCH";

        [MenuItem("Thrid2dCapture/Test/SwitchRenderMaskKeyword")]
        public static void SwitchRenderMaskKeyword()
        {
            var current = Shader.GetGlobalFloat(RENDER_MASK_KEYWORD);
            Debug.Log($"ÇÐ»»Ç°_MASKSWITCHÖµ: {current} -> {1 - current}");
            Shader.SetGlobalFloat(RENDER_MASK_KEYWORD, 1 - current);
        }
    }
}
#endif