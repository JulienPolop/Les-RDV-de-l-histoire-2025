using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    private Action OnCardCliked;

    [SerializeField] private CardDeckConfig config;
    [SerializeField] private CardDB DB;
    [Space(20)]
    [SerializeField] private Transform DeckZone;
    [SerializeField] private Transform EngagementZone;
    
    [Space(20)]
    [SerializeField] private InteractWithPoint rerollInteractor;

    private List<Card> cards = new();
    private List<Card> engagedCards = new();

    public void Init(Action OnCardCliked)
    {
        this.OnCardCliked = OnCardCliked;
        rerollInteractor.OnClick = this.ReRoll;
    }

    private async void ReRoll()
    {
        rerollInteractor.Interactable = false;
        
        DestroyHand();
        await Task.Delay(TimeSpan.FromSeconds(config.REROLL_DELAY));
        CompleteHand();

        rerollInteractor.Interactable = true;
    }

    public void DestroyHand()
    {
        foreach (Card card in cards)
        {
            card.Depop();
        }
        cards = new();
    }

    public void CompleteHand()
    {
        cards = new();
        for (int i = 0; i < config.CARD_COUNT; i++)
        {
            float unitOffset = i - (config.CARD_COUNT - 1) / 2f;
            Vector3 localPosition = Vector3.left * unitOffset * config.SPACING_BTW_CARDS;
            Debug.Log(unitOffset);
            CardData pickedCardConfig = this.PickPrefab();
            Card newCard = GameObject.Instantiate(config.CardPrefab, DeckZone.transform.position + localPosition, config.CARD_ORIENTATION, DeckZone);
            newCard.Interactor.OnClick = OnCardCliked;
            newCard.Set(pickedCardConfig);
            newCard.Pop();
            cards.Add(newCard);
        }
    }

        private CardData PickPrefab()
    {
        int randomIndex = UnityEngine.Random.Range(0, DB.cards.Length);
        return DB.cards[randomIndex];
    }
}
