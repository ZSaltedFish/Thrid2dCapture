using System;

namespace com.knight.thrid2dcapture
{

    public class ActionJson
    {
        public ActionType Type;
        public int FrameCount;
        public string AnimName;
        public string BaseColorTextureArrayPath;
        public string MaskTextureArrayPath;
        public string[] AnimationClipPaths;
    }

    public class GenJson
    {
        public string BasePath;
        public string CharName;
        public int TextureWidth;
        public int TextureHeight;
        public ActionJson[] ActionJsons;
        public string ControllerPath;
        public int Rate;
        public bool ExtensionGen;
    }
}
