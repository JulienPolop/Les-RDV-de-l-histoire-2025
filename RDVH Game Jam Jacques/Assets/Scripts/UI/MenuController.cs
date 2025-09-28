using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public string sceneName;
    public Transform playButton;
    //public Transform quitButton;
    //public Transform howToPlayButton;
    //public Transform nextButton;
    //public Transform backButton;

    Vector3 initialPlayButtonScale;
    //Vector3 initialQuitButtonScale;
    //Vector3 initialHowToPlayButtonScale;
    //Vector3 initialNextButtonScale;
    //Vector3 initialBackButtonScale;

    public FadeController fadeController;

    //public GameObject MainMenu;
    //public GameObject TutoMenu;

    //public Image imageUI;
    //public List<Sprite> imagesTuto;
    //private int indexTuto = 0;

    private void Start()
    {
        // Commence le fade-in quand la sc�ne d�marre
        StartCoroutine(fadeController.FadeIn());
    }

    private void Awake()
    {
        Debug.Log("Awake");
        initialPlayButtonScale = playButton.localScale;
        //initialQuitButtonScale = quitButton.localScale;
        //initialHowToPlayButtonScale = howToPlayButton.localScale;
        //initialNextButtonScale = nextButton.localScale;
        //initialBackButtonScale = backButton.localScale;
    }
    public void Play()
    {
        fadeController.FadeToScene(sceneName);
        //SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }

    //public void HowToPlay()
    //{
    //    indexTuto = -1;
    //    //MainMenu.SetActive(false);
    //    TutoMenu.SetActive(true);

    //    NextTuto();
    //}

    public void PointerEnterPlay()
    {
        Debug.Log("Hey");
        playButton.localScale = initialPlayButtonScale * 1.1f;
    }

    public void PointerExitPlay()
    {
        playButton.localScale = initialPlayButtonScale;
    }

    //public void PointerEnterQuit()
    //{
    //    quitButton.localScale = initialQuitButtonScale * 1.1f;
    //}

    //public void PointerExitQuit()
    //{
    //    quitButton.localScale = initialQuitButtonScale;
    //}

    //public void PointerEnterHowToPlay()
    //{
    //    howToPlayButton.localScale = initialHowToPlayButtonScale * 1.1f;
    //}

    //public void PointerExitHowToPlay()
    //{
    //    howToPlayButton.localScale = initialHowToPlayButtonScale;
    //}

    //public void PointerEnterNext()
    //{
    //    nextButton.localScale = initialNextButtonScale * 1.1f;
    //}

    //public void PointerExitNext()
    //{
    //    nextButton.localScale = initialNextButtonScale;
    //}

    //public void PointerEnterBack()
    //{
    //    backButton.localScale = initialBackButtonScale * 1.1f;
    //}

    //public void PointerExitBack()
    //{
    //    backButton.localScale = initialBackButtonScale;
    //}

    //public void NextTuto()
    //{
    //    indexTuto++;
    //    if (indexTuto < imagesTuto.Count)
    //    {
    //        imageUI.sprite = imagesTuto[indexTuto];
    //        if (indexTuto == imagesTuto.Count - 1)
    //            nextButton.gameObject.SetActive(false);
    //        else
    //            nextButton.gameObject.SetActive(true);
    //    }
    //}

    //public void QuitTuto()
    //{
    //    MainMenu.SetActive(true);
    //    TutoMenu.SetActive(false);
    //}
}
