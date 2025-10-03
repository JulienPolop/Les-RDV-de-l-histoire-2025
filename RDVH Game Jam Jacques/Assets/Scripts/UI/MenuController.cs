using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Refs")]
    public Button playButton;
    public Image Logo;
    public Main MainController;

    [Header("Hover Scale Settings")]
    [SerializeField] private float hoverScaleMultiplier = 1.1f;
    [SerializeField] private float scaleDuration = 0.12f;
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private AnimationCurve scaleEase = null;

    [Header("Intro Fade")]
    [SerializeField] private float logoFadeDuration = 2f;
    [SerializeField] private float buttonFadeDuration = 2f;

    private Vector3 initialPlayButtonScale;
    private Coroutine scaleRoutine;

    private void Awake()
    {
        if (scaleEase == null) scaleEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        Debug.Log("Awake");
        initialPlayButtonScale = playButton.transform.localScale;

        // Prépare alpha à 0 au démarrage
        GetOrAddCanvasGroup(Logo.gameObject).alpha = 0f;
        GetOrAddCanvasGroup(playButton.gameObject).alpha = 0f;
        playButton.interactable = false;
    }

    private void Start()
    {
        // Séquence d'intro : fade logo puis bouton
        StartCoroutine(IntroSequence());
    }

    private void OnDisable()
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = null;
        if (playButton) playButton.transform.localScale = initialPlayButtonScale;
    }

    public void Play()
    {
        StartCoroutine(ClickBounceAndPlay());
        //MainController.FirstStep();
        //SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PointerEnterPlay()
    {
        AnimatePlayButtonScale(initialPlayButtonScale * hoverScaleMultiplier);
    }

    public void PointerExitPlay()
    {
        AnimatePlayButtonScale(initialPlayButtonScale);
    }

    // ---------- Intro Fade sequence ----------
    private IEnumerator IntroSequence()
    {
        var logoGroup = GetOrAddCanvasGroup(Logo.gameObject);
        var buttonGroup = GetOrAddCanvasGroup(playButton.gameObject);

        var a = StartCoroutine(FadeCanvasGroup(logoGroup, 0f, 1f, logoFadeDuration, useUnscaledTime));
        var b = StartCoroutine(FadeCanvasGroup(buttonGroup, 0f, 1f, buttonFadeDuration, useUnscaledTime));
        yield return a; yield return b;
        playButton.interactable = true;
    }

    private static CanvasGroup GetOrAddCanvasGroup(GameObject go)
    {
        var cg = go.GetComponent<CanvasGroup>();
        if (!cg) cg = go.AddComponent<CanvasGroup>();
        return cg;
    }

    private static IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration, bool unscaled)
    {
        float t = 0f;
        cg.alpha = from;

        while (t < duration)
        {
            t += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            float k = duration > 0f ? Mathf.Clamp01(t / duration) : 1f;
            cg.alpha = Mathf.Lerp(from, to, k);
            yield return null;
        }

        cg.alpha = to;
    }

    // ---------- Anim helpers ----------
    private void AnimatePlayButtonScale(Vector3 targetScale)
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleToRoutine(playButton.transform, targetScale, scaleDuration, scaleEase, useUnscaledTime));
    }

    private static IEnumerator ScaleToRoutine(Transform t, Vector3 target, float duration, AnimationCurve ease, bool unscaled)
    {
        if (!t) yield break;

        Vector3 start = t.localScale;
        float time = 0f;

        if ((start - target).sqrMagnitude <= 0.000001f)
        {
            t.localScale = target;
            yield break;
        }

        while (time < duration)
        {
            time += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            float k = duration > 0f ? Mathf.Clamp01(time / duration) : 1f;
            float e = ease != null ? ease.Evaluate(k) : k;

            t.localScale = Vector3.LerpUnclamped(start, target, e);
            yield return null;
        }

        t.localScale = target;
    }

    private IEnumerator ClickBounceAndPlay()
    {
        Transform t = playButton.transform;
        Vector3 start = initialPlayButtonScale * hoverScaleMultiplier;
        Vector3 up = start * 1.1f;
        float d = 0.12f;

        yield return ScaleToRoutine(t, up, d, AnimationCurve.EaseInOut(0, 0, 1, 1), true);
        yield return ScaleToRoutine(t, start, d, AnimationCurve.EaseInOut(0, 0, 1, 1), true);

        MainController.FirstStep();
    }
}
