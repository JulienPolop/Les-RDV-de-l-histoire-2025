using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Main : MonoBehaviour
{
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
        this.cardDeck.Init(NextStep);
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
        cardDeck.Complete();
    }
}