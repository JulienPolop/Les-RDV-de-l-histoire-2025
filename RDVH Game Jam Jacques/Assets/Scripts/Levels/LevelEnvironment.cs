using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LevelEnvironment : MonoBehaviour
{
    public Transform cameraFocusPoint;
    public PlayableDirector playableDirector;
    public Transform VaubanPosition;
    public List<string> VaubanDialog;

    public void Validate()
    {
        Debug.Log("Piou piou, enviroment is shaking !");
        if(playableDirector != null)
            playableDirector.Play();
    }

    public double GetValidateTime()
    {
        return playableDirector.playableAsset.duration;
    }
}

