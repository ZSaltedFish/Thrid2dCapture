using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace com.knight.thrid2dcapture
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorWatcher : MonoBehaviour
    {
        public AnimationClip[] Clips;
        private PlayableGraph _graph;

        public void Start()
        {
            var animator = GetComponent<Animator>();
            _graph = PlayableGraph.Create("Animation Playable");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

            if (Clips.Length == 0) return;
            var playable = AnimationClipPlayable.Create(_graph, Clips[0]);
            var output = AnimationPlayableOutput.Create(_graph, "Output", animator);
            output.SetSourcePlayable(playable);

            
            _graph.Play();
        }

        public void OnDestroy()
        {
            _graph.Destroy();
        }
    }
}