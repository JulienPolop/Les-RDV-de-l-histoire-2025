using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button playButton;
    public Image Logo;

    Vector3 initialPlayButtonScale;

    public Main MainController;

    private void Start()
    {

    }

    private void Awake()
    {
        Debug.Log("Awake");
        initialPlayButtonScale = playButton.transform.localScale;
    }
    public void Play()
    {

        //SceneManager.LoadScene(sceneName);
        MainController.FirstStep();
    }

    public void Quit()
    {
        Application.Quit();
    }
    public void PointerEnterPlay()
    {
        Debug.Log("Hey");
        playButton.transform.localScale = initialPlayButtonScale * 1.1f;
    }

    public void PointerExitPlay()
    {
        playButton.transform.localScale = initialPlayButtonScale;
    }
}
