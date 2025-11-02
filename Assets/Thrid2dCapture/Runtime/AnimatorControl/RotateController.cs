using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class RotateController
    {
        public RotateType CurrentIndex => _currentRotateIndex;

        private readonly GameObject _controlGameObject;
        private RotateType _currentRotateIndex;

        public RotateController(GameObject controlGameObject)
        {
            _controlGameObject = controlGameObject;
            _currentRotateIndex = 0;
        }

        /// <summary>
        /// 旋转到下一个状态
        /// </summary>
        /// <returns>如果已经旋转一圈，返回False</returns>
        public bool GetNextRotate()
        {
            if (_currentRotateIndex >= RotateType.End) return false;
            ++_currentRotateIndex;
            var rotate = GetRotate(_currentRotateIndex);

            _controlGameObject.transform.localRotation = rotate;
            return true;
        }
        
        private static Quaternion GetRotate(RotateType rotateType)
        {
            int end = (int)RotateType.End;
            int index = (int)rotateType;
            var lerp = index % end;
            var angle = lerp * 360.0f / end;
            return Quaternion.AngleAxis(angle, Vector3.up);
        }
    }
}