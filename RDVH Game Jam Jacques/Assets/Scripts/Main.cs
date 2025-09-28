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

    [SerializeField] private TutoManager tutoManager;
    [SerializeField] private FadeController fadeController;

    [Header("Levels Configs")]
    int contextStep;
    [SerializeField] public List<LevelContext> contexts;
    [SerializeField] public LevelContext outroContext;
    private LevelContext currentLevelContext = null;

    public void Restart()
    {
        // Get the currently active scene and reload it
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        // Load the first scene of the game
        SceneManager.LoadScene(0);
    }

    public void Start()
    {
        StartCoroutine(fadeController.FadeIn());

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
        cardDeck.gameObject.SetActive(false);
        attackDeck.gameObject.SetActive(false);

        contextStep = 0;
        if (contextStep < contexts.Count) // Get next level context
        {
            currentLevelContext = contexts[contextStep];
            this.attackDeck.SetGoals(currentLevelContext.config.Goal);
        }
        else // End Game
        {
            Debug.Log("End Game");
            currentLevelContext = null;
            return;
        }

        await director.Init(currentLevelContext.levelEnvironment);

        tutoManager.gameObject.SetActive(true);
        await tutoManager.StartTuto();

        cardDeck.gameObject.SetActive(true);
        attackDeck.gameObject.SetActive(true);
        detector.SetActive(true);
    }

    public async void NextStep()
    {
        Debug.Log("Next Step");
        detector.SetActive(false);
        cardDeck.gameObject.SetActive(false);
        attackDeck.gameObject.SetActive(false);

        await director.EndStep();

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
            await director.GoToOutro(outroContext.levelEnvironment);
            return;
        }

        Debug.Log("Go to " + contextStep);
        await director.GoTo(currentLevelContext.levelEnvironment);

        detector.SetActive(true);
        cardDeck.gameObject.SetActive(true);
        attackDeck.gameObject.SetActive(true);

        cardDeck.ReRoll();
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