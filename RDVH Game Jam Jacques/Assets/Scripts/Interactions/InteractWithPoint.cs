using System;
using UnityEngine;

public class InteractWithPoint : MonoBehaviour
{
    public Action OnHoverStart;
    public Action OnHoverEnd;
    public Action OnClick;

    protected virtual void Start(){}
}
