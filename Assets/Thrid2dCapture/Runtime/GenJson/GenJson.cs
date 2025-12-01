namespace com.knight.thrid2dcapture
{
    public class RotateActionJson
    {
        public RotateType RotateType;
        public string Path;
    }

    public class ActionJson
    {
        public RotateActionJson[] RotateActions;
        public ActionType Type;
    }

    public class GenJson
    {
        public ActionJson[] ActionJsons;
    }
}
