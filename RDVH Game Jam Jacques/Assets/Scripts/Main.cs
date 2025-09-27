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

    [Header("Levels Configs")]
    int contextStep;
    [SerializeField] public List<LevelContext> contexts;
    private LevelContext currentLevelContext = null;

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
        FirstStep();
    }

    public async void FirstStep()
    {
        Debug.Log("Init First Step");
        detector.SetActive(false);

        contextStep = 0;
        if (contextStep < contexts.Count) // Get next level context
        {
            currentLevelContext = contexts[contextStep];
        }
        else // End Game
        {
            Debug.Log("End Game");
            currentLevelContext = null;
            return;
        }

        director.Init(currentLevelContext.levelEnvironment);

        detector.SetActive(true);
        //cardDeck.Complete();
    }

    public async void NextStep()
    {
        Debug.Log("Next Step");
        director.CameraShake(0.2f, 0.005f);

        detector.SetActive(false);
        if (currentLevelContext != null)
        {
            //Play destruction Animation
            currentLevelContext.levelEnvironment.Validate();
        }

        await Task.Delay(TimeSpan.FromSeconds(currentLevelContext.levelEnvironment.GetValidateTime()));


        contextStep++;
        if (contextStep < contexts.Count) // Get next level context
        {
            currentLevelContext = contexts[contextStep];
            this.attackDeck.SetGoals(currentLevelContext.config.Goal);
        }
        else // End Game
        {
            Debug.Log("End Game, GG!");
            currentLevelContext = null;
            return;
        }

        Debug.Log("Go to " + contextStep);
        director.GoTo(currentLevelContext.levelEnvironment);

        await Task.Delay(1000);

        detector.SetActive(true);
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