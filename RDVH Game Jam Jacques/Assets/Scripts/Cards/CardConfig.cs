using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardConfig", menuName = "ScriptableObjects/CardConfig", order = 1)]
public class CardConfig : ScriptableObject
{
    public int cardMaterialIndex;
    public int topCardMaterialIndex;
    public int buttonCardMaterialIndex;
    public int flagMaterialIndex;
    public List<Material> FlagMaterial;
    public float DESTROY_DELAY = 0.5f;
}

[CreateAssetMenu(fileName = "CardPlaceHolderConfig", menuName = "ScriptableObjects/CardPlaceHolderConfig", order = 1)]
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