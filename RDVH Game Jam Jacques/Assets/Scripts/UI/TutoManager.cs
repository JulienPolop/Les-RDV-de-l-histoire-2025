using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TutoManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject uiTuto;      // conteneur de tout le tuto
    [SerializeField] private List<Image> pages;      // chaque page est une Image (UI)
    [SerializeField] private Button nextButton;      // bouton "Next"

    private TaskCompletionSource<bool> _clickTcs;
    private CancellationTokenSource _runCts;
    private bool _isRunning;

    void Awake()
    {
        HideAllPages();
        if (uiTuto) uiTuto.SetActive(false);
    }

    void OnDisable()
    {
        _runCts?.Cancel();
    }

    /// <summary>
    /// Lance le tuto et ne rend la main qu'après le dernier clic sur "Next".
    /// </summary>
    public async Task StartTuto(CancellationToken externalToken = default)
    {
        if (_isRunning) return; // évite la réentrance
        _isRunning = true;

        _runCts?.Cancel();
        _runCts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
        var token = _runCts.Token;

        nextButton.onClick.AddListener(OnNextClicked);

        try
        {
            if (uiTuto) uiTuto.SetActive(true);
            HideAllPages();

            if (pages != null && pages.Count > 0)
            {
                // Affiche la 1ère page
                ShowPage(0);

                // Pour chaque page sauf la dernière : attendre clic, passer à la suivante
                for (int i = 0; i < pages.Count - 1; i++)
                {
                    await WaitNextClickAsync(token);
                    HidePage(i);
                    ShowPage(i + 1);
                }

                // Dernière page visible → attendre un dernier clic
                await WaitNextClickAsync(token);
            }
            else
            {
                // Pas de pages ? On attend un clic, puis on ferme.
                await WaitNextClickAsync(token);
            }

            // Fermer le tuto
            HideAllPages();
            if (uiTuto) uiTuto.SetActive(false);
        }
        finally
        {
            nextButton.onClick.RemoveListener(OnNextClicked);
            _clickTcs = null;
            _isRunning = false;
        }
    }

    // ====== Helpers ======

    private void OnNextClicked()
    {
        _clickTcs?.TrySetResult(true);
    }

    private Task WaitNextClickAsync(CancellationToken token)
    {
        _clickTcs = new TaskCompletionSource<bool>();
        var reg = token.Register(() => _clickTcs.TrySetCanceled());
        return _clickTcs.Task.ContinueWith(t =>
        {
            reg.Dispose();
            return t;
        }, TaskScheduler.Current).Unwrap();
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
