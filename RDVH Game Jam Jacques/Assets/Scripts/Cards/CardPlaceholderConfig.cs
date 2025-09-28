using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardPlaceHolderConfig", menuName = "ScriptableObjects/CardPlaceHolderConfig")]
public class CardPlaceHolderConfig : ScriptableObject
{
    public List<Color> FlagMaterial;
    public float ASPIRE_DELAY = 0.5f;
    public float ASPIRE_DURATION = 0.5f;
    public float ASPIRE_ENDPAUSE = 0.5f;
    public AnimationCurve aspireCurve;
    public float DESTROY_DELAY = 0.5f;
    public float POP_DURATION = 1f;
}
