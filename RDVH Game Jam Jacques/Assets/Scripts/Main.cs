using System;
using System.Collections;
using System.Collections.Generic;
// using System.Threading.Tasks; // plus n�cessaire
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
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
        MainMenu();
        //FirstStep(); // garde le m�me nom: lance la routine
    }

    // ---- m�mes NOMS publics, d�sormais wrappers vers des coroutines ----
    public void MainMenu()
    {
        {
            StartCoroutine(MainMenuRoutine());
        }
    }


    public void FirstStep()
    {
        StartCoroutine(FirstStepRoutine());
    }

    public void NextStep()
    {
        StartCoroutine(NextStepRoutine());
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
        StartCoroutine(OnDeckFullRoutine());
    }

    // -------------------- Coroutines internes --------------------
    private IEnumerator MainMenuRoutine()
    {
        detector.SetActive(false);
        cardDeck.gameObject.SetActive(false);
        attackDeck.gameObject.SetActive(false);

        Debug.Log("MainMenuRoutine");
        yield return director.ShowMainMenu();
    }



    private IEnumerator FirstStepRoutine()
    {
        Debug.Log("Init First Step");
        detector.SetActive(false);
        cardDeck.gameObject.SetActive(false);
        attackDeck.gameObject.SetActive(false);

        contextStep = 0;
        if (contextStep < contexts.Count)
        {
            currentLevelContext = contexts[contextStep];
            this.attackDeck.SetGoals(currentLevelContext.config.Goal);
        }
        else
        {
            Debug.Log("End Game");
            currentLevelContext = null;
            yield break;
        }

        Debug.Log("Just Before Director.Init");
        // director.Init est d�j� une coroutine (m�me nom)
        yield return director.Init(currentLevelContext.levelEnvironment);


        cardDeck.gameObject.SetActive(true);
        attackDeck.gameObject.SetActive(true);

        yield return cardDeck.ReRoll();

        yield return new WaitForSeconds(2);

        // Tuto (m�me nom StartTuto, version coroutine)
        tutoManager.gameObject.SetActive(true);
        yield return tutoManager.StartTuto();

        detector.SetActive(true);
    }

    private IEnumerator NextStepRoutine()
    {
        Debug.Log("Next Step");
        detector.SetActive(false);
        cardDeck.gameObject.SetActive(false);
        attackDeck.gameObject.SetActive(false);

        yield return director.EndStep();

        if (currentLevelContext != null)
        {
            // Play destruction Animation
            currentLevelContext.levelEnvironment.Validate();
        }

        // Attente de la dur�e de validation
        float wait = Mathf.Max(0f, (float)currentLevelContext.levelEnvironment.GetValidateTime());
        yield return new WaitForSeconds(wait);

        contextStep++;
        if (contextStep < contexts.Count)
        {
            currentLevelContext = contexts[contextStep];
            this.attackDeck.SetGoals(currentLevelContext.config.Goal);
        }
        else
        {
            Debug.Log("End Game, GG!");
            currentLevelContext = null;
            yield return director.GoToOutro(outroContext.levelEnvironment);
            yield break;
        }

        Debug.Log("Go to " + contextStep);
        yield return director.GoTo(currentLevelContext.levelEnvironment);

        detector.SetActive(true);
        cardDeck.gameObject.SetActive(true);
        attackDeck.gameObject.SetActive(true);

        yield return cardDeck.ReRoll();
    }

    private IEnumerator OnDeckFullRoutine()
    {
        // attackDeck.Attack est suppos�e coroutine d�sormais
        yield return attackDeck.Attack();

        // Remplace Task.Delay(0.5s)
        yield return new WaitForSeconds(0.5f);

        // Encha�ne sur la suite
        NextStep(); // wrapper public -> lance la coroutine
    }
}
