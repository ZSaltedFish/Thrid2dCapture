using UnityEditor;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public static class SpriteSetting
    {
        public static void SetPathTextue2Sprite(string path)
        {
            var texGUIDs = AssetDatabase.FindAssetGUIDs("t:Texture2D", new[] { path });
            var count = 0;
            foreach (var texGUID in texGUIDs)
            {
                var texPath = AssetDatabase.GUIDToAssetPath(texGUID);
                var importer = AssetImporter.GetAtPath(texPath) as TextureImporter;

                if (!importer) continue;
                ++count;

                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.filterMode = FilterMode.Bilinear;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;

                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }

            AssetDatabase.Refresh();

            Debug.Log($"重新加载了{count}个图片");
        }
    }
}