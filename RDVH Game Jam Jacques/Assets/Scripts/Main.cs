using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Main : MonoBehaviour
{
    [SerializeField] private AttackDeck attackDeck;
    [SerializeField] private CardDeck cardDeck;
    [SerializeField] private GameDirector director;
    [SerializeField] private ClickDetector detector;

    public void Restart()
    {
        // Get the currently active scene and reload it
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Start()
    {
        // Setup
        this.cardDeck.Init(OnCardSelected);
        this.attackDeck.Init(OnDeckFull);
        cardDeck.CompleteHand();

        // Testing
        contextStep = -1;
        NextStep();
    }

    int contextStep;
    [SerializeField] public List<LevelContext> contexts;
    private LevelContext currentLevelContext = null;

    public async void NextStep()
    {
        detector.SetActive(false);
        if (currentLevelContext != null)
        {
            currentLevelContext.levelEnvironment.Validate();
        }

        await Task.Delay(1000);


        contextStep++;
        if (contextStep < contexts.Count) // Get next level context
        {
            currentLevelContext = contexts[contextStep];
            this.attackDeck.SetGoals(currentLevelContext.config.Goal);
        }
        else // End Game
        {
            currentLevelContext = null;
            return;
        }


        director.GoTo(currentLevelContext.levelEnvironment);
        await Task.Delay(1000);

        //await Task.Delay(TimeSpan.FromSeconds(currentLevelContext.config.playableClip.duration));
        detector.SetActive(true);
        //
    }

    public void OnCardSelected(Card card)
    {
        // Bonne objectif ?
        if (attackDeck.IsNeeding(card.Data.flag))
        {
            cardDeck.Remove(card);
            attackDeck.Fill(card);
        }
        else
        {
            cardDeck.WrongRemove(card);
        }
    }
    
    public void OnDeckFull()
    {
        attackDeck.Attack();
        NextStep();
    }
}