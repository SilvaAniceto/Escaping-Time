using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    private Animator _characterAnimator;
    public Animator CharacterAnimator { get => _characterAnimator; set => _characterAnimator = value; }

    private const string IDLE_ANIMATION = "Idle";
    private const string RUN_ANIMATION = "Run";
    private const string DASH_ANIMATION = "Dash";
    private const string JUMP_ANIMATION = "Jump";
    private const string FALL_ANIMATION = "Fall";
    private const string ON_WALL_ANIMATION = "On Wall";
    private const string HIT_ANIMATION = "Hit";
    private const string SPAWNING_ANIMATION = "Spawning";
    private const string DISABLED_ANIMATION = "Disabled";

    public void SetIdleAnimation()
    {
        _characterAnimator.Play(IDLE_ANIMATION);
    }

    public void SetRunAnimation()
    {
        _characterAnimator.Play(RUN_ANIMATION);
    }

    public void SetDashAnimation()
    {
        _characterAnimator.Play(DASH_ANIMATION);
    }

    public void SetJumpAnimation()
    {
        _characterAnimator.Play(JUMP_ANIMATION);
    }

    public void SetFallAnimation()
    {
        _characterAnimator.Play(FALL_ANIMATION);
    }

    public void SetOnWallAnimation()
    {
        _characterAnimator.Play(ON_WALL_ANIMATION);
    }

    public void SetHitAnimation()
    {
        _characterAnimator.Play(HIT_ANIMATION);
    }

    public void SetSpawningAnimation()
    {
        _characterAnimator.Play(SPAWNING_ANIMATION);
    }

    public void SetDisabledAnimation()
    {
        _characterAnimator.Play(DISABLED_ANIMATION);
    }
}
