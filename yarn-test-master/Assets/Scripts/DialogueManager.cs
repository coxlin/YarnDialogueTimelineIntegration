
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Playables;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { private set; get; }
    private PlayableDirector _activeDirector;

    private void Awake()
    {
        Instance = this;
    }

    //Called by the TimeMachine Clip (of type Pause)
    public void PauseTimeline(PlayableDirector whichOne)
    {
        _activeDirector = whichOne;
        _activeDirector.Pause();
    }

    //Called by the DialogueImplementation
    public void ResumeTimeline()
    {
        _activeDirector.Resume();
    }
}
