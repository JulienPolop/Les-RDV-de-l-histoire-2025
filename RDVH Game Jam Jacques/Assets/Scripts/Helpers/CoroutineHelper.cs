using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineHelper
{
    // Attendre que TOUT soit fini (�quivalent Task.WhenAll)
    public static IEnumerator WaitAll(MonoBehaviour owner, List<IEnumerator> routines)
    {
        if (owner == null || routines == null || routines.Count == 0)
            yield break;

        int remaining = routines.Count;

        // D�marrer toutes sans bloquer
        for (int i = 0; i < routines.Count; i++)
            owner.StartCoroutine(RunAndFlag(owner, routines[i], () => remaining--));

        // Attendre que tout soit termin�
        while (remaining > 0)
            yield return null;
    }

    // Attendre que l�UNE d�entre elles finisse (�quivalent Task.WhenAny)
    public static IEnumerator WaitAny(MonoBehaviour owner, params IEnumerator[] routines)
    {
        if (owner == null || routines == null || routines.Length == 0)
            yield break;

        bool done = false;

        for (int i = 0; i < routines.Length; i++)
            owner.StartCoroutine(RunAndFlag(owner, routines[i], () => done = true));

        while (!done)
            yield return null;
    }

    // Encha�ner s�quentiellement (pipeline)
    public static IEnumerator Sequence(MonoBehaviour owner, params IEnumerator[] routines)
    {
        if (owner == null || routines == null) yield break;
        foreach (var r in routines)
            if (r != null) yield return owner.StartCoroutine(r);
    }

    // Petit utilitaire d�lai
    public static IEnumerator Delay(float seconds, bool unscaled = false)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }
    }

    // ---------- privates ----------
    private static IEnumerator RunAndFlag(MonoBehaviour owner, IEnumerator routine, Action onDone)
    {
        if (routine != null)
            yield return owner.StartCoroutine(routine);
        onDone?.Invoke();
    }
}