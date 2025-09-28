using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AttackDeck : MonoBehaviour
{
    public Action OnDeckFull;

    [SerializeField] private CardDeckConfig config;
    [Space(20)]
    [SerializeField] private Transform DeckZone;

    [Space(20)]

    private List<CardPlaceHolder> cardHolders = new();
    private List<CardFlag> goals;

    public void Init(Action OnDeckFull)
    {
        this.OnDeckFull = OnDeckFull;
    }

    public void SetGoals(Goal goal)
    {
        goals = goal.Flags;
        CompleteHand();
    }

    public async Task Attack()
    {
        Task[] tasks = new Task[cardHolders.Count];
        for (int i = 0; i < cardHolders.Count; i++)
        {
            tasks[i] = cardHolders[i].Attack();
        }

        await Task.WhenAll(tasks);
        cardHolders = new();
    }

    public void CompleteHand()
    {
        cardHolders = new();
        for (int i = 0; i < goals.Count; i++)
        {
            float unitOffset = i - (goals.Count - 1) / 2f;
            Vector3 localPosition = Vector3.left * unitOffset * config.SPACING_BTW_CARDS;
            CardPlaceHolder newCardPlaceHolder = GameObject.Instantiate(config.CardPlaceHolderPrefab, DeckZone.transform.position + localPosition, config.CARD_ORIENTATION, DeckZone);
            newCardPlaceHolder.Set(goals[i]);
            newCardPlaceHolder.Pop();
            cardHolders.Add(newCardPlaceHolder);
        }
    }

    public bool IsNeeding(CardFlag flag)
    {
        foreach (CardPlaceHolder cardholder in cardHolders)
        {
            if (cardholder.IsEmpty && flag == cardholder.Flag)
            {
                return true;
            }
        }
        return false;
    }

    public async void Fill(Card card)
    {
        foreach (CardPlaceHolder cardholder in cardHolders)
        {
            if (cardholder.IsEmpty && card.Data.flag == cardholder.Flag)
            {
                cardholder.Fill(card);
                bool isDeckFull = IsDeckFull();

                await cardholder.Aspire();

                if (isDeckFull)
                {
                    OnDeckFull?.Invoke();
                }
                return;
            }
        }
    }

    public bool IsDeckFull()
    {
        foreach (CardPlaceHolder cardholder in cardHolders)
        {
            if (cardholder.IsEmpty)
            {
                return false;
            }
        }

        return true;
    }
}
