using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Tri2Bi.Editor
{
    [RequireComponent(typeof(Camera))]
    public class ScreenShot : MonoBehaviour
    {
        public Camera SrcCamera;
        public string SavePath;
        public string Name;

        public List<float> CaptureTimes = new()
        {
            0.5f,
            5.0f / 8.0f,
            0.75f,
            7.0f / 8.0f,
            1f
        };

        private int _count;

        public void Start()
        {
            _count = CaptureTimes.Count - 1;
        }

        public void Update()
        {
            if (CaptureTimes.Count == 0) return;

            var captureTime = CaptureTimes[0];
            if (Time.time <  captureTime) return;

            CaptureTimes.RemoveAt(0);
            var bytes = CaptureCamera();

            var index = _count - CaptureTimes.Count;
            var path = Path.Combine(SavePath, $"{Name}{index}.png");

            File.WriteAllBytes(path, bytes);
        }

        private byte[] CaptureCamera()
        {
            var width = Screen.width;
            var height = Screen.height;

            var rt = RenderTexture.GetTemporary(width, height, 24);
            SrcCamera.targetTexture = rt;

            SrcCamera.Render();

            RenderTexture.active = rt;
            var screenShot = new Texture2D(width, height, TextureFormat.ARGB32, false);
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenShot.Apply();

            SrcCamera.targetTexture = null;
            RenderTexture.ReleaseTemporary(rt);

            return screenShot.EncodeToPNG();
        }
    }
}
