using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class RotateController
    {
        public const int Divid = 16;
        private GameObject ControlGameObject;
        private int _currentRotateIndex;

        public RotateController(GameObject controlGameObject)
        {
            ControlGameObject = controlGameObject;
            _currentRotateIndex = 0;
        }

        /// <summary>
        /// 旋转到下一个状态
        /// </summary>
        /// <returns>如果已经旋转一圈，返回False</returns>
        public bool GetNextRotate()
        {
            if (_currentRotateIndex >= Divid) return false;
            ++_currentRotateIndex;
            var rotate = GetRotate(_currentRotateIndex);

            ControlGameObject.transform.rotation = rotate;
            return true;
        }
        
        private static Quaternion GetRotate(int index)
        {
            var lerp = index % Divid;
            var angle = lerp * 360.0f / Divid;
            return Quaternion.AngleAxis(angle, Vector3.up);
        }
    }
}