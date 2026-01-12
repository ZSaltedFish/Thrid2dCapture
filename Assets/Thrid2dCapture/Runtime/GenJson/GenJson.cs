using System;

namespace com.knight.thrid2dcapture
{
    [Obsolete("旧方法即将被淘汰")]
    public class RotateActionJson
    {
        public RotateType RotateType;
        public string Path;
    }

    public class ActionJson
    {
        [Obsolete("旧方法即将被淘汰")]
        public RotateActionJson[] RotateActions;
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
    }
}
