using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class AnimatorEntiryEditor : EditorWindow
    {
        private string _path = "";
        private string _charName = "";
        private string _animState = "";
        private bool _allAction = false;
        private ActionType _actionType = ActionType.Attack;
        private AnimatorController _controller;

        public void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                _controller = EditorGUILayout.ObjectField("Controller", _controller, typeof(AnimatorController), false) as AnimatorController;
                _path = EditorGUILayout.TextField("Path", _path);
                _charName = EditorGUILayout.TextField("CharName", _charName);
                _animState = EditorGUILayout.TextField("Anim State", _animState);
                _allAction = EditorGUILayout.Toggle("All Action", _allAction);
                _actionType = (ActionType)EditorGUILayout.EnumPopup("Action Type", _actionType);
            }
            if (GUILayout.Button("TEST"))
            {
                _controller.RunText(_actionType, _path, _charName, _animState);
            }
        }

        [MenuItem("Tools/Test/AnimatorCreator")]
        public static void Init()
        {
            var window = GetWindow<AnimatorEntiryEditor>();
            window.Show();
        }
    }
}
