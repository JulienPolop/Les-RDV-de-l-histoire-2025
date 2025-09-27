using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;


[CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig", order = 1)]
public class LevelConfig : ScriptableObject
{
    public string Title;
    public Goal Goal;
    public TimelineClip playableClip;
}

[Serializable]
public class Goal
{
    public List<CardFlag> Flags;
}
