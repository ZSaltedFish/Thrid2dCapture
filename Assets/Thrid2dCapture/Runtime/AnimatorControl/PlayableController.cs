using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace com.knight.thrid2dcapture
{
    public class PlayableController : IDisposable
    {
        public AnimationClip Clip => _clip;
        public int CurrentIndex => _currentFrame - 1;

        private AnimationClip _clip;
        private AnimationClipPlayable _playable;
        private PlayableGraph _graph;
        private AnimationPlayableOutput _output;
        private bool _inited;
        private Animator _animator;
        private int _frameCount;
        private int _currentFrame;
        
        public PlayableController(Animator anim)
        {
            _graph = PlayableGraph.Create("Controller Graph");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
            _animator = anim;
        }

        public void Dispose()
        {
            _graph.Destroy();
        }

        public void InitClip(AnimationClip clip)
        {
            _clip = clip;

            if (_inited)
            {
                _playable.Destroy();
            }
            else
            {
                _output = AnimationPlayableOutput.Create(_graph, "Out", _animator);
            }
            _inited = true;
            _playable = AnimationClipPlayable.Create(_graph, clip);
            _output.SetSourcePlayable(_playable);

            _currentFrame = 0;
            _frameCount = (int)(clip.length * clip.frameRate);
            _playable.Play();
        }

        /// <summary>
        /// 播放下一帧
        /// </summary>
        /// <returns>如果已经播放完毕返回False</returns>
        public bool GoNextFrame()
        {
            if (!_inited) throw new InvalidOperationException("尚未初始化");
            if (_currentFrame >= _frameCount) return false;
            var timeLerp = (double)_currentFrame / _frameCount;
            ++_currentFrame;
            _playable.SetTime(timeLerp * _clip.length);
            _graph.Evaluate(0);
            return true;
        }
    }
}