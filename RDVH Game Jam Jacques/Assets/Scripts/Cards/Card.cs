using System.Collections;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    public InteractWithPoint Interactor => interactor;

    [SerializeField] private CardConfig config;
    [Space(20)]
    private CardData data;
    public CardData Data => data;
    [SerializeField] private InteractWithPoint interactor;
    [SerializeField] private Animator animator;
    [Space(20)]
    [SerializeField] private Renderer visualRenderer;

    public void OnHoverStart() => this.animator.SetBool("hover", true);
    public void OnHoverEnd() => this.animator.SetBool("hover", false);

    public void Set(CardData pickedCardData)
    {
        data = pickedCardData;

        // Dupliquer les materials pour éviter de modifier les assets partagés
        Material[] newMaterials = new Material[visualRenderer.materials.Length];
        for (int i = 0; i < visualRenderer.materials.Length; i++)
            newMaterials[i] = new Material(visualRenderer.materials[i]);

        visualRenderer.materials = newMaterials;

        if (newMaterials.Length > 1)
            newMaterials[1].mainTexture = pickedCardData.BackTexture;     // bg
        if (newMaterials.Length > 2)
            newMaterials[2].color = pickedCardData.BorderColor;           // border
        if (newMaterials.Length > 3)
            newMaterials[3].mainTexture = pickedCardData.IllustrationTexture; // illustration
        if (newMaterials.Length > 4)
            newMaterials[4].mainTexture = pickedCardData.BandTexture;     // band
    }

    public void Randomizeidle()
    {
        float randomOffset = UnityEngine.Random.value;
        this.animator.Play("Idle", 0, randomOffset);
    }

    // ---- Coroutines (mêmes noms) ----

    public IEnumerator Depop()
    {
        yield return new WaitForSeconds(Random.Range(0f, config.DEPOP_RANDOM_DELAY));

        if (interactor) interactor.Interactable = false;
        this.animator.SetTrigger("depop");

        yield return new WaitForSeconds(config.ANIMATION_DEPOP_WAITING_TIME);

        Destroy(this.gameObject);
    }

    public IEnumerator Pop()
    {
        this.gameObject.SetActive(false);
        yield return new WaitForSeconds(Random.Range(0f, config.POP_RANDOM_DELAY));

        this.gameObject.SetActive(true);
        this.animator.SetTrigger("pop");

        yield return new WaitForSeconds(config.ANIMATION_POP_WAITING_TIME);
    }

    public void Validate()
    {
        AudioManager.Play("cardSelected");
    }

    public void Wrong()
    {
        this.animator.SetTrigger("wrong");
    }

    public IEnumerator Attack()
    {
        //yield return new WaitForSeconds(Random.Range(0f, config.ATTACK_RANDOM_DELAY));

        this.animator.SetTrigger("attack");

        yield return null;

        //yield return new WaitForSeconds(config.ATTACK_SOUND_DELAY);

        //AudioManager.Play("canon");
    }

    public void Placed()
    {
        this.animator.SetTrigger("validate");
        AudioManager.Play("cardPlaced");
    }
}
