using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public class TextureArrayGen
    {
        private static readonly RotateType[] _ROT_TYPES = (RotateType[])System.Enum.GetValues(typeof(RotateType));
        private string _charName;
        private string _srcPath;
        private GenJson _srcJson;
        public TextureArrayGen(GenJson json)
        {
            _charName = json.CharName;
            _srcPath = Path.Combine(json.BasePath, json.CharName);
            _srcJson = json;
        }

        public void GenAllAnimTextureArray()
        {
            var json = _srcJson;
            foreach (var actionJson in json.ActionJsons)
            {
                GenSimgleAnimTextureArray(actionJson);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void GenSimgleAnimTextureArray(ActionJson actionJson)
        {
            var actionPath = Path.Combine(_srcPath, actionJson.AnimName);
            var count = actionJson.FrameCount;
            var baseColorArray = new Texture2DArray(_srcJson.TextureWidth, _srcJson.TextureHeight, count * (_ROT_TYPES.Length -1), TextureFormat.RGBA32, false);
            var maskArray = new Texture2DArray(_srcJson.TextureWidth, _srcJson.TextureHeight, count * (_ROT_TYPES.Length -1), TextureFormat.R8, false);
            foreach (var rotateType in _ROT_TYPES)
            {
                if (rotateType == RotateType.End) continue;
                for (var i = 0; i < count; ++i)
                {
                    var baseColorName = Path.Combine(actionPath, $"{rotateType}_{i}.png");
                    var maskName = Path.Combine(actionPath, $"{rotateType}_{i}_mask.png");
                    var baseColorTex = AssetDatabase.LoadAssetAtPath<Texture2D>(baseColorName);
                    var maskTex = AssetDatabase.LoadAssetAtPath<Texture2D>(maskName);

                    if (!baseColorTex || !maskTex)
                    {
                        throw new FileNotFoundException($"No Texture Found {baseColorName} or {maskName}");
                    }

                    baseColorArray.SetPixels32(baseColorTex.GetPixels32(), i + ((int)rotateType * count));
                    maskArray.SetPixels32(maskTex.GetPixels32(), i + ((int)rotateType * count));
                }
            }

            baseColorArray.Apply();
            maskArray.Apply();

            var baseColorArrayPath = actionJson.BaseColorTextureArrayPath;;
            var maskArrayPath = actionJson.MaskTextureArrayPath;

            AssetDatabase.CreateAsset(baseColorArray, baseColorArrayPath);
            AssetDatabase.CreateAsset(maskArray, maskArrayPath);
        }
    }
}