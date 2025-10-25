using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.knight.thrid2dcapture
{
    public class CameraControl : MonoBehaviour
    {
        [HideInInspector] public Camera SrcCamera;
        [Range(0, 90f)] [InspectorName("摄像机角度")]public float UpAngle;
        [Range(1f, 12f)] public float Distance = 5f;
        [Range(0f, 10f)] public float CameraFowardDistance = 0f;
        public int Width = 128, Height = 128;

        private int _revertWidth, _revertHeight;
        private PropertyInfo _widthProp, _heightProp;
        private object _sizeObj;

        public void Start()
        {
            SrcCamera = Camera.main;
#if UNITY_EDITOR
            SetEditorGameView(Width, Height);
#endif
        }

        public void Update()
        {
            SetCameraDirection();
            SetDistance();
        }

        #region GameView Property setting
        private void SetEditorGameView(int width, int height)
        {
#if UNITY_EDITOR
            if (_sizeObj == null)
            {
                SetGameViewProperty();
            }
            _widthProp.SetValue(_sizeObj, width, null);
            _heightProp.SetValue(_sizeObj, height, null);
#else
            Debug.LogError("仅在Editor可以调用");
#endif
        }

#if UNITY_EDITOR
        private void SetGameViewProperty()
        {
            var gameView = GetMainGameView();
            var prop = gameView.GetType().GetProperty("currentGameViewSize", BindingFlags.NonPublic | BindingFlags.Instance);
            var size = prop.GetValue(gameView, null);

            var widthProp = size.GetType().GetProperty("width", BindingFlags.Public | BindingFlags.Instance);
            var heightProp = size.GetType().GetProperty("height", BindingFlags.Public | BindingFlags.Instance);

            _sizeObj = size;
            _widthProp = widthProp;
            _heightProp = heightProp;

            _revertWidth = (int)widthProp.GetValue(size);
            _revertHeight = (int)heightProp.GetValue(size);
        }

        private static EditorWindow GetMainGameView()
        {
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
            var gameView = EditorWindow.GetWindow(type);
            return gameView;
        }
#endif
        #endregion

        #region Camera setting
        private void SetCameraDirection()
        {
            SrcCamera.transform.localEulerAngles = Vector3.zero;
            SrcCamera.transform.Rotate(Vector3.right, 90 - UpAngle);
        }

        private void SetDistance()
        {
            var angle = (90 - UpAngle) * Mathf.Deg2Rad;
            var localY = Distance * Mathf.Sin(angle);
            var localX = -Distance * Mathf.Cos(angle);

            var localPosition = new Vector3(0, localY, localX + CameraFowardDistance);
            SrcCamera.transform.position = localPosition + transform.position;
        }
        #endregion

        public void OnDestroy()
        {
#if UNITY_EDITOR
            if (_sizeObj == null) return;
            SetEditorGameView(_revertWidth, _revertHeight);
#endif
        }
    }
}
