using UnityEngine;
using System;

public class GameStateTransitionManager : MonoBehaviour
{
    public static Action OnFadeOff;
    public static Action OnFadeInStart;
    public static Action OnFadeInEnd;
    public static Action OnFadeOutStart;
    public static Action OnFadeOutEnd;

    private static Animator _animator;

    private const string FADE_IN = "Fade_In";
    private const string FADE_OUT = "Fade_Out";
    private const string FADE_OFF = "Fade_Off";

    public void Initialize()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
    }

    public static void FadeIn()
    {
        _animator.Play(FADE_IN);
    }
    public void FadeInStarted()
    {
        OnFadeInStart?.Invoke();
        OnFadeInStart = null;
    }
    public void FadeInEnded()
    {
        OnFadeInEnd?.Invoke();
        OnFadeInEnd = null;
    }
    public static void FadeOut()
    {
        _animator.Play(FADE_OUT);
    }
    public void FadeOutStarted()
    {
        OnFadeOutStart?.Invoke();
        OnFadeOutStart = null;
    }
    public void FadeOutEnded()
    {
        OnFadeOutEnd?.Invoke();
        OnFadeOutEnd = null;
    }
    public static void FadeOff()
    {
        _animator.Play(FADE_OFF);
        OnFadeOff?.Invoke();
        OnFadeOff = null;
    }
}
