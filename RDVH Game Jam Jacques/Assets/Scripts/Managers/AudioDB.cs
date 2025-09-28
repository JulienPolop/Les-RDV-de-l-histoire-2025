using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDB", menuName = "ScriptableObjects/AudioDB", order = 1)]
public class AudioDB : ScriptableObject
{
    public List<AudioEvent> Events;
}


[Serializable] public class AudioEvent
{
    public enum PickType {Random, First, Sequence}
    public string ID;
    public List<AudioClip> clips;
    public PickType Type;
}
