using UnityEngine;

[CreateAssetMenu(fileName = "CardDB", menuName = "ScriptableObjects/CardDB", order = 1)]
public class CardDB : ScriptableObject
{
    public CardData[] cards;
    public Card cardPrefab;
}

