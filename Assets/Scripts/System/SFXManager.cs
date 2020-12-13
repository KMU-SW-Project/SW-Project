using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioClip))]
public class SFXManager : MonoBehaviour
{
    public AudioClip[] vfxs;
    AudioSource audioSource;
   
    private static SFXManager instance = null;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static SFXManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public void PlaySFX(vfx audio)
    {

        switch (audio)
        {
            case vfx.Ready:
                audioSource.clip = vfxs[0];
                audioSource.Play();
                break;
            case vfx.Stady:
                audioSource.clip = vfxs[1];
                audioSource.Play();
                break;
            case vfx.Bang:
                audioSource.clip = vfxs[2];
                audioSource.Play();
                break;
            case vfx.Victory:
                audioSource.clip = vfxs[3];
                audioSource.Play();
                break;
            case vfx.Lose:
                audioSource.clip = vfxs[4];
                audioSource.Play();
                break;
        }
    }
}

public enum vfx
{
    Ready,
    Stady,
    Bang,
    Victory,
    Lose
}