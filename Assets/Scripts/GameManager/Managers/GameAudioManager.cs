using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static GameAudioManager;

public class GameAudioManager : MonoBehaviour
{
    #region INTERNAL CLASSES
    [System.Serializable]
    public class GameSounds
    {
        public string _soundName = "";

        public AudioClip _audioClip;

        [Range(0f, 1f)] public float _volume;
        [Range(-3f, 3f)] public float _pitch;
        [Range(0.0f, 0.5f)] public float _pitchVariation;

        public bool _loop;
    }

    [System.Serializable]
    public class PowerUPCounterSFX
    {
        public float _duration;

        public GameSounds _gameSound;

        public AudioSource _audioSource;

        public UnityEngine.UI.Image _powerUpImage;

        public Coroutine _powerUpCoroutine;

        public PowerUPCounterSFX(float duration, AudioSource audioSource)
        {
            this._duration = duration;
            this._audioSource = audioSource;
        }
    }
    #endregion

    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _characterSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private GameSounds[] _gameSounds;

    private List<PowerUPCounterSFX> _powerUpCounterSFX = new List<PowerUPCounterSFX>();

    #region ENQUEUEMENT SOUND MANAGEMENT
    public void CreateEnqueuedPowerUpSFX(string name, float duration, UnityEngine.UI.Image powerUpImage, bool lerpPitch = false)
    {
        GameObject gameObject = new GameObject("TempAudioSource");
        gameObject.transform.parent = transform;

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        PowerUPCounterSFX powerUpCounterSFX = new PowerUPCounterSFX(duration, audioSource);

        powerUpCounterSFX._powerUpImage = powerUpImage;

        if (!string.IsNullOrEmpty(name))
        {
            GameSounds sound = Array.Find(_gameSounds, sound => sound._soundName == name);

            if (sound == null)
            {
                Debug.LogWarning($"Sound {name} not found.");
                return;
            }

            powerUpCounterSFX._gameSound = sound;

            audioSource.clip = sound._audioClip;
            audioSource.volume = sound._volume;
            audioSource.pitch = sound._pitch;
            audioSource.loop = sound._loop;
        }

        _powerUpCounterSFX.Add(powerUpCounterSFX);
        _powerUpCounterSFX.Sort((sfx1, sfx2) => sfx1._duration.CompareTo(sfx2._duration));
        _powerUpCounterSFX.Reverse();

        for (int i = 0; i < _powerUpCounterSFX.Count; i++)
        {
            _powerUpCounterSFX[i]._audioSource.volume = 0;
        }

        audioSource.Play();

        if (lerpPitch)
        {
           powerUpCounterSFX._powerUpCoroutine = StartCoroutine(LerpAudioSource(powerUpCounterSFX));
        }

        _powerUpCounterSFX[_powerUpCounterSFX.Count - 1]._audioSource.volume = _powerUpCounterSFX[_powerUpCounterSFX.Count - 1]._gameSound._volume;

        IEnumerator LerpAudioSource(PowerUPCounterSFX powerCounterSFX)
        {
            while (powerCounterSFX._powerUpImage.fillAmount >= 0.00f)
            {
                if (powerCounterSFX._powerUpImage.fillAmount < 0.4f)
                {
                    float pitchOverride = Mathf.InverseLerp(0.4f, 0.05f, powerCounterSFX._powerUpImage.fillAmount);

                    powerCounterSFX._audioSource.pitch = Mathf.Lerp(powerCounterSFX._gameSound._pitch, powerCounterSFX._gameSound._pitch + powerCounterSFX._gameSound._pitchVariation, pitchOverride);
                }

                yield return null;
            }
        }
    }
    public void StopEnqueuedPowerUpSFX()
    {
        _powerUpCounterSFX[_powerUpCounterSFX.Count - 1]._audioSource.Stop();

        PowerUPCounterSFX powerCounterSFX = _powerUpCounterSFX[_powerUpCounterSFX.Count - 1];

        _powerUpCounterSFX.Remove(powerCounterSFX);

        StopCoroutine(powerCounterSFX._powerUpCoroutine);

        Destroy(powerCounterSFX._audioSource.gameObject);

        if (_powerUpCounterSFX.Count > 0)
        {
            _powerUpCounterSFX[_powerUpCounterSFX.Count - 1]._audioSource.volume = _powerUpCounterSFX[_powerUpCounterSFX.Count - 1]._gameSound._volume;
        }
    }
    #endregion

    #region CHARACTER SOUND MANAGEMENT
    public void PlayCharacterSFX(string name, float delay = 0.00f)
    {
        if (_characterSource.isPlaying)
        {
            return;
        }

        if (!string.IsNullOrEmpty(name))
        {
            GameSounds sound = Array.Find(_gameSounds, sound => sound._soundName == name);

            if (sound == null)
            {
                Debug.LogWarning($"Sound {name} not found.");
                return;
            }

            if (_characterSource.clip != sound._audioClip)
            {
                _characterSource.clip = sound._audioClip;
                _characterSource.volume = sound._volume;
                _characterSource.pitch = sound._pitch;
                _characterSource.loop = sound._loop;
            }

            if (sound._pitchVariation != 0.00f)
            {
                _characterSource.pitch = UnityEngine.Random.Range(sound._pitch - sound._pitchVariation, _characterSource.pitch + sound._pitchVariation);
            }

            if (delay != 0.00f)
            {
                _characterSource.PlayDelayed(delay);
            }
            else
            {
                _characterSource.Play();
            }
        }
    }
    public void StopCharacterSFX()
    {
        _characterSource.Stop();
    }
    #endregion

    #region BGM SOUND MANAGEMENT
    public void PlayBGM(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            GameSounds sound = Array.Find(_gameSounds, sound => sound._soundName == name);

            if (sound == null)
            {
                Debug.LogWarning($"Sound {name} not found.");
                return;
            }

            if (_musicSource.clip != sound._audioClip)
            {
                _musicSource.clip = sound._audioClip;
            }
            _musicSource.volume = sound._volume;
            _musicSource.pitch = sound._pitch;
            _musicSource.loop = sound._loop;
            _musicSource.Play();
        }
    }
    public void PlayFadedBGM(string name, float fadeDuration)
    {
        float targetVolume = 0;

        if (!string.IsNullOrEmpty(name))
        {
            GameSounds sound = Array.Find(_gameSounds, sound => sound._soundName == name);

            if (sound == null)
            {
                Debug.LogWarning($"Sound {name} not found.");
                return;
            }

            if (_musicSource.clip != sound._audioClip)
            {
                _musicSource.clip = sound._audioClip;
            }

            _musicSource.volume = 0;
            _musicSource.pitch = sound._pitch;
            _musicSource.loop = sound._loop;

            targetVolume = sound._volume;
        }

        StartCoroutine(FadeIn(targetVolume, fadeDuration));
    }
    public void StopBGM()
    {
        _musicSource.Stop();
        _musicSource.clip = null;
        _musicSource.loop = false;
    }
    public void StopFadedBGM(float targetVolume, float fadeDuration)
    {
        StartCoroutine(FadeOut(targetVolume, fadeDuration));
    }
    #endregion

    #region SFX SOUND MANAGEMENT
    public void PlaySFX(string name)
    {
        if (_sfxSource.isPlaying)
        {
            return;
        }

        if (!string.IsNullOrEmpty(name))
        {
            GameSounds sound = Array.Find(_gameSounds, sound => sound._soundName == name);

            if (sound == null)
            {
                Debug.LogWarning($"Sound {name} not found.");
                return;
            }

            if (_sfxSource.clip != sound._audioClip)
            {
                _sfxSource.clip = sound._audioClip;
            }
            _sfxSource.volume = sound._volume;
            _sfxSource.pitch = sound._pitch;
            _sfxSource.loop = sound._loop;
            _sfxSource.Play();
        }
    }
    public void PlaySFX(string name, float delay = 0.00f)
    {
        if (_sfxSource.isPlaying)
        {
            return;
        }

        if (!string.IsNullOrEmpty(name))
        {
            GameSounds sound = Array.Find(_gameSounds, sound => sound._soundName == name);

            if (sound == null)
            {
                Debug.LogWarning($"Sound {name} not found.");
                return;
            }

            if (_sfxSource.clip != sound._audioClip)
            {
                _sfxSource.clip = sound._audioClip;
            }
            _sfxSource.volume = sound._volume;
            _sfxSource.pitch = sound._pitch;
            _sfxSource.loop = sound._loop;

            if (delay != 0.00f)
            {
                _sfxSource.PlayDelayed(delay);
            }
            else
            {
                _sfxSource.Play();
            }
        }
    }
    public void PlaySFX(string name, AudioSource audioSource)
    {
        if (audioSource.isPlaying)
        {
            return;
        }

        if (!string.IsNullOrEmpty(name))
        {
            GameSounds sound = Array.Find(_gameSounds, sound => sound._soundName == name);

            if (sound == null)
            {
                Debug.LogWarning($"Sound {name} not found.");
                return;
            }

            if (audioSource.clip != sound._audioClip)
            {
                audioSource.clip = sound._audioClip;
            }
            audioSource.volume = sound._volume;
            audioSource.pitch = sound._pitch;
            audioSource.loop = sound._loop;

            audioSource.Play();
        }
    }
    public void StopSFX()
    {
        _sfxSource.Stop();
        _sfxSource.clip = null;
        _sfxSource.loop = false;
    }
    public void StopSFX(AudioSource audioSource)
    {
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
    }
    #endregion

    #region SOUND UTILS
    public float AudioClipLength(string name)
    {
        float length = 0.00f;

        if (!string.IsNullOrEmpty(name))
        {
            GameSounds sound = Array.Find(_gameSounds, sound => sound._soundName == name);

            if (sound == null)
            {
                Debug.LogWarning($"Sound {name} not found.");
                return length;
            }

            length = sound._audioClip.length;
        }

        return length;
    }
    public void LerpPitch(string name, float lerpFactor = 0.00f)
    {
        if (!string.IsNullOrEmpty(name))
        {
            GameSounds sound = Array.Find(_gameSounds, sound => sound._soundName == name);

            if (sound == null)
            {
                Debug.LogWarning($"Sound {name} not found.");
                return;
            }

            _sfxSource.pitch = Mathf.Lerp(sound._pitch, sound._pitch + sound._pitchVariation, lerpFactor);
        }
    }
    IEnumerator FadeIn(float targetVolume, float fadeDuration)
    {
        _musicSource.Play();

        float currentTime = 0;
        float startVolume = _musicSource.volume;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / fadeDuration);
            yield return null;
        }
        _musicSource.volume = targetVolume;
    }
    IEnumerator FadeOut(float targetVolume, float fadeDuration)
    {
        float currentTime = 0;
        float startVolume = _musicSource.volume;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / fadeDuration);
            yield return null;
        }

        _musicSource.volume = targetVolume;
        _musicSource.Stop();
    }
    #endregion
}
