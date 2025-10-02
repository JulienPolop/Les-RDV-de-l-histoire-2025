using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance; // Access singleton
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource battleSource;
    [SerializeField] public AudioSource writeSource;
    [SerializeField] private AudioSource vfxSource;
    [SerializeField] private AudioDB audioConfig;
    private Dictionary<string, AudioEvent> audioDB;
    private Dictionary<string, int> lastStateSave;

    public void Awake()
    {
        audioDB = audioConfig.Events.ToDictionary(e => e.ID, e => e);
        lastStateSave = new();

        instance = this;

        //if(musicSource!=null)
        //    musicSource.Play();
        //if(battleSource!=null)
        //    battleSource.Play();
    }
    public void Init()
    {

    }

    public static void Play(string id) => instance._Play(id);

    public void _Play(string id)
    {
        AudioEvent evt = audioDB[id];

        // Pick clip
        int oldIndexPlayed = lastStateSave.ContainsKey(id) ? lastStateSave[id] : 0;
        int pickedIndex = 0; //Default
        int eventClipCount = evt.clips.Count;
        switch (evt.Type)
        {
            case AudioEvent.PickType.Random:
                pickedIndex = UnityEngine.Random.Range(0, eventClipCount);
                break;
            case AudioEvent.PickType.First:
                pickedIndex = 0;
                break;
            case AudioEvent.PickType.Sequence:
                pickedIndex = oldIndexPlayed + 1;
                if (pickedIndex >= eventClipCount) pickedIndex = 0;
                break;
        }

        lastStateSave[id] = pickedIndex;
        AudioClip clip = evt.clips[pickedIndex];
        vfxSource.PlayOneShot(clip);
    }
}
