using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIManager : MonoBehaviour
{
    [Header("UI Game Objects")]
    [SerializeField] private GameObject _masterScoreIcon;
    [SerializeField] private GameObject _levelScoreIcon;
    [SerializeField] private GameObject _timer;

    [Header("Text Fields")]
    [SerializeField] private Text _timerDisplay;
    [SerializeField] private Text _scoreDisplay;

    [Header("Power Ups")]
    [SerializeField] private Animator _airJumpAnimatorPowerUp;
    [SerializeField] private Image _airJumpUIpowerUp;
    [SerializeField] private Animator _dashAnimatorPowerUp;
    [SerializeField] private Image _dashUIpowerUp;
    [SerializeField] private Animator _wallMoveAnimatorPowerUp;
    [SerializeField] private Image _wallMoveUIpowerUp;

    [Header("Hourglass")]
    [SerializeField] private Transform _hourglassParent;

    public void SetHubUIObjects()
    {
        _masterScoreIcon.SetActive(true);
        _levelScoreIcon.SetActive(false);
        _timer.SetActive(false);
    }
    public void SetLevelUIObjects()
    {
        _masterScoreIcon.SetActive(false);
        _levelScoreIcon.SetActive(true);
        _timer.SetActive(true);
    }
    public void SetTimerDisplay(string time)
    {
        _timerDisplay.text = $"time: {time}";
    }
    public void SetScoreDisplay(int points) 
    {
        _scoreDisplay.text = points.ToString();
    }
    public void SetHourglassDisplay(int count)
    {
        if (count == 0)
        {
            for (int i = 0; i < _hourglassParent.childCount; i++)
            {
                _hourglassParent.GetChild(i).gameObject.SetActive(false);
            }

            return;
        }

        int value = count - 1;

        value = Mathf.Clamp(value, 0, 4);

        _hourglassParent.GetChild(value).gameObject.SetActive(true);
    }
    public void SetAirJumpPowerUpUI(string clip)
    {
        _airJumpAnimatorPowerUp.gameObject.SetActive(true);

        _airJumpAnimatorPowerUp.Play(clip);
        _airJumpUIpowerUp.fillAmount = 1;
    }
    public void SetOvertimeAirJumpPowerUpUI(float value, CharacterContextManager characterContextManager)
    {
        StartCoroutine(FillAmount(value, _airJumpUIpowerUp, _airJumpAnimatorPowerUp, characterContextManager));
    }
    public void SetDashPowerUpUI(string clip)
    {
        _dashAnimatorPowerUp.gameObject.SetActive(true);

        _dashAnimatorPowerUp.Play(clip);
        _dashUIpowerUp.fillAmount = 1;
    }
    public void SetOvertimeDashPowerUpUI(float value, CharacterContextManager characterContextManager)
    {
        StartCoroutine(FillAmount(value, _dashUIpowerUp, _dashAnimatorPowerUp, characterContextManager));
    }
    public void SetWallMovePowerUpUI(string clip)
    {
        _wallMoveAnimatorPowerUp.gameObject.SetActive(true);

        _wallMoveAnimatorPowerUp.Play(clip);
        _wallMoveUIpowerUp.fillAmount = 1;
    }
    public void SetOvertimeWallMovePowerUpUI(float value, CharacterContextManager characterContextManager)
    {
        StartCoroutine(FillAmount(value, _wallMoveUIpowerUp, _wallMoveAnimatorPowerUp, characterContextManager));
    }

    IEnumerator FillAmount(float value, Image sourceImage, Animator animator, CharacterContextManager characterContextManager)
    {
        float currentTime = value;

        characterContextManager.GameContextManager.GameAudioManager.CreateEnqueuedPowerUpSFX("TimeCount", value, sourceImage, true);

        while (currentTime >= 0.00f)
        {
            currentTime -= Time.deltaTime;

            sourceImage.fillAmount = Mathf.Clamp01(currentTime / value);

            yield return null;
        }

        characterContextManager.GameContextManager.GameAudioManager.StopEnqueuedPowerUpSFX();
        characterContextManager.GameContextManager.GameAudioManager.PlaySFX("EndTimeCount");

        System.Action action = () =>
        {
            animator.gameObject.SetActive(false);
            characterContextManager.DispatchPowerUpInteractableRecharge();
        };

        characterContextManager.GameContextManager.WaitSeconds(action, characterContextManager.GameContextManager.GameAudioManager.AudioClipLength("EndTimeCount"));
    }
}
