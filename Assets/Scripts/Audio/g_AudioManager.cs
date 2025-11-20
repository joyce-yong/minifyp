using System;
using UnityEngine;

public class g_AudioManager : MonoBehaviour
{
    public g_Sound[] sounds;

    void Start()
    {
        foreach (var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
        }

        playSound("theme");
    }

    public void playSound(string soundName)
    {
        g_Sound s = Array.Find(sounds, x => x.name == soundName);

        if (s != null)
        {
            if (s.loop)
            {
                s.source.Play();
            }
            else
            {
                s.source.PlayOneShot(s.clip); 
            }
        }
    }

    public void stopSound(string soundName)
    {
        g_Sound s = Array.Find(sounds, x => x.name == soundName);

        if (s != null)
        {
            s.source.Stop();
        }
    }
}