using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    private Action<Card> OnCardCliked;

    [SerializeField] private CardDeckConfig config;
    [SerializeField] private CardDB DB;
    [Space(20)]
    [SerializeField] private Transform DeckZone;

    [Space(20)]
    [SerializeField] private InteractWithPoint rerollInteractor;
    [SerializeField] private ClocheController bellController;
    [SerializeField] private CardDescriptor descriptor;

    private List<Card> cards = new();

    public void Init(Action<Card> OnCardCliked)
    {
        this.OnCardCliked = OnCardCliked;
        rerollInteractor.OnClick = this.OnClickReroll;
    }

    private void OnClickReroll()
    {
        bellController.RingBell();
        // ReRoll est maintenant une coroutine : on la lance
        StartCoroutine(ReRoll());
    }

    private void OnHoverEnd(Card card)
    {
        this.descriptor.Hide();
        card.OnHoverEnd();
    }

    private void OnHoverStart(Card card)
    {
        this.descriptor.Describe(card.Data);
        AudioManager.Play("cardHoover");
        card.OnHoverStart();
    }

    public IEnumerator ReRoll()
    {
        rerollInteractor.Interactable = false;

        yield return DestroyHand();
        CompleteHand();

        rerollInteractor.Interactable = true;
    }

    public IEnumerator DestroyHand()
    {
        // lancer toutes les Depop en parallèle, puis attendre qu'elles finissent
        var routines = new List<IEnumerator>(cards.Count);
        for (int i = 0; i < cards.Count; i++)
            routines.Add(cards[i].Depop());

        yield return CoroutineHelper.WaitAll(this, routines);

        cards = new();
    }

    public void CompleteHand()
    {
        cards = new();
        for (int i = 0; i < config.CARD_COUNT; i++)
        {
            float unitOffset = i - (config.CARD_COUNT - 1) / 2f;
            Vector3 localPosition = Vector3.left * unitOffset * config.SPACING_BTW_CARDS;

            CardData pickedCardConfig = this.PickPrefab();
            Card newCard = GameObject.Instantiate(
                config.CardPrefab,
                DeckZone.transform.position + localPosition,
                config.CARD_ORIENTATION,
                DeckZone
            );

            newCard.Interactor.OnClick = () => OnCardCliked(newCard);
            newCard.Interactor.OnHoverEnd = () => OnHoverEnd(newCard);
            newCard.Interactor.OnHoverStart = () => OnHoverStart(newCard);

            newCard.Set(pickedCardConfig);

            // Pop en "fire-and-forget"
            StartCoroutine(newCard.Pop());

            cards.Add(newCard);
        }
    }

    private CardData PickPrefab()
    {
        int randomIndex = UnityEngine.Random.Range(0, DB.cards.Length);
        return DB.cards[randomIndex];
    }

    public void WrongRemove(Card card)
    {
        card.Wrong();
        // TODO SON ?
    }

    public void Remove(Card card)
    {
        this.cards.Remove(card);
    }
}
