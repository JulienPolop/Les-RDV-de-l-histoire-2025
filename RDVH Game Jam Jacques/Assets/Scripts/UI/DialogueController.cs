using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [Header("UI refs")]
    [SerializeField] private GameObject bubbleRoot;      // Parent de la bulle (à activer/masquer)
    [SerializeField] private CanvasGroup bubbleGroup;     // Optionnel : pour un fade propre
    [SerializeField] private TextMeshProUGUI textUI;      // Le texte TMP
    [SerializeField] private Button nextButton;           // Bouton pour avancer / skip
    [SerializeField] private AudioManager AudioManager;         // Pour jouer le son d'écriture

    [Header("Typewriter")]
    [SerializeField] private float charsPerSecond = 40f;
    [SerializeField] private bool useUnscaledTime = false;
    [SerializeField] private string punctuation = ".,;:!?…";
    [SerializeField] private float punctuationExtraDelay = 0.25f;

    // État interne
    private bool _typing;                      // vrai pendant la frappe
    private bool _skipRequested;               // demande de skip via bouton
    private TaskCompletionSource<bool> _advanceTcs; // completé quand on clique "suivant" pour avancer
    private CancellationTokenSource _runCts;   // pour annuler proprement si on relance un dialogue

    void Awake()
    {
        if (bubbleRoot != null) bubbleRoot.SetActive(false);
        if (textUI != null) textUI.text = string.Empty;
    }

    void OnDisable()
    {
        // si un dialogue tourne et que l'objet est désactivé -> annuler
        _runCts?.Cancel();
    }

    /// <summary>
    /// Lance un dialogue et ne rend la main qu’une fois terminé.
    /// </summary>
    public async Task RunAsync(IList<string> lines, CancellationToken externalToken = default)
    {
        if (lines == null || lines.Count == 0) return;

        _runCts?.Cancel();
        _runCts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
        var token = _runCts.Token;

        nextButton.onClick.AddListener(OnNextClicked);

        try
        {
            AudioManager.writeSource.Play();
            await ShowBubbleAsync(true, token);

            for (int i = 0; i < lines.Count; i++)
            {
                token.ThrowIfCancellationRequested();

                // 1) Frappe lettre par lettre (clic = skip)
                await TypeLineAsync(lines[i], token);

                // 2) Attendre un clic "Next" pour continuer
                //    (même sur la DERNIÈRE ligne)
                await WaitAdvanceClickAsync(token);
            }

            // Après le dernier clic, on peut cacher la bulle
            await ShowBubbleAsync(false, token);
        }
        finally
        {
            AudioManager.writeSource.Stop();
            nextButton.onClick.RemoveListener(OnNextClicked);
            _advanceTcs = null;
            _typing = false;
            _skipRequested = false;
            if (textUI) textUI.text = string.Empty;
        }
    }

    private Task WaitAdvanceClickAsync(CancellationToken token)
    {
        _advanceTcs = new TaskCompletionSource<bool>();
        // Relie l’annulation à la TCS
        var reg = token.Register(() => _advanceTcs.TrySetCanceled());

        // Important : on détache l’enregistrement quand la Task se termine
        return _advanceTcs.Task.ContinueWith(t =>
        {
            reg.Dispose();
            return t; // on propage le même Task (status/exception)
        }, TaskScheduler.Current).Unwrap();
    }

    // ====== Implémentation ======

    private void OnNextClicked()
    {
        if (_typing)
        {
            // En cours de frappe -> on demande un skip
            _skipRequested = true;
        }
        else
        {
            // Ligne finie -> on avance
            _advanceTcs?.TrySetResult(true);
        }
    }

    private async Task TypeLineAsync(string line, CancellationToken token)
    {
        if (!textUI) return;

        _typing = true;
        _skipRequested = false;

        textUI.text = line;
        textUI.maxVisibleCharacters = 0;
        textUI.ForceMeshUpdate();

        int total = textUI.textInfo.characterCount;
        int visible = 0;

        float perChar = 1f / Mathf.Max(1f, charsPerSecond);
        float timer = 0f;

        while (visible < total && !_skipRequested)
        {
            token.ThrowIfCancellationRequested();

            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            timer += dt;

            if (timer >= perChar)
            {
                timer -= perChar;

                visible = Mathf.Min(visible + 1, total);
                textUI.maxVisibleCharacters = visible;

                // Petite pause sur la ponctuation (optionnel)
                if (visible > 0)
                {
                    char c = textUI.textInfo.characterInfo[visible - 1].character;
                    if (punctuation.IndexOf(c) >= 0)
                        await WaitSecondsAsync(punctuationExtraDelay, token);
                }
            }

            await Task.Yield();
        }

        // Si on a demandé "skip", révéler tout d'un coup
        textUI.maxVisibleCharacters = total;

        _typing = false;
    }

    private async Task ShowBubbleAsync(bool show, CancellationToken token)
    {
        if (!bubbleRoot && !bubbleGroup)
        {
            // fallback simple
            if (bubbleRoot) bubbleRoot.SetActive(show);
            return;
        }

        if (show)
        {
            if (bubbleRoot) bubbleRoot.SetActive(true);
        }

        float duration = 0.15f;
        float start = bubbleGroup ? bubbleGroup.alpha : (show ? 0f : 1f);
        float end = show ? 1f : 0f;

        float t = 0f;
        while (t < duration)
        {
            token.ThrowIfCancellationRequested();
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            if (bubbleGroup) bubbleGroup.alpha = Mathf.Lerp(start, end, k);
            await Task.Yield();
        }

        if (bubbleGroup) bubbleGroup.alpha = end;
        if (!show && bubbleRoot) bubbleRoot.SetActive(false);
    }

    private async Task WaitSecondsAsync(float seconds, CancellationToken token)
    {
        float t = 0f;
        while (t < seconds)
        {
            token.ThrowIfCancellationRequested();
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            await Task.Yield();
        }
    }
}