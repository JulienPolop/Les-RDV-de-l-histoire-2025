using UnityEngine;

[CreateAssetMenu(fileName = "CardDeckConfig", menuName = "ScriptableObjects/CardDeckConfig", order = 1)]
public class CardDeckConfig : ScriptableObject
{
    public int CARD_COUNT = 7;
    public float SPACING_BTW_CARDS = 0.25f;
    public Card CardPrefab;
}

