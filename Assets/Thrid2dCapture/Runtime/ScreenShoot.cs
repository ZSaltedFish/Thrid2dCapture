using System;
using System.IO;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class ScreenShoot : MonoBehaviour
    {
        public Camera TargetCamera;
        public string SavePath;

        public void OutputShoot(string charName, string rotate, string animName, int index)
        {
            if (!TargetCamera) return;

            var bytes = CaptureCamera();
            var name = Path.Combine(SavePath, $"{charName}_{animName}_{rotate}", $"{index}.png");

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

        private byte[] CaptureCamera()
        {
            var width = Screen.width;
            var height = Screen.height;

            var rt = RenderTexture.GetTemporary(width, height, 24);
            TargetCamera.targetTexture = rt;
            TargetCamera.Render();

            RenderTexture.active = rt;
            var screenShoot = new Texture2D(width, height, TextureFormat.ARGB32, false);
            screenShoot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenShoot.Apply();

            TargetCamera.targetTexture = null;
            RenderTexture.ReleaseTemporary(rt);

            return screenShoot.EncodeToPNG();
        }
    }
}
