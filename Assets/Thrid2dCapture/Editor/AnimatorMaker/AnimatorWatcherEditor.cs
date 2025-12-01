using System;
using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    [CustomEditor(typeof(AnimatorWatcher))]
    public class AnimatorWatcherEditor : Editor
    {
        public static int ACTION_COUNT = Enum.GetValues(typeof(ActionType)).Length;
        public static ActionType[] ACTION_TYPES = Enum.GetValues(typeof(ActionType)) as ActionType[];

        public override void OnInspectorGUI()
        {
            using (var changed = new EditorGUI.ChangeCheckScope())
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AnimatorWatcher.NoOutputModel)));
                DrawActionClips();
                if (changed.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        #region Action Clips
        private void DrawActionClips()
        {
            CheckAndSetList();
            var actTypeField = serializedObject.FindProperty(nameof(AnimatorWatcher.ActType));
            var animationClipField = serializedObject.FindProperty(nameof(AnimatorWatcher.Clips));

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                var index = 0;
                foreach (var action in ACTION_TYPES)
                {
                    var actTypeF = actTypeField.GetArrayElementAtIndex(index);
                    var clipTypeF = animationClipField.GetArrayElementAtIndex(index);
                    
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        actTypeF.enumValueIndex = index;
                        EditorGUILayout.PropertyField(clipTypeF, new GUIContent(action.ToString()));
                        ++index;
                    }

                }
            }
        }

        private void CheckAndSetList()
        {
            var actTypeField = serializedObject.FindProperty(nameof(AnimatorWatcher.ActType));
            var animationClipField = serializedObject.FindProperty(nameof(AnimatorWatcher.Clips));

            if (actTypeField.arraySize == ACTION_COUNT) return;

            while (actTypeField.arraySize < ACTION_COUNT)
            {
                actTypeField.InsertArrayElementAtIndex(actTypeField.arraySize);
                animationClipField.InsertArrayElementAtIndex(animationClipField.arraySize);
            }
        }
        #endregion
    }
}
