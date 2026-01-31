using Newtonsoft.Json;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    [InitializeOnLoad]
    public class EditorRun
    {
        static EditorRun()
        {
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }

        private static void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
        {
            if (obj != PlayModeStateChange.EnteredEditMode) return;
            if (PlayerPrefs.GetInt(AnimatorWatcher.GEN_KEY, 0) == 0) return;
            PlayerPrefs.SetInt(AnimatorWatcher.GEN_KEY, 0);

            var jsonPath = PlayerPrefs.GetString(AnimatorWatcher.JSON_PATH_NAME_KEY, "");
            if (string.IsNullOrEmpty(jsonPath)) return;
            Debug.Log($"Create from json: {jsonPath}");
            var jsonText = File.ReadAllText(jsonPath);
            var genJson = JsonConvert.DeserializeObject<GenJson>(jsonText);
            var textureGen = new TextureArrayGen(genJson);
            textureGen.GenAllAnimTextureArray();

            var clipGen = new AnimationClipGen(genJson);
            clipGen.Generate();

            var animatorGen = new AnimatorGen(genJson);
            animatorGen.Generate();
            AssetDatabase.Refresh();
            PlayerPrefs.Save();
        }
    }
}
