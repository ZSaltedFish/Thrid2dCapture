using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class CameraControl : MonoBehaviour
    {
        [HideInInspector] public Camera SrcCamera;
        [Range(0, 90f)] [InspectorName("摄像机角度")]public float UpAngle;
        [Range(1f, 12f)] public float Distance = 5f;
        [Range(0f, 10f)] public float CameraFowardDistance = 0f;
        public int Width = 128, Height = 128;

        public void Start()
        {
            SrcCamera = Camera.main;
        }

        public void Update()
        {
            SetCameraDirection();
            SetDistance();
        }

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
    }
}
