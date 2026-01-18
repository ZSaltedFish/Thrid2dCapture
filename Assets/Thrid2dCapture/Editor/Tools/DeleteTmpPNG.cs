using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class DeleteTmpPNG : EditorWindow
    {
        public TextAsset JsonAsset;

        public void OnGUI()
        {
            JsonAsset = (TextAsset)EditorGUILayout.ObjectField("Json Asset", JsonAsset, typeof(TextAsset), false);
            if (GUILayout.Button("Delete Tmp PNG"))
            {
                if (JsonAsset == null)
                {
                    Debug.LogError("Please assign a Json Asset.");
                    return;
                }

                var jsonText = JsonAsset.text;
                var genJson = JsonConvert.DeserializeObject<GenJson>(jsonText);
                if (genJson == null)
                {
                    Debug.LogError("Invaild Json File");
                    return;
                }

                DeleteTmp(genJson);
            }
        }

        private void DeleteTmp(GenJson json)
        {
            var deleteFileResults = new List<string>();
            foreach (var actJson in json.ActionJsons)
            {
                foreach (var path in actJson.AnimationClipPaths)
                {
                    var dire = Path.GetDirectoryName(path);
                    var guids = AssetDatabase.FindAssets("t:Texture2D", new string[] { dire });
                    var deleteList = new string[guids.Length];
                    for (var i = 0; i < deleteList.Length; ++i)
                    {
                        var guid = guids[i];
                        var deleteFileName = AssetDatabase.GUIDToAssetPath(guid);
                        deleteList[i] = deleteFileName;
                    }
                    var deleteResults = new List<string>();
                    var result = AssetDatabase.DeleteAssets(deleteList, deleteResults);
                    if (result)
                    {
                        deleteFileResults.AddRange(deleteResults);
                    }
                }
            }

            if (deleteFileResults.Count == 0)
            {
                Debug.Log($"Delete finished: {json.BasePath}");
            }
            else
            {
                foreach (var deleteFileResult in deleteFileResults)
                {
                    Debug.LogError($"FILE DELETE FAILED: {deleteFileResult}");
                }
            }
        }

        [MenuItem("Thrid2dCapture/Tools/Delete Tmp PNG")]
        public static void ShowWindow()
        {
            GetWindow<DeleteTmpPNG>("Delete Tmp PNG");
        }
    }
}
