using System;
using System.IO;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class ScreenShoot : MonoBehaviour
    {
        public Camera MainCamera;
        public string SavePath;
        public LayerMask MaskCameraLayer;
        public Vector2 Size = new(1024, 1024);
        private RenderTexture _targetTexture, _maskTexture;
        private Camera _targetCamera, _maskCamera;
        public string AssetRootPath
        {
            get
            {
                var path = SavePath[SavePath.IndexOf("Assets")..];
                return path;
            }
        }

        public void OutputShoot(string charName, string rotate, string animName, int index)
        {
            var rgbBytes = CaptureCamera();
            var maskBytes = CaptureCameraMask();
            var rgbName = Path.Combine($"{SavePath}/{charName}/{animName}", $"{rotate}_{index}.png");
            var maskName = Path.Combine($"{SavePath}/{charName}/{animName}", $"{rotate}_{index}_mask.png");

            var fileDirection = Path.GetDirectoryName(rgbName);
            if (!Directory.Exists(fileDirection)) 
            {
                Directory.CreateDirectory(fileDirection);
            }

            try
            {
                File.WriteAllBytes(rgbName, rgbBytes);
                File.WriteAllBytes(maskName, maskBytes);
            }
            catch (Exception err)
            {
                Debug.LogException(err);
            }
        }

        public void Oestroy()
        {
            if (_targetTexture)
            {
                _targetCamera.targetTexture = null;
                _targetTexture.Release();
                DestroyImmediate(_targetTexture);
            }

            if (_maskTexture)
            {
                _maskCamera.targetTexture = null;
                _maskTexture.Release();
                DestroyImmediate(_maskTexture);
            }

            if (_targetCamera)
            {
                Destroy(_targetCamera.gameObject);
            }

            if (_maskCamera)
            {
                Destroy(_maskCamera.gameObject);
            }
        }

        private byte[] CaptureCamera()
        {
            if (!_targetCamera || !_maskCamera) InitializedRenderCamera();
            if (!_targetTexture || !_maskTexture) InitializeRenderTexture((int)Size.x, (int)Size.y);

            _targetCamera.Render();
            var tex = new Texture2D(_targetCamera.targetTexture.width, _targetCamera.targetTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = _targetCamera.targetTexture;
            tex.ReadPixels(new Rect(0, 0, _targetCamera.targetTexture.width, _targetCamera.targetTexture.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
            return tex.EncodeToPNG();
        }

        private byte[] CaptureCameraMask()
        {
            if (!_targetCamera || !_maskCamera) InitializedRenderCamera();
            if (!_targetTexture || !_maskTexture) InitializeRenderTexture((int)Size.x, (int)Size.y);

            Shader.SetGlobalFloat("_MASKSWITCH", 1f);
            _maskCamera.Render();
            Shader.SetGlobalFloat("_MASKSWITCH", 0f);

            var tex = new Texture2D(_maskCamera.targetTexture.width, _maskCamera.targetTexture.height, TextureFormat.R8, false);
            RenderTexture.active = _maskCamera.targetTexture;
            tex.ReadPixels(new Rect(0, 0, _maskCamera.targetTexture.width, _maskCamera.targetTexture.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
            return tex.EncodeToPNG();
        }

        private void InitializeRenderTexture(int width, int height)
        {
            if (!_targetTexture)
            {
                _targetTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
                _targetCamera.targetTexture = _targetTexture;
            }

            if (!_maskTexture)
            {
                _maskTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
                _maskCamera.targetTexture = _maskTexture;
            }
        }

        private void InitializedRenderCamera()
        {
            if (!_targetCamera)
            {
                _targetCamera = new GameObject("TargetCamera").AddComponent<Camera>();
                _targetCamera.transform.SetParent(MainCamera.transform);
                _targetCamera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _targetCamera.transform.localScale = Vector3.one;
                _targetCamera.CopyFrom(MainCamera);
                _targetCamera.enabled = false;
            }

            if (!_maskCamera)
            {
                _maskCamera = new GameObject("MaskCamera").AddComponent<Camera>();
                _maskCamera.transform.SetParent(MainCamera.transform);
                _maskCamera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _maskCamera.transform.localScale = Vector3.one;
                _maskCamera.CopyFrom(MainCamera);
                _maskCamera.cullingMask = MaskCameraLayer;
                _maskCamera.enabled = false;
            }
        }
        #region FOR_TESTING
#if THRID2DCAPTURE
        public void TryCaptureRenderTexture(out byte[] baseColorBytes, out byte[] maskBytes)
        {
            baseColorBytes = CaptureCamera();
            maskBytes = CaptureCameraMask();
        }
#endif
        #endregion
    }
}
