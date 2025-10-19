using System.Linq;
using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class AnimationMaker : EditorWindow
    {
        [MenuItem("Tools/Thrid2dCapture/AnimationMaker")]
        public static void ShowWindow()
        {
            var window = GetWindow<AnimationMaker>();
            window.titleContent = new GUIContent("AnimationMaker");
            window.Show();
        }

        public void OnGUI()
        {

        }

        #region Load PNG

        public static void LoadTexture2D(AnimationClip clip, string srcPath)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(srcPath);
            var index = 0;
            var curve = new AnimationCurve();
            var fps = clip.frameRate;
            
            foreach (Texture2D tex in assets.Cast<Texture2D>())
            {
                if (!tex) continue;

                
            }
        }
        #endregion
    }
}