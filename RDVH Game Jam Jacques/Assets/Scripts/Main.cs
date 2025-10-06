using System;
using System.Collections;
using System.Collections.Generic;
// using System.Threading.Tasks; // plus nécessaire
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.STP;

public partial class Main : MonoBehaviour
{
    [SerializeField] private AttackDeck attackDeck;
    [SerializeField] private CardDeck cardDeck;
    [SerializeField] public GameDirector director;
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
        StartCoroutine(fadeController.FadeOutAndLoad(SceneManager.GetActiveScene().name));
        //SceneManager.LoadScene(0);
    }

    public void Start()
    {
        fadeController.gameObject.SetActive(true);
        StartCoroutine(fadeController.FadeIn());

        // Setup
        this.cardDeck.Init(OnCardSelected);
        this.attackDeck.Init(OnDeckFull);
        cardDeck.CompleteHand();

        // Testing
        MainMenu();
        //FirstStep(); // garde le même nom: lance la routine
    }

    // ---- mêmes NOMS publics, désormais wrappers vers des coroutines ----
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
        AudioManager.Play("startGame");
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
        // director.Init est déjà une coroutine (même nom)
        yield return director.Init(currentLevelContext.levelEnvironment);


        cardDeck.gameObject.SetActive(true);
        attackDeck.gameObject.SetActive(true);

        yield return cardDeck.ReRoll();

        yield return new WaitForSeconds(2);

        // Tuto (même nom StartTuto, version coroutine)
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

        float wait = Mathf.Max(0f, (float)currentLevelContext.levelEnvironment.GetValidateTime());
        if (currentLevelContext != null)
        {
            // Play destruction Animation
            AudioManager.Play("eboulement");
            currentLevelContext.levelEnvironment.Validate();
            director.CameraShake(wait, 0.025f);
        }

        // Attente de la durée de validation

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

        cardDeck.CompleteHand();
    }

    private IEnumerator OnDeckFullRoutine()
    {
        // Lance les d'atack
        yield return CoroutineHelper.WaitAll(this, new System.Collections.Generic.List<IEnumerator> {
                attackDeck.Attack(),
                director.OnAttack()
            });

        // Lance les animations de depop des cartes
        yield return CoroutineHelper.WaitAll(this, new System.Collections.Generic.List<IEnumerator> {
                attackDeck.DepopCards(),
                cardDeck.DestroyHand()
            });

        // Remplace Task.Delay(0.5s)
        yield return new WaitForSeconds(0.5f);

        // Enchaîne sur la suite
        NextStep(); // wrapper public -> lance la coroutine
    }
}
