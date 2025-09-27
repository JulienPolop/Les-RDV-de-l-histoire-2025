using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public Camera camera;
    public GameDirectorConfig config;
    public void GoTo(LevelEnvironment levelEnvironment)
    {
        camera.transform.position = levelEnvironment.cameraFocusPoint.position + config.cameraOffset;
    }
}
