using System;
using System.Threading.Tasks;
using TMPro;
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

    public async Task Aspire()
    {
        Vector3 initialPosition = this.card.transform.position;
        Vector3 finalPosition = this.transform.position;

        this.card.Validate();

        await Task.Delay(TimeSpan.FromSeconds(config.ASPIRE_DELAY));
        float initialTime = Time.time;
        float animationRatio = 0f;
        while (animationRatio < 1f)
        {
            Vector3 currentposition = Vector3.Lerp(initialPosition, finalPosition, config.aspireCurve.Evaluate(animationRatio));
            this.card.transform.position = currentposition;
            await Task.Delay(10);
            animationRatio = (Time.time - initialTime) / config.ASPIRE_DURATION;
        }

        this.card.transform.position = finalPosition;
        this.image.enabled = false; 
        await Task.Delay(TimeSpan.FromSeconds(config.ASPIRE_ENDPAUSE));
    }

    public void Pop()
    {
        Color color = this.image.color;
        color.a = 0f;
        this.image.color = color;
        //TODO Animate
        Color colorEnd = this.image.color;
        colorEnd.a = 1f;
        this.image.color = colorEnd;
    }

    public void Attack()
    {
        if(!IsEmpty)Card.Attack();
        GameObject.Destroy(this.gameObject, config.DESTROY_DELAY);
    }

    public Image image;
}