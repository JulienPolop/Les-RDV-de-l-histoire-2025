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
    public float destroyDelay = 0.5f;
}