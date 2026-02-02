using UnityEngine;
using UnityEngine.Events;

public class GameStateTransitionManager : MonoBehaviour
{
    public static UnityEvent OnFadeOff = new UnityEvent();
    public static UnityEvent OnFadeInStart = new UnityEvent();
    public static UnityEvent OnFadeInEnd = new UnityEvent();
    public static UnityEvent OnFadeOutStart = new UnityEvent();
    public static UnityEvent OnFadeOutEnd = new UnityEvent();

    private static Animator _animator;

    private const string FADE_IN = "Fade_In";
    private const string FADE_OUT = "Fade_Out";
    private const string FADE_OFF = "Fade_Off";

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public static void FadeIn()
    {
        _animator.Play(FADE_IN);
    }
    public void FadeInStarted()
    {
        OnFadeInStart.Invoke();
        OnFadeInStart.RemoveAllListeners();
    }
    public void FadeInEnded()
    {
        OnFadeInEnd.Invoke();
        OnFadeInEnd.RemoveAllListeners();
    }
    public static void FadeOut()
    {
        _animator.Play(FADE_OUT);
    }
    public void FadeOutStarted()
    {
        OnFadeOutStart.Invoke();
        OnFadeOutStart.RemoveAllListeners();
    }
    public void FadeOutEnded()
    {
        OnFadeOutEnd.Invoke();
        OnFadeOutEnd.RemoveAllListeners();
    }
    public static void FadeOff()
    {
        _animator.Play(FADE_OFF);
        OnFadeOff?.Invoke();
        OnFadeOff.RemoveAllListeners();
    }
}
