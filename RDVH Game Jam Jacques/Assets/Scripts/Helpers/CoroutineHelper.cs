using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineHelper
{
    // Attendre que TOUT soit fini (équivalent Task.WhenAll)
    public static IEnumerator WaitAll(MonoBehaviour owner, List<IEnumerator> routines)
    {
        if (owner == null || routines == null || routines.Count == 0)
            yield break;

        int remaining = routines.Count;

        // Démarrer toutes sans bloquer
        for (int i = 0; i < routines.Count; i++)
            owner.StartCoroutine(RunAndFlag(owner, routines[i], () => remaining--));

        // Attendre que tout soit terminé
        while (remaining > 0)
            yield return null;
    }

    // Attendre que l’UNE d’entre elles finisse (équivalent Task.WhenAny)
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

    // Enchaîner séquentiellement (pipeline)
    public static IEnumerator Sequence(MonoBehaviour owner, params IEnumerator[] routines)
    {
        if (owner == null || routines == null) yield break;
        foreach (var r in routines)
            if (r != null) yield return owner.StartCoroutine(r);
    }

    // Petit utilitaire délai
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