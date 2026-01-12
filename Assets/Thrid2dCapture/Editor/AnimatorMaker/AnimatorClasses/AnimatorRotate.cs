using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace com.knight.thrid2dcapture
{
    public static class AnimatorRotate
    {
        public const string ROTATE_NAME = "RotateParam";
        public static Dictionary<ActionType, AnimatorControllerParameterType> ACTION_ANIM_CTRL_TYPE = new()
        {
            {ActionType.Attack, AnimatorControllerParameterType.Trigger },
            {ActionType.Hit, AnimatorControllerParameterType.Trigger },
            {ActionType.SpecialAttack, AnimatorControllerParameterType.Trigger },
            {ActionType.Skill1, AnimatorControllerParameterType.Trigger },
            {ActionType.Skill2, AnimatorControllerParameterType.Trigger },
            {ActionType.Skill3, AnimatorControllerParameterType.Trigger },
            {ActionType.Move, AnimatorControllerParameterType.Bool },
            {ActionType.Die, AnimatorControllerParameterType.Bool },
            {ActionType.Idle, AnimatorControllerParameterType.Bool },
        };

        //public static void CreateRotateAnimator(this AnimatorController controller, string jsonPath)
        //{
        //    var genJson = LoadJson(jsonPath);
        //    TryCreateParam(controller);
        //    var mainMachain = controller.layers[0].stateMachine;
        //    var index = 0;
        //    foreach (var actionJson in genJson.ActionJsons)
        //    {
        //        if (actionJson.RotateActions == null || actionJson.RotateActions.Length == 0)
        //        {
        //            Debug.LogError($"No Data about {actionJson.Type}");
        //            continue;
        //        }
        //        var x = index * actionJson.RotateActions.Length * 100;
        //        var y = index * actionJson.RotateActions.Length * 100;
        //        var subState = mainMachain.AddStateMachine(actionJson.Type.ToString(), new Vector3(x, y, 0));
        //        foreach (var rotateAction in actionJson.RotateActions)
        //        {
        //            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(rotateAction.Path);
        //            if (!clip)
        //            {
        //                Debug.LogError($"No Rotate {rotateAction.Path}");
        //                continue;
        //            }
        //            var state = subState.AddState($"{clip.name}_state", GetPosition(rotateAction.RotateType));
        //            state.motion = clip;
        //            CreateAnyToState(mainMachain, state, rotateAction.RotateType, actionJson.Type);
        //        }
        //    }
        //    EditorUtility.SetDirty(controller);
        //}

        public static void RunText(this AnimatorController ctrl, ActionType action, string path, string charName, string animationName)
        {
            TryCreateParam(ctrl);
            var stateMachine = CreateSubState(ctrl, action, path, charName, animationName);
            //stateMachine.states = SetStatePosition(stateMachine.states);
            Debug.Log($"Length {stateMachine.states.Length}");
        }

        private static GenJson LoadJson(string jsonPath)
        {
            var json = File.ReadAllText(jsonPath);
            return JsonConvert.DeserializeObject<GenJson>(json);
        }

        private static ChildAnimatorState[] SetStatePosition(ChildAnimatorState[] states)
        {
            var rotates = Enum.GetValues(typeof(RotateType)).Cast<RotateType>();
            var stateList = new List<ChildAnimatorState>(states);
            var rta = Mathf.PI * .5f;
            var step = Mathf.PI * .125f;
            var dist = 5f;
            var index = 0;
            foreach (var rotate in rotates)
            {
                if (rotate == RotateType.End) continue;
                var name = rotate.ToString();
                var state = stateList.Find(state => state.state.name.EndsWith($"{name}_state"));
                if (state.state == null)
                {
                    Debug.LogError($"找不到动作朝向:{name}");
                    continue;
                }

                var nRta = rta - index * step;
                var x = Mathf.Cos(nRta) * dist;
                var y = Mathf.Sin(nRta) * dist;
                ++index;
                state.position = new Vector3(x, y, 0);
            }
            return states;
        }

        public static void TryCreateParam(AnimatorController controller)
        {
            if (HasParam(controller, ROTATE_NAME)) return;

            controller.AddParameter(ROTATE_NAME, AnimatorControllerParameterType.Int);
            var actionTypes = Enum.GetValues(typeof(ActionType)).Cast<ActionType>();

            foreach (var actionType in actionTypes)
            {
                var paramType = ACTION_ANIM_CTRL_TYPE[actionType];
                controller.AddParameter(actionType.ToString(), paramType);
            }

            controller.AddParameter("AttackPlaybackSpeed", AnimatorControllerParameterType.Float);
        }

        private static AnimatorStateMachine CreateSubState(AnimatorController controller, ActionType action, string path, string charName, string animationName)
        {
            var name = Path.GetFileName(path);
            var mainMachine = controller.layers[0].stateMachine;
            if (HasMachine(mainMachine, name))
            {
                return mainMachine.stateMachines.FirstOrDefault(p => p.stateMachine.name == name).stateMachine;
            }

            var subState = mainMachine.AddStateMachine(name);

            var clips = GetClips(path, $"{charName}_{animationName}");
            foreach (var clip in clips)
            {
                var rotateType = NameToRotateType(clip.name);
                var state = subState.AddState($"{clip.name}_state", GetPosition(rotateType));
                state.motion = clip;
                CreateAnyToState(mainMachine, state, rotateType, action);
            }

            return subState;
        }

        private static void CreateAnyToState(AnimatorStateMachine mainMachine, AnimatorState state, RotateType rotateType, ActionType action)
        {
            var positionIndex = (int)rotateType;
            var trans = mainMachine.AddAnyStateTransition(state);
            trans.AddCondition(AnimatorConditionMode.Equals, positionIndex, ROTATE_NAME);
            var paramType = ACTION_ANIM_CTRL_TYPE[action];

            if (paramType == AnimatorControllerParameterType.Bool)
            {
                trans.AddCondition(AnimatorConditionMode.If, 1, action.ToString());
            }
            else
            {
                trans.AddCondition(AnimatorConditionMode.If, 1, action.ToString());
            }
            trans.canTransitionToSelf = false;
            trans.hasFixedDuration = false;
            trans.duration = 0;
        }

        private static Vector3 GetPosition(RotateType rotateType)
        {
            var positionIndex = (int)rotateType;
            var rta = Mathf.PI * .5f;
            var step = Mathf.PI * .125f;
            var dist = 500f;

            var nRta = rta - positionIndex * step;
            var x = Mathf.Cos(nRta) * dist;
            var y = Mathf.Sin(nRta) * dist;

            return new Vector3(x, y, 0);
        }
        
        private static AnimationClip[] GetClips(string path, string nameFilter)
        {
            var list = new List<AnimationClip>();
            var clipGUIDs = AssetDatabase.FindAssetGUIDs($"{nameFilter} t:AnimationClip", new[] { path });
            foreach (var guid in clipGUIDs)
            {
                var clip = AssetDatabase.LoadAssetByGUID<AnimationClip>(guid);
                list.Add(clip);
            }
            return list.ToArray();
        }

        private static bool HasMachine(AnimatorStateMachine machine, string name)
        {
            foreach (var sub in machine.stateMachines)
            {
                if (sub.stateMachine.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool HasParam(AnimatorController controller, string name)
        {
            var value = controller.parameters.FirstOrDefault(p => p.name == name);
            return value != null;
        }

        private static RotateType NameToRotateType(string stateName)
        {
            var subName = stateName[(stateName.LastIndexOf('_') + 1)..];
            _ = Enum.TryParse<RotateType>(subName, out var type);
            return type;
        }
    }
}