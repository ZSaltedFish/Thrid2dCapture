using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
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

            var customGen = LoadCustomGenClass(genJson);

            var textureGen = new TextureArrayGen(genJson, customGen);
            textureGen.GenAllAnimTextureArray();

            var clipGen = new AnimtionSingleClipGen(genJson, customGen);
            clipGen.Generate();

            var animatorGen = new AnimatorGen(genJson, customGen);
            animatorGen.Generate();
            AssetDatabase.Refresh();
            PlayerPrefs.Save();
        }

        private static CustomGen LoadCustomGenClass(GenJson json)
        {
            var className = json.CustomGenClassName;
            if (string.IsNullOrEmpty(className)) return null;

            var assmblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assmb in assmblies)
            {
                var list = assmb.GetTypes().Where(t => t.Name == className).ToList();
                if (list.Count == 0) continue;

                var type = list[0];
                if (type == null) continue;
                if (!typeof(CustomGen).IsAssignableFrom(type))
                {
                    Debug.LogError($"CustomGen class {className} must inherit from CustomGen");
                    return null;
                }

                var customGen = Activator.CreateInstance(type) as CustomGen;
                return customGen;
            }

            return null;
        }
    }
}
