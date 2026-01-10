using System;
using System.IO;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class ScreenShoot : MonoBehaviour
    {
        public Camera MainCamera;
        public string SavePath;
        public Material MergeMaterial;
        public LayerMask MaskCameraLayer;
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
            if (!_targetCamera) return;

            var bytes = CaptureCameraWithMask();
            var name = Path.Combine(SavePath, $"{charName}_{animName}_{rotate}", $"{index}.png");
            Debug.Log($"Output: {name}");

            var fileDirection = Path.GetDirectoryName(name);
            if (!Directory.Exists(fileDirection)) 
            {
                Directory.CreateDirectory(fileDirection);
            }

            try
            {
                File.WriteAllBytes(name, bytes);
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

        private byte[] CaptureCameraWithMask()
        {
            if (!_targetCamera || !_maskCamera) return null;
            if (!_targetTexture || !_maskTexture) InitializeRenderTexture(Screen.width, Screen.height);

            _targetCamera.Render();

            Shader.SetGlobalFloat("_MASKSWITCH", 1f);
            _maskCamera.Render();
            Shader.SetGlobalFloat("_MASKSWITCH", 0f);
            var mergedTexture = MergeTexture(_targetCamera.targetTexture, _maskCamera.targetTexture);
            return mergedTexture.EncodeToPNG();
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

        private Texture2D MergeTexture(Texture colorTex, Texture maskTex)
        {
            if (!MergeMaterial)
            {
                Debug.LogError("Merge Material is null!");
                return null;
            }

            var desc = new RenderTextureDescriptor(colorTex.width, colorTex.height, RenderTextureFormat.ARGB32)
            {
                sRGB = false
            };

            var rt = RenderTexture.GetTemporary(desc);
            rt.filterMode = FilterMode.Point;

            MergeMaterial.SetTexture("_MainTex", colorTex);
            MergeMaterial.SetTexture("_MaskTex", maskTex);
            Graphics.Blit(null, rt, MergeMaterial);
            RenderTexture.active = rt;
            var tex2D = new Texture2D(colorTex.width, colorTex.height, TextureFormat.RGBA32, false);
            tex2D.ReadPixels(new Rect(0, 0, colorTex.width, colorTex.height), 0, 0);
            tex2D.Apply();
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            return tex2D;
        }
    }
}
