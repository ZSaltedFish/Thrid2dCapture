using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public static class EasyTools
    {
        public const string RENDER_KEY = "_MASKSWITCH";

        [MenuItem("Thrid2dCapture/Tools/Switch Render Key")]
        public static void SwitchRenderKey()
        {
            var key = Shader.GetGlobalFloat(RENDER_KEY);
            Shader.SetGlobalFloat(RENDER_KEY, 1 - key);
            Debug.Log($"Switch Render Key ({RENDER_KEY}) to: {1 - key}");
        }
    }
}
