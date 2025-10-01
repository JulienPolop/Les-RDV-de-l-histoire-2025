using System.Collections;
using System.Threading;
using UnityEngine;

public class AnimationHelper
{
    public static IEnumerator WaitForState(Animator anim, int layer, string stateName, CancellationToken token)
    {
        int targetHash = Animator.StringToHash(stateName);
        while (true)
        {
            if ((token.CanBeCanceled && token.IsCancellationRequested)) yield break;
            var st = anim.GetCurrentAnimatorStateInfo(layer);
            if (st.shortNameHash == targetHash) yield break;
            yield return null;
        }
    }

    public static IEnumerator WaitForStateEnd(Animator anim, int layer, string stateName, CancellationToken token)
    {
        int targetHash = Animator.StringToHash(stateName);
        while (true)
        {
            if ((token.CanBeCanceled && token.IsCancellationRequested)) yield break;

            var st = anim.GetCurrentAnimatorStateInfo(layer);

            // Tant qu’on est dans le state et pas en transition, on attend sa fin (normalizedTime >= 1)
            if (st.shortNameHash == targetHash && !anim.IsInTransition(layer) && st.normalizedTime >= 1f)
                yield break;

            // Si on a quitté le state (transition terminée), on considère fini aussi
            if (st.shortNameHash != targetHash && !anim.IsInTransition(layer))
                yield break;

            yield return null;
        }
    }
}
