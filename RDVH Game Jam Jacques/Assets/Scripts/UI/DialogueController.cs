using System.Collections;
using System.Collections.Generic;
using System.Threading; // pour CancellationToken
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [Header("UI refs")]
    [SerializeField] private GameObject bubbleRoot;      // Parent de la bulle (à activer/masquer)
    [SerializeField] private CanvasGroup bubbleGroup;     // Optionnel : pour un fade propre
    [SerializeField] private TextMeshProUGUI textUI;      // Le texte TMP
    [SerializeField] private Button nextButton;           // Bouton pour avancer / skip
    [SerializeField] private AudioManager AudioManager;   // Pour jouer le son d'écriture

    [Header("Typewriter")]
    [SerializeField] private float charsPerSecond = 40f;
    [SerializeField] private bool useUnscaledTime = false;
    [SerializeField] private string punctuation = ".,;:!?…";
    [SerializeField] private float punctuationExtraDelay = 0.25f;

    [Header("Animation")]
    [SerializeField] private Animator bubbleAnimator;
    [SerializeField] private string stateEnter = "Enter";
    [SerializeField] private string stateIdle = "Idle";
    [SerializeField] private string stateExit = "Exit";
    [SerializeField] private string triggerShow = "Enter";
    [SerializeField] private string triggerHide = "Exit";

    // État interne
    private bool _typing;               // vrai pendant la frappe
    private bool _skipRequested;        // demande de skip via bouton
    private bool _advanceRequested;     // clic "next" pour avancer
    private bool _cancelRequested;      // annulation locale
    private CancellationToken _externalToken;

    void Awake()
    {
        if (bubbleRoot != null) bubbleRoot.SetActive(false);
        if (textUI != null) textUI.text = string.Empty;
    }

    void OnDisable()
    {
        // annule en cas de désactivation du GO
        _cancelRequested = true;
    }

    /// <summary>
    /// Lance un dialogue et ne rend la main qu’une fois terminé (coroutine).
    /// </summary>
    public IEnumerator RunDialog(IList<string> lines, CancellationToken externalToken = default)
    {
        if (lines == null || lines.Count == 0) yield break;

        // reset et setup
        _cancelRequested = false;
        _externalToken = externalToken;

        nextButton.onClick.AddListener(OnNextClicked);
        if (AudioManager != null && AudioManager.writeSource != null)
            AudioManager.writeSource.Play();

        // montrer la bulle
        yield return ShowBubble(true, externalToken);
        if (IsCancelled()) goto Cleanup;

        // chaque ligne : typewriter puis attendre clic "next"
        for (int i = 0; i < lines.Count; i++)
        {
            if (IsCancelled()) break;

            yield return TypeLine(lines[i], externalToken);
            if (IsCancelled()) break;

            yield return WaitAdvanceClick(externalToken);
        }

        if (!IsCancelled())
            yield return ShowBubble(false, externalToken);

        Cleanup:
        if (AudioManager != null && AudioManager.writeSource != null)
            AudioManager.writeSource.Stop();

        nextButton.onClick.RemoveListener(OnNextClicked);
        _typing = false;
        _skipRequested = false;
        _advanceRequested = false;

        if (textUI) textUI.text = string.Empty;
    }

    private bool IsCancelled()
        => _cancelRequested || (_externalToken.CanBeCanceled && _externalToken.IsCancellationRequested);

    // ====== Implémentation ======

    private void OnNextClicked()
    {
        AudioManager.Play("uiButtonClick");
        if (_typing)
        {
            // En cours de frappe -> on demande un skip
            _skipRequested = true;
        }
        else
        {
            // Ligne finie -> on avance
            _advanceRequested = true;
        }
    }

    public IEnumerator WaitAdvanceClick(CancellationToken token)
    {
        _advanceRequested = false;
        while (!_advanceRequested)
        {
            if (token.IsCancellationRequested) yield break;
            if (_cancelRequested) yield break;
            yield return null;
        }
    }

    public IEnumerator TypeLine(string line, CancellationToken token)
    {
        if (!textUI) yield break;

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
            if (token.IsCancellationRequested || _cancelRequested) { _typing = false; yield break; }

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
                    var ci = textUI.textInfo.characterInfo[visible - 1];
                    char c = ci.character;
                    if (punctuation.IndexOf(c) >= 0)
                        yield return WaitSeconds(punctuationExtraDelay, token);
                }
            }

            yield return null; // prochaine frame
        }

        // Si on a demandé "skip", révéler tout d'un coup
        textUI.maxVisibleCharacters = total;

        _typing = false;
    }

    public IEnumerator ShowBubble(bool show, CancellationToken token)
    {
        // Fallback simple si rien pour le fade
        if (bubbleRoot == null && bubbleGroup == null)
        {
            if (bubbleRoot != null) bubbleRoot.SetActive(show);
            yield break;
        }

        if (show)
        {
            if (bubbleRoot != null) yield return PlayEnter(token);

        }

        float duration = 0.15f;
        float start = bubbleGroup ? bubbleGroup.alpha : (show ? 0f : 1f);
        float end = show ? 1f : 0f;

        float t = 0f;
        while (t < duration)
        {
            if (token.IsCancellationRequested || _cancelRequested) yield break;

            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            if (bubbleGroup) bubbleGroup.alpha = Mathf.Lerp(start, end, k);
            yield return null;
        }

        if (bubbleGroup) bubbleGroup.alpha = end;
        if (!show && bubbleRoot != null) yield return PlayExit(token);

    }

    public IEnumerator WaitSeconds(float seconds, CancellationToken token)
    {
        float t = 0f;
        while (t < seconds)
        {
            if (token.IsCancellationRequested || _cancelRequested) yield break;
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator PlayEnter(CancellationToken token)
    {
        if (bubbleRoot && !bubbleRoot.activeSelf) bubbleRoot.SetActive(true);
        if (bubbleGroup) { bubbleGroup.interactable = false; bubbleGroup.blocksRaycasts = false; }

        bubbleAnimator.ResetTrigger(triggerHide);
        bubbleAnimator.SetTrigger(triggerShow);

        // Attendre qu’on entre dans Enter puis sa fin, puis qu’on arrive en Idle
        yield return AnimationHelper.WaitForState(bubbleAnimator, 0, stateEnter, token);
        yield return AnimationHelper.WaitForStateEnd(bubbleAnimator, 0, stateEnter, token);
        yield return AnimationHelper.WaitForState(bubbleAnimator, 0, stateIdle, token);

        if (bubbleGroup) { bubbleGroup.interactable = true; bubbleGroup.blocksRaycasts = true; }
    }

    private IEnumerator PlayExit(CancellationToken token)
    {
        if (bubbleGroup) { bubbleGroup.interactable = false; bubbleGroup.blocksRaycasts = false; }

        bubbleAnimator.ResetTrigger(triggerShow);
        bubbleAnimator.SetTrigger(triggerHide);

        // Attendre qu’on entre dans Exit puis sa fin (Exit -> Hidden auto dans le controller)
        yield return AnimationHelper.WaitForState(bubbleAnimator, 0, stateExit, token);
        yield return AnimationHelper.WaitForStateEnd(bubbleAnimator, 0, stateExit, token);

        if (bubbleRoot) bubbleRoot.SetActive(false);

        Debug.Log("Dialog Exit");
    }
}
