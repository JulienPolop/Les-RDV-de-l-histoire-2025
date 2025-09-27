using UnityEngine;
using UnityEngine.Timeline;

public class LevelEnvironment : MonoBehaviour
{
    public TimelinePlayable playable;
    public Transform cameraFocusPoint;

    public void Validate()
    {
        Debug.Log("Piou piou, enviroment is shaking !");
    }
}

