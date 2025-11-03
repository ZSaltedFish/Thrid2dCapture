using UnityEditor;
using UnityEditor.Animations;

namespace com.knight.thrid2dcapture
{
    public class AnimatorMaker : EditorWindow
    {
        #region Public static call
        public static AnimatorController CreateAnimatorController(string savePath, string controllerName)
        {
            var controller = AnimatorController.CreateAnimatorControllerAtPath($"{savePath}/{controllerName}.controller");
            return controller;
        }
        #endregion
    }
}