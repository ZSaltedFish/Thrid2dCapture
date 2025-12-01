using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class AnimationMaker : EditorWindow
    {
        private string _clipPath;
        private string _imagePath;

        [MenuItem("Tools/Thrid2dCapture/AnimationMaker")]
        public static void ShowWindow()
        {
            var window = GetWindow<AnimationMaker>();
            window.titleContent = new GUIContent("AnimationMaker");
            window.Show();
        }

        public void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                _clipPath = EditorGUILayout.TextField("Clip Path", _clipPath);
                if (GUILayout.Button(">>", GUILayout.Width(30)))
                {
                    var clipPath = EditorUtility.SaveFilePanelInProject("Clip File", "Clip", "anim", "Save Clip");
                    if (!string.IsNullOrEmpty(clipPath)) _clipPath = clipPath;
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                _imagePath = EditorGUILayout.TextField("Image Path", _imagePath);
                if (GUILayout.Button(">>", GUILayout.Width(30)))
                {
                    var path = EditorUtility.OpenFolderPanel("Path Select", "", "");
                    if (!string.IsNullOrEmpty(path)) _imagePath = path;
                    if (!path.Contains("Assets"))
                    {
                        _imagePath = "";
                    }
                    else
                    {
                        _imagePath = _imagePath.Substring(_imagePath.LastIndexOf("Assets"));
                    }
                }
            }

            if (GUILayout.Button("Create"))
            {
                CreateClip();
            }
        }

        private void CreateClip()
        {
            if (string.IsNullOrEmpty(_clipPath))
            {
                Debug.LogError("û���趨Clip�ļ���");
                return;
            }

            if (string.IsNullOrEmpty(_imagePath))
            {
                Debug.LogError("û���趨ͼƬ·��");
                return;
            }

            DividPathAndName(_clipPath, out var name, out var path);
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("��ȡNameʧ��");
                return;
            }

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("��ȡPathʧ��");
                return;
            }

            var clip = CreateAnimationClip(_imagePath, name);
            SaveClip(_clipPath, clip);
        }

        #region Static public method to create clip
        public static void CreateAndSaveClip(string clipPath, string imagePath, bool isLooping)
        {
            DividPathAndName(clipPath, out var name, out var path);
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("��ȡNameʧ��");
                return;
            }

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("��ȡPathʧ��");
                return;
            }

            var clip = CreateAnimationClip(imagePath, name);
            if (isLooping)
            {
                var settings = AnimationUtility.GetAnimationClipSettings(clip);
                settings.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(clip, settings);
                clip.wrapMode = WrapMode.Loop;
            }
            SaveClip(clipPath, clip);
        }
        #endregion

        #region Load PNG

        private static void SaveClip(string clipPath, AnimationClip clip)
        {
            AssetDatabase.CreateAsset(clip, clipPath);
        }

        private static AnimationClip CreateAnimationClip(string imagePath, string clipName, float rate = 30)
        {
            var clip = new AnimationClip()
            {
                name = clipName,
                frameRate = rate
            };

            LoadTexture2D(clip, imagePath);
            return clip;
        }

        private static void LoadTexture2D(AnimationClip clip, string srcPath)
        {
            var assets = LoadAllAssets(srcPath);
            var index = 0;
            var fps = clip.frameRate;
            Debug.Log($"准备生成{clip} rate:{fps} length:{clip.length}");

            var list = new List<ObjectReferenceKeyframe>();
            
            foreach (Sprite tex in assets.Cast<Sprite>())
            {
                if (!tex) continue;

                var keyFrame = new ObjectReferenceKeyframe()
                {
                    time = index * fps / 1000f,
                    value = tex
                };
                ++index;

                list.Add(keyFrame);
            }

            AnimationUtility.SetObjectReferenceCurve(clip, EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"), list.ToArray());
        }
        #endregion

        #region Path Method

        private static void DividPathAndName(string file, out string name, out string path)
        {
            path = Path.GetDirectoryName(file);
            name = Path.GetFileName(file);
        }

        private static Sprite[] LoadAllAssets(string path)
        {
            var guids = AssetDatabase.FindAssets("t:Sprite", new[] {path} );
            var sprites = new List<Sprite>();

            foreach (var guid in guids)
            {
                var guidPath = AssetDatabase.GUIDToAssetPath(guid);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(guidPath);
                sprites.Add(sprite);
            }
            return sprites.ToArray();
        }
        #endregion
    }
}