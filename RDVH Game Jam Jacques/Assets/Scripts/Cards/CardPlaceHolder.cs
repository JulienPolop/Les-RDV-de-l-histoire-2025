using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardPlaceHolder : MonoBehaviour
{
    [SerializeField] private CardPlaceHolderConfig config;

    public Card Card => card;
    public CardFlag Flag => flag;
    public bool IsEmpty => card == null;

    private CardFlag flag;
    private Card card;

    public Image image;

    public void Set(CardFlag flag)
    {
        this.flag = flag;
        this.image.color = config.FlagMaterial[(int)flag];
    }

    public void Fill(Card card)
    {
        this.card = card;
        this.card.transform.SetParent(this.transform);
        this.card.Interactor.Interactable = false;
    }

    // Était: async Task Aspire()
    public IEnumerator Aspire()
    {
        if (card == null) yield break;

        Vector3 initialPosition = card.transform.position;
        Vector3 finalPosition = transform.position;

        card.Validate();

        // délai avant l’aspiration
        if (config.ASPIRE_DELAY > 0f)
            yield return new WaitForSeconds(config.ASPIRE_DELAY);

        // animation d’aspiration selon la courbe
        float dur = Mathf.Max(0.0001f, config.ASPIRE_DURATION);
        float t = 0f;
        while (t < dur)
        {
            float k = Mathf.Clamp01(t / dur);
            float eased = config.aspireCurve != null ? config.aspireCurve.Evaluate(k) : k;
            card.transform.position = Vector3.Lerp(initialPosition, finalPosition, eased);

            t += Time.deltaTime;
            yield return null;
        }

        card.transform.position = finalPosition;
        image.enabled = false;
        card.Placed();

        if (config.ASPIRE_ENDPAUSE > 0f)
            yield return new WaitForSeconds(config.ASPIRE_ENDPAUSE);
    }

    public void Pop()
    {
        Color color = image.color;
        color.a = 0f;
        image.color = color;

        // TODO: animer si besoin; pour l'instant on “snap” à 1
        color.a = 1f;
        image.color = color;
    }

    // Était: async Task Attack()
    public IEnumerator Attack()
    {
        if (!IsEmpty)
            yield return card.Attack(); // card.Attack() est déjà une coroutine

        if (config.DESTROY_DELAY > 0f)
            yield return new WaitForSeconds(config.DESTROY_DELAY);

    }

    internal IEnumerator Depop()
    {
        if (!IsEmpty)
            yield return card.Depop(); // card.Attack() est déjà une coroutine


        Destroy(gameObject);
    }
}
