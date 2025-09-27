using Unity.VisualScripting;
using UnityEngine;

public class InteracDebugNextStep : MonoBehaviour
{
    public InteractWithPoint Interractor;
    public Main MainManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Interractor.OnClick = () =>
        {
            Debug.Log("CLICK ON NEXT STEP");
            MainManager.NextStep();
        };
    }
}
