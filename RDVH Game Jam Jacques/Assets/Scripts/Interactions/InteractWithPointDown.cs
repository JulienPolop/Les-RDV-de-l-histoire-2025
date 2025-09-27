using UnityEngine;

public class InteractWithPointDown : InteractWithPoint
{
    [SerializeField] private Transform hooveredMovingTransform;
    [SerializeField] private float downDistance;
    protected override void Start()
    {
        base.Start(); // This will invoke your overridden Action

        // Default Value filled with default animation
        OnHoverStart = () =>
        {
            hooveredMovingTransform.transform.localPosition = Vector3.down * downDistance;
        };

        OnHoverEnd = () =>
        {
            hooveredMovingTransform.transform.localPosition = Vector3.zero;
        };
    }
}
