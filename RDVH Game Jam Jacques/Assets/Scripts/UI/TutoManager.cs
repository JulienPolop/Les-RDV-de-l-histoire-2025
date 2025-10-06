using System.Collections;
using System.Collections.Generic;
using System.Threading; // pour CancellationToken
using UnityEngine;
using UnityEngine.UI;

public class TutoManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject uiTuto;      // conteneur de tout le tuto
    [SerializeField] private List<Image> pages;      // chaque page est une Image (UI)
    [SerializeField] private Button nextButton;      // bouton "Next"

    // Remplacements coroutine-friendly
    private bool _clickRequested;
    private bool _cancelRequested;
    private CancellationToken _externalToken;

    private bool _isRunning;

    void Awake()
    {
        HideAllPages();
        if (uiTuto) uiTuto.SetActive(false);
    }

    void OnDisable()
    {
        // annule si l'objet est désactivé
        _cancelRequested = true;
    }

    /// <summary>
    /// Lance le tuto et ne rend la main qu'après le dernier clic sur "Next".
    /// (Coroutine)
    /// </summary>
    public IEnumerator StartTuto(CancellationToken externalToken = default)
    {
        if (_isRunning) yield break; // évite la réentrance
        _isRunning = true;

        _cancelRequested = false;
        _externalToken = externalToken;

        nextButton.onClick.AddListener(OnNextClicked);

        if (uiTuto) uiTuto.SetActive(true);
        HideAllPages();

        if (pages != null && pages.Count > 0)
        {
            // Affiche la 1ère page
            ShowPage(0);

            // Pour chaque page sauf la dernière : attendre clic, passer à la suivante
            for (int i = 0; i < pages.Count - 1; i++)
            {
                yield return WaitNextClick(externalToken);
                if (IsCancelled()) break;

                HidePage(i);
                ShowPage(i + 1);
            }

            if (!IsCancelled())
            {
                // Dernière page visible → attendre un dernier clic
                yield return WaitNextClick(externalToken);
            }
        }
        else
        {
            // Pas de pages ? On attend un clic, puis on ferme.
            yield return WaitNextClick(externalToken);
        }

        // Fermer le tuto
        HideAllPages();
        if (uiTuto) uiTuto.SetActive(false);

        nextButton.onClick.RemoveListener(OnNextClicked);
        _isRunning = false;
    }

    // ====== Helpers ======

    private bool IsCancelled()
        => _cancelRequested || (_externalToken.CanBeCanceled && _externalToken.IsCancellationRequested);

    private void OnNextClicked()
    {
        _clickRequested = true;
        AudioManager.Play("uiButtonClick");
    }

    public IEnumerator WaitNextClick(CancellationToken token)
    {
        _clickRequested = false;
        while (!_clickRequested)
        {
            if (_cancelRequested) yield break;
            if (token.IsCancellationRequested) yield break;
            yield return null;
        }
    }

    private void ShowPage(int index)
    {
        if (pages == null || index < 0 || index >= pages.Count) return;
        if (pages[index]) pages[index].gameObject.SetActive(true);
    }

    private void HidePage(int index)
    {
        if (pages == null || index < 0 || index >= pages.Count) return;
        if (pages[index]) pages[index].gameObject.SetActive(false);
    }

    private void HideAllPages()
    {
        if (pages == null) return;
        foreach (var img in pages)
            if (img) img.gameObject.SetActive(false);
    }
}
