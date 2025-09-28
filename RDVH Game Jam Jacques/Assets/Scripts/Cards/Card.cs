using System;
using System.Threading.Tasks;
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

                // Duplicate materials so we don’t modify shared assets
        Material[] newMaterials = new Material[visualRenderer.materials.Length];
        for (int i = 0; i < visualRenderer.materials.Length; i++)
        {
            newMaterials[i] = new Material(visualRenderer.materials[i]);
        }

        // Replace with the duplicated array
        visualRenderer.materials = newMaterials;

        // Modify each one based on its index
        if (newMaterials.Length > 0)
        {
            //Don't change the back of card, it is constant
        }
        if (newMaterials.Length > 1)
        {
            newMaterials[1].mainTexture = pickedCardData.BackTexture; // second: change texture of background
        }
        if (newMaterials.Length > 2)
        {
            newMaterials[2].color = pickedCardData.BorderColor;
        }
        if (newMaterials.Length > 3)
        {
            newMaterials[3].mainTexture = pickedCardData.IllustrationTexture;
        }
        if (newMaterials.Length > 4)
        {
            newMaterials[4].mainTexture = pickedCardData.BandTexture;
        }
    }

    public void Randomizeidle()
    {
        // Pick a random normalized time (0 = start, 1 = end)
        float randomOffset = UnityEngine.Random.value;

        // Force the idle state to play starting at that random point
        this.animator.Play("Idle", 0, randomOffset);
    }

    public async Task Depop()
    {
        await Task.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(0f, config.DEPOP_RANDOM_DELAY)));
        this.interactor.Interactable = false;
        this.animator.SetTrigger("depop");
        await Task.Delay(TimeSpan.FromSeconds(config.ANIMATION_DEPOP_WAITING_TIME));
        Destroy(this.gameObject);
    }

    public async Task Pop()
    {
        this.gameObject.SetActive(false);
        await Task.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(0f, config.POP_RANDOM_DELAY)));
        this.gameObject.SetActive(true);
        this.animator.SetTrigger("pop");
        await Task.Delay(TimeSpan.FromSeconds(config.ANIMATION_POP_WAITING_TIME));
    }

    public void Validate()
    {
        AudioManager.Play("cardSelected");
    }

    public void Wrong()
    {
        this.animator.SetTrigger("wrong");
    }

    public async Task Attack()
    {
        await Task.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(0f, config.ATTACK_RANDOM_DELAY)));
        this.animator.SetTrigger("attack");
        await Task.Delay(TimeSpan.FromSeconds(config.ATTACK_SOUND_DELAY));
        AudioManager.Play("canon");
    }

    public void Placed()
    {
        this.animator.SetTrigger("validate");
        AudioManager.Play("cardPlaced");
    }
}
