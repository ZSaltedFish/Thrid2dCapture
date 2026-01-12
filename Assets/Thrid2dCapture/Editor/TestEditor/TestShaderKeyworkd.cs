#if THRID2DCAPTURE
using Newtonsoft.Json;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public static class TestShaderKeyworkd
    {
        public const string RENDER_MASK_KEYWORD = "_MASKSWITCH";
        public const string TEST_RENDER_TEXTURE_PATH = "Assets/RenderTextureTest.png";
        public const string TEST_RENDER_TEXTURE_MASK_PATH = "Assets/RenderTextureTest_mask.png";

        //[MenuItem("Thrid2dCapture/Test/SwitchRenderMaskKeyword")]
        public static void SwitchRenderMaskKeyword()
        {
            var current = Shader.GetGlobalFloat(RENDER_MASK_KEYWORD);
            Debug.Log($"�л�ǰ_MASKSWITCHֵ: {current} -> {1 - current}");
            Shader.SetGlobalFloat(RENDER_MASK_KEYWORD, 1 - current);
        }

        //[MenuItem("Thrid2dCapture/Test/TryRenderTexture")]
        public static void TryRenderTexture()
        {
            if (!UnityEditor.EditorApplication.isPlaying) return;
            var mainCamera = Camera.main;
            if (!mainCamera) return;
            if (!mainCamera.TryGetComponent<ScreenShoot>(out var screenShoot)) return;
            screenShoot.TryCaptureRenderTexture(out var baseColorBytes, out var maskBytes);

            if (baseColorBytes != null)
            {
                File.WriteAllBytes(TEST_RENDER_TEXTURE_PATH, baseColorBytes);
                Debug.Log($"RenderTexture Test Output: {TEST_RENDER_TEXTURE_PATH}");
            }
            else
            {
                Debug.LogError("RenderTexture Test Failed");
            }

            var maskPath = TEST_RENDER_TEXTURE_PATH.Replace(".png", "_mask.png");
            if (maskBytes != null)
            {
                File.WriteAllBytes(TEST_RENDER_TEXTURE_MASK_PATH, maskBytes);
                Debug.Log($"RenderTexture Mask Test Output: {TEST_RENDER_TEXTURE_MASK_PATH}");
            }
            else
            {
                Debug.LogError("RenderTexture Mask Test Failed");
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Thrid2dCapture/Test/AnimationClipCreateTest")]
        public static void AnimationClipCreateTest()
        {
            var jsonPath = "Assets/Res/Samurai_door/Samurai_door_json.json";
            var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);
            var genJson = JsonConvert.DeserializeObject<GenJson>(jsonAsset.text);

            var clipGen = new AnimationClipGen(genJson);
            clipGen.Generate();
        }

        [MenuItem("Thrid2dCapture/Test/AnimatorControllerCreateTest")]
        public static void AnimatorControllerCreateTest()
        {
            var jsonPath = "Assets/Res/Samurai_door/Samurai_door_json.json";
            var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);
            var genJson = JsonConvert.DeserializeObject<GenJson>(jsonAsset.text);
            var animatorGen = new AnimatorGen(genJson);
            animatorGen.Generate();

            AssetDatabase.Refresh();
        }
    }
}
#endif