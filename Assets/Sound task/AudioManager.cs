using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] AudioSource _bg;
    [SerializeField] AudioSource _click;
    [SerializeField] AudioSource _color;
    [SerializeField] AudioSource _win;
    public void PlayMusic()
    {
        if(!_bg.isPlaying)
            _bg.Play();
    }
    public void StopMusic()
    {
        _bg.Stop();
    }
    public void PlayClick()
    {
        _click.Play();
    }
    public void PlayColorDone()
    {
        _color.Play();
    }
    public void PlayWinSound()
    {
        _win.Play();
    }

}
