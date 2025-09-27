using UnityEngine;

[CreateAssetMenu(fileName = "GameDirectorConfig", menuName = "ScriptableObjects/GameDirectorConfig", order = 1)]
public class GameDirectorConfig : ScriptableObject
{
    public AnimationCurve cameraEasing;
    public Vector3 cameraOffset;
}

