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
    [SerializeField] private TextMeshProUGUI textComponent;

    public void Start()
    {
        interactor.OnHoverStart = () => this.animator.SetBool("hover", true); 
        interactor.OnHoverEnd = () => this.animator.SetBool("hover", false);
    }

    public void Set(CardData pickedCardData)
    {
        data = pickedCardData;
        textComponent.text = pickedCardData.Title;
    }

    public void Randomizeidle()
    {
        // Pick a random normalized time (0 = start, 1 = end)
        float randomOffset = UnityEngine.Random.value;

        // Force the idle state to play starting at that random point
        this.animator.Play("Idle", 0, randomOffset);
    }

    public void Depop()
    {
        this.interactor.Interactable = false;
        this.animator.SetTrigger("depop");
        Destroy(this.gameObject, config.DESTROY_DELAY);
    }

    public void Pop()
    {
        this.animator.SetTrigger("pop");
    }

    public void Validate()
    {
        this.animator.SetTrigger("validate");
    }

    public void Wrong()
    {
        this.animator.SetTrigger("wrong");
    }

    public void Attack()
    {
        this.animator.SetTrigger("attack");
    }
}
