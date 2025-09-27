using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardDeck : MonoBehaviour
    {
        private Action OnCardCliked;

        [SerializeField] private CardDeckConfig config;
        [SerializeField] private CardDB DB;
        [Space(20)]
        [SerializeField] private Transform CardCreationPoint;
        [Space(20)]
        [SerializeField] private InteractWithPoint debugInteractor;

        private List<Card> cards = new();
        private List<Card> engagedCards = new();

        public void Init(Action OnCardCliked)
        {
            this.OnCardCliked = OnCardCliked;
            debugInteractor.OnClick = OnCardCliked;
        }

        public void Complete()
        {
            while (cards.Count < config.CARD_COUNT)
            {
                CardData pickedCardConfig = this.PickPrefab();
                Card newCard = GameObject.Instantiate(config.CardPrefab, CardCreationPoint.position, Quaternion.identity, this.transform);
                //TODO newCard.interactor.OnClick = OnCardCliked;
                newCard.Set(pickedCardConfig);
            }
        }

        private CardData PickPrefab()
        {
            int randomIndex = UnityEngine.Random.Range(0, DB.cards.Length);
            return DB.cards[randomIndex];
        }
    }
