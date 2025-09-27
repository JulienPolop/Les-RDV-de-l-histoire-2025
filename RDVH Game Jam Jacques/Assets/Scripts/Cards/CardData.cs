using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "ScriptableObjects/CardData", order = 1)]
public class CardData : ScriptableObject
{
    public CardFlag flag = CardFlag.Building;

    public string Title;
    public string Description;

    public Material cardMaterial;
    public Material topCardMaterial;
    public Material buttonCardMaterial;
}
