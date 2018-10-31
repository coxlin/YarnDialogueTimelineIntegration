using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    [SerializeField]
    private TextAsset _yarnFile;

    public bool HasToPause;

    private bool _clipPlayed = false;
    private bool _pauseScheduled = false;
    private PlayableDirector _director;
    

    public override void OnPlayableCreate(Playable playable)
    {
        _director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
       if (!_clipPlayed && info.weight > 0f)
        {
            DialogueManager.Instance.PauseTimeline(_director);
            DialogueImplementation.Instance.Run(_yarnFile.text);
            _pauseScheduled = true;
            _clipPlayed = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
    }


}
