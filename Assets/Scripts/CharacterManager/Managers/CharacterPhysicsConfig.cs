using UnityEngine;

[CreateAssetMenu(fileName = "CharacterPhysicsConfig", menuName = "Character Physics Config", order = 1)]
public class CharacterPhysicsConfig : ScriptableObject
{
    [Header("Acceleration")]
    public AnimationCurve AccelerationCurve;
    [Header("Jump Force")]
    public AnimationCurve JumpForceCurve;
    [Header("Fall")]
    public AnimationCurve FallCurve;
    [Header("Dash")]
    public AnimationCurve DashCurve;
    [Header("Damage")]
    public AnimationCurve DamageCurve;

    [Header("Durations")]
    public float AccelerationDuration = 0.62f;
    public float JumpDuration = 0.36f;
    public float FallDuration = 0.48f;
    public float DashDuration = 0.62f;
    public float DamageImpulseDuration = 0.62f;
}
