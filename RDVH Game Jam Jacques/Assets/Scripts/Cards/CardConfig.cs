using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardConfig", menuName = "ScriptableObjects/CardConfig")]
public class CardConfig : ScriptableObject
{
    public int cardMaterialIndex;
    public int topCardMaterialIndex;
    public int buttonCardMaterialIndex;
    public int flagMaterialIndex;
    public List<Material> FlagMaterial;
    public float DESTROY_DELAY = 0.5f;
}