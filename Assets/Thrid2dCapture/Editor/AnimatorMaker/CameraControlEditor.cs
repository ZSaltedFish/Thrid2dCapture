using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    [CustomEditor(typeof(CameraControl))]
    public class CameraControlEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Start"))
            {
                StartCatch();
            }

            EditorGUILayout.Space(30f);
            if (GUILayout.Button("Extesion Start"))
            {

            }
        }

        private void StartCatch()
        {
            var cameraControl = (CameraControl)target;
            if (!cameraControl) { Debug.LogError("Target is null"); return; }
            if (!cameraControl.TryGetComponent<AnimatorWatcher>(out var watcher)) { Debug.LogError("没有AnimatorWatcher"); }

            watcher.StartCatch();
        }
    }
}