using UnityEngine;


public class Card : MonoBehaviour
{
    private CardData data;
    private InteractWithPoint interactor;
    [SerializeField] private CardConfig config;

    public void Start()
    {

    }
    public void Set(CardData pickedCardData)
    {
        data = pickedCardData;
    }
}

