using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    [CustomEditor(typeof(ScreenShoot))]
    public class ScreenShootEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            // using (var change = new EditorGUI.ChangeCheckScope())
            // {
            //     var serProp = serializedObject.FindProperty("TargetCamera");
            //     _ = EditorGUILayout.PropertyField(serProp, new GUIContent("目标摄像机"));

            //     using (new EditorGUILayout.HorizontalScope())
            //     {
            //         var pathProp = serializedObject.FindProperty("SavePath");
            //         _ = EditorGUILayout.PropertyField(pathProp, new GUIContent("保存路径"));

            //         if (GUILayout.Button(">>", GUILayout.Width(30)))
            //         {
            //             var path = EditorUtility.SaveFolderPanel("保存路径", "", "");
            //             if (!string.IsNullOrEmpty(path) && path.Contains("Assets"))
            //             {
            //                 pathProp.stringValue = path;
            //             }
            //         }
            //     }

            //     if (change.changed)
            //     {
            //         serializedObject.ApplyModifiedProperties();
            //         AssetDatabase.Refresh();
            //     }
            // }
        }
    }
}