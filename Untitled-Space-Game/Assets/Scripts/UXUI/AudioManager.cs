using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioSource _deathMusic;
    [SerializeField] AudioSource[] _inGameMusic;

    [SerializeField] int _musicIndex;

    [Header("Stats")]
    [SerializeField] bool _isPlayingMusic;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _inGameMusic[_musicIndex].Play();
    }

    void Update()
    {
        if (_inGameMusic[_musicIndex].isPlaying == false)
        {
            SkipSong();
        }

        if (_deathMusic.isPlaying && _isPlayingMusic)
        {
            _inGameMusic[_musicIndex].Pause();
            _isPlayingMusic = false;
        }
        else if (!_deathMusic.isPlaying && !_isPlayingMusic)
        {
            _deathMusic.Stop();
            _inGameMusic[_musicIndex].Play();
            _isPlayingMusic = true;
        }
    }

    public void SkipSong()
    {
        _inGameMusic[_musicIndex].Stop();

        if (_musicIndex == _inGameMusic.Length)
        {
            _musicIndex = 0;
        }
        else
        {
            _musicIndex++;
        }

        _inGameMusic[_musicIndex].Play();

    }


}
