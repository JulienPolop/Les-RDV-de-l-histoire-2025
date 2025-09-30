using System;
using System.Collections;
using System.Collections.Generic;
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

    // Était: async Task Attack()
    public IEnumerator Attack()
    {
        // Lancer toutes les attaques en parallèle sur chaque placeholder
        var routines = new List<IEnumerator>(cardHolders.Count);
        for (int i = 0; i < cardHolders.Count; i++)
        {
            // Suppose que CardPlaceHolder.Attack() est aussi une coroutine désormais
            routines.Add(cardHolders[i].Attack());
        }

        // Attendre que toutes soient terminées
        yield return WaitAll(routines);

        // Vider la main
        cardHolders = new();
    }

    public void CompleteHand()
    {
        cardHolders = new();
        for (int i = 0; i < goals.Count; i++)
        {
            float unitOffset = i - (goals.Count - 1) / 2f;
            Vector3 localPosition = Vector3.left * unitOffset * config.SPACING_BTW_CARDS;

            CardPlaceHolder newCardPlaceHolder = GameObject.Instantiate(
                config.CardPlaceHolderPrefab,
                DeckZone.transform.position + localPosition,
                config.CARD_ORIENTATION,
                DeckZone
            );

            newCardPlaceHolder.Set(goals[i]);

            // Était: newCardPlaceHolder.Pop(); (async)
            newCardPlaceHolder.Pop();

            cardHolders.Add(newCardPlaceHolder);
        }
    }

    public bool IsNeeding(CardFlag flag)
    {
        foreach (CardPlaceHolder cardholder in cardHolders)
        {
            if (cardholder.IsEmpty && flag == cardholder.Flag)
                return true;
        }
        return false;
    }

    // Était: async void Fill(Card card)
    public void Fill(Card card)
    {
        foreach (CardPlaceHolder cardholder in cardHolders)
        {
            if (cardholder.IsEmpty && card.Data.flag == cardholder.Flag)
            {
                // On “remplit” tout de suite, comme avant
                cardholder.Fill(card);

                // On calcule si le deck est plein AVANT l’aspiration (même logique que l’original)
                bool isDeckFull = IsDeckFull();

                // Puis on enchaîne la séquence d’aspiration + éventuel OnDeckFull dans une coroutine
                StartCoroutine(FillRoutine(cardholder, isDeckFull));
                return;
            }
        }
    }

    private IEnumerator FillRoutine(CardPlaceHolder holder, bool isDeckFullAfterFill)
    {
        // Était: await cardholder.Aspire();
        yield return holder.Aspire();

        if (isDeckFullAfterFill)
            OnDeckFull?.Invoke();
    }

    public bool IsDeckFull()
    {
        foreach (CardPlaceHolder cardholder in cardHolders)
        {
            if (cardholder.IsEmpty)
                return false;
        }
        return true;
    }

    // -------- helper parallèle (équivalent Task.WhenAll pour coroutines) --------
    private IEnumerator WaitAll(List<IEnumerator> routines)
    {
        if (routines == null || routines.Count == 0) yield break;

        int remaining = routines.Count;

        for (int i = 0; i < routines.Count; i++)
            StartCoroutine(RunAndFlag(routines[i], () => remaining--));

        while (remaining > 0)
            yield return null;
    }

    private IEnumerator RunAndFlag(IEnumerator routine, Action onDone)
    {
        yield return StartCoroutine(routine);
        onDone?.Invoke();
    }
}
