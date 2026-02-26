using UnityEngine;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace com.knight.thrid2dcapture
{
    public class JsonGen
    {
        public string JsonPath;
        public bool ExtensionGen;
        private string _basePath;
        private string _charName;
        private int _texWidth;
        private int _texHeight;
        private AnimationClip[] _clips;
        private ActionType[] _actTypes;
        private int _rate;
        public JsonGen(AnimationClip[] clips, ActionType[] actTypes, string basePath, string charName, int texWidth, int texHeight, int rate)
        {
            _basePath = basePath;
            _charName = charName;
            _texWidth = texWidth;
            _texHeight = texHeight;
            _clips = clips;
            _actTypes = actTypes;
            _rate = rate;
        }

        public GenJson GenerateJson()
        {
            var json = GetJson();
            var jsonPath = Path.Combine(_basePath, _charName, $"{_charName}_json.json");
            JsonPath = jsonPath;
            var serObj = JsonConvert.SerializeObject(json);
            File.WriteAllText(jsonPath, serObj);
            return json;
        }

        private GenJson GetJson()
        {
            var genJson = new GenJson
            {
                BasePath = _basePath,
                CharName = _charName,
                TextureWidth = _texWidth,
                TextureHeight = _texHeight,
                ActionJsons = new ActionJson[_actTypes.Length],
                ControllerPath = Path.Combine(_basePath, _charName, $"{_charName}_Controller.controller").Replace('\\', '/'),
                Rate = _rate,
                ExtensionGen = ExtensionGen
            };
            
            var rotateTypes = (Enum.GetValues(typeof(RotateType)) as RotateType[]).ToList();
            rotateTypes.Remove(RotateType.End);
            for (var i = 0; i < _clips.Length; ++i)
            {
                var actType = _actTypes[i];
                var clip = _clips[i];

                if (!clip) continue;
                var actionJson = new ActionJson
                {
                    Type = actType,
                    AnimName = clip.name,
                    FrameCount = Mathf.RoundToInt(clip.length * clip.frameRate),
                    BaseColorTextureArrayPath = Path.Combine(_basePath, _charName, clip.name, $"{_charName}_{actType}_BaseColorArray.asset").Replace('\\', '/'),
                    MaskTextureArrayPath = Path.Combine(_basePath, _charName, clip.name, $"{_charName}_{actType}_MaskArray.asset").Replace('\\', '/')
                };

                var clipPaths = new List<string>();
                foreach (var rotateType in rotateTypes)
                {
                    var path = Path.Combine(_basePath, _charName, clip.name, $"{_charName}_{actType}_{rotateType}.anim").Replace('\\', '/');
                    clipPaths.Add(path);
                }
                actionJson.AnimationClipPaths = clipPaths.ToArray();
                genJson.ActionJsons[i] = actionJson;
            }

            return genJson;
        }
    }
}