using UnityEngine;

public interface IAudioManager
{
    void PlaySFX(string name);
    void PlaySFX(string name, AudioSource audioSource);
    void PlaySFX(string name, float delay);
    void StopSFX();
    void StopSFX(AudioSource audioSource);
    void PlayCharacterSFX(string name, float delay = 0f);
    void StopCharacterSFX();
    void PlayFadedBGM(string name, float fadeDuration);
    void StopFadedBGM(float targetVolume, float fadeDuration);
    float AudioClipLength(string name);
    void CreateEnqueuedPowerUpSFX(string name, float duration, UnityEngine.UI.Image powerUpImage, bool lerpPitch = false);
    void StopEnqueuedPowerUpSFX();
    void LerpPitch(string name, float lerpFactor);
}