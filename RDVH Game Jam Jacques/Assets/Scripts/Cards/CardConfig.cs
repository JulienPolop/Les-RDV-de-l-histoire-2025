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
    public float ATTACK_SOUND_DELAY = 1f;
    public float ATTACK_RANDOM_DELAY = 0.4f;
    public float DEPOP_RANDOM = 0.5f;
    public float DEPOP_RANDOM_DELAY = 0.2f;
    public float POP_RANDOM_DELAY = 0.4f;
    public float ANIMATION_DEPOP_WAITING_TIME = 0.6f;
    public float ANIMATION_POP_WAITING_TIME = 0.6f;
}