using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDescriptor : MonoBehaviour
{
    [SerializeField] private CardDescriptorConfig config;
    [Space(20)]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image image;
    [SerializeField] private RectTransform bumpedTransform;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;


    private List<Card> cards = new();

    public void Describe(CardData data)
    {
        canvas.gameObject.SetActive(true);
        this.StartCoroutine(this.Bump());

        image.sprite = data.DescriptionImage;
        titleText.text = data.Title;
        descriptionText.text = data.Description;
    }

    public void Hide()
    {
        canvas.gameObject.SetActive(false);
    }

    private IEnumerator Bump()
    {
        float time = 0f;
        Vector3 initialPosition = Vector3.zero;
        Vector3 maxPosition = config.BUMP_OFFSET * Vector3.up;
        while (time < config.BUMP_DURATION)
        {
            time += Time.deltaTime;
            float timeRatio = time / config.BUMP_DURATION;
            this.bumpedTransform.anchoredPosition = Vector3.LerpUnclamped(initialPosition, maxPosition, config.BUMP_OFFSET_CURVE.Evaluate(timeRatio));
            yield return null;
        }

        this.bumpedTransform.anchoredPosition = initialPosition;
    }

    private void Update()
    {
        // Animate image
        float ratio = (Time.time % config.IMAGE_DURATION)/config.IMAGE_DURATION;
        float degree = config.IMAGE_ROTATION_CURVE.Evaluate(ratio) * config.IMAGE_ROTATION_AMPLITUDE;
        this.image.transform.localRotation = Quaternion.Euler(0f,degree, 0f);
    }
}