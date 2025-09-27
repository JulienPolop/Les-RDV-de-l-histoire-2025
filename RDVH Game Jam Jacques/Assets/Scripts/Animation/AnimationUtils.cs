using System;
using UnityEngine;

public class AnimationUtils
{


    public async void MoveFrom(Transform transform, Vector3 start, Func<float,float> easing) => Move(transform, start, transform.position,easing);
    public async void MoveTo(Transform transform, Vector3 end, Func<float, float> easing) => Move(transform, transform.position, end, easing);
    public async void Move(Transform transform, Vector3 start, Vector3 end, Func<float, float> easing)
    {
        float durationRatio = 0;
        /*transform.
        while ()
        {
            durationRatio = 
        }*/
    }
}