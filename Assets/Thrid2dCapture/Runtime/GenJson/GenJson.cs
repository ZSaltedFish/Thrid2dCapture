using System;

namespace com.knight.thrid2dcapture
{

    [Obsolete("Old Action Type")]
    public class ActionJson
    {
        public ActionType Type;
        public int FrameCount;
        public string AnimName;
        public string BaseColorTextureArrayPath;
        public string MaskTextureArrayPath;
        public string[] AnimationClipPaths;
    }

    public class SingleActionJson
    {
        public ActionType Type;
        public int FrameCount;
        public string AnimName;
        public string BaseColorTextureArrayPath;
        public string MaskTextureArrayPath;
        public string AnimationClipPath;
    }

    public class GenJson
    {
        public string BasePath;
        public string CharName;
        public int TextureWidth;
        public int TextureHeight;
        [Obsolete("Old Action Type")]
        public ActionJson[] ActionJsons;
        public SingleActionJson[] SingleActionJsons;
        public string ControllerPath;
        public int Rate;
        public bool ExtensionGen;
        public string CustomGenClassName;
    }
}
