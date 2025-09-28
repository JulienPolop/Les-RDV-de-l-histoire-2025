using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "ScriptableObjects/CardData", order = 1)]
public class CardData : ScriptableObject
{
    public CardFlag flag = CardFlag.Building;

    public string Title;
    [TextArea]public string Description;
    public Sprite DescriptionImage;

    public Texture2D IllustrationTexture;
    public Texture2D BandTexture;
    public Texture2D BackTexture;
    public Color BorderColor;
}
