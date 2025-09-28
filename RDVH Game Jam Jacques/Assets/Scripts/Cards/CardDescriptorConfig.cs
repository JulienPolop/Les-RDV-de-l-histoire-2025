using UnityEngine;

[CreateAssetMenu(fileName = "CardDescriptorConfig", menuName = "ScriptableObjects/CardDescriptorConfig", order = 1)]
public class CardDescriptorConfig : ScriptableObject
{
    public float BUMP_DURATION = 1f;
    public float BUMP_OFFSET = 10f;
    public AnimationCurve BUMP_OFFSET_CURVE;
    public float IMAGE_DURATION = 4f;
    public AnimationCurve IMAGE_ROTATION_CURVE;
    public float IMAGE_ROTATION_AMPLITUDE = 20f;
}
