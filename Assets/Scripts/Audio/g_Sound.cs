using UnityEngine;

[System.Serializable]
public class g_Sound
{
    public string name;
    public AudioClip clip;
    public float volume;
    public bool loop;
    [HideInInspector]
    public AudioSource source;
}
