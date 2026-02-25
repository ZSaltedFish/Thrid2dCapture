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
            var aniamtionInfoField = serializedObject.FindProperty(nameof(AnimatorWatcher.AnimationInfos));

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                var index = 0;
                foreach (var action in ACTION_TYPES)
                {
                    var infoF = aniamtionInfoField.GetArrayElementAtIndex(index);
                    if (infoF == null) continue;
                    var actTypeF = infoF.FindPropertyRelative(nameof(AnimatorWatcher.AnimationInfo.ActType));
                    var clipTypeF = infoF.FindPropertyRelative(nameof(AnimatorWatcher.AnimationInfo.Clip));
                    var enableF = infoF.FindPropertyRelative(nameof(AnimatorWatcher.AnimationInfo.Enable));

                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        actTypeF.enumValueIndex = index;
                        EditorGUILayout.PropertyField(clipTypeF, new GUIContent(action.ToString()));
                        EditorGUILayout.PropertyField(enableF, new GUIContent(), GUILayout.Width(15f));
                        ++index;
                    }

                }
            }
        }

        private void CheckAndSetList()
        {
            var aniamtionInfoField = serializedObject.FindProperty(nameof(AnimatorWatcher.AnimationInfos));

            if (aniamtionInfoField.arraySize == ACTION_COUNT) return;

            while (aniamtionInfoField.arraySize < ACTION_COUNT)
            {
                aniamtionInfoField.InsertArrayElementAtIndex(aniamtionInfoField.arraySize);
            }
        }
        #endregion
    }
}
