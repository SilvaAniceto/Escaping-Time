using UnityEngine;

[System.Serializable]
public class CharacterPhysicsManager
{
    [Header("Acceleration Curve")]
    [SerializeField] private AnimationCurve _accelerationCurve;
    [Header("Jump Force Curve")]
    [SerializeField] private AnimationCurve _jumpForceCurve;
    [Header("Fall Curve")]
    [SerializeField] private AnimationCurve _fallCurve;
    [Header("Dash Curve")]
    [SerializeField] private AnimationCurve _dashCurve;
    [Header("Damage Curve")]
    [SerializeField] private AnimationCurve _damageCurve;

    private float[] _accelerationLUT;
    private float[] _jumpLUT;
    private float[] _fallLUT;
    private float[] _dashLUT;
    private float[] _damageLUT;

    private float _horizontalOvertime;
    private float _jumpOvertime;
    private float _fallOvertime;
    private float _dashOvertime;
    private float _damageOvertime;

    private int _moveDirection;
    private float _horizontalSpeed;
    private float _horizontalStartSpeed;
    private float _horizontalTopSpeed;
    private float _jumpSpeed;
    private float _fallStartSpeed;
    private bool _coyoteTime;

    private const float ACCELERATION_DURATION = 0.62f;
    private const float JUMP_DURATION = 0.36f;
    private const float FALL_DURATION = 0.48f;
    private const float DASH_DURATION = 0.62f;
    private const float DAMAGE_IMPULSE_DURATION = 0.62f;

    public int MoveDirection { get => _moveDirection; set => _moveDirection = value; }
    public float JumpSpeed { get => _jumpSpeed; set => _jumpSpeed = value; }
    public Vector2 MovePosition => new Vector2(HorizontalSpeed, JumpSpeed);
    public float HorizontalSpeed { get => _horizontalSpeed; set => _horizontalSpeed = value; }
    public float HorizontalStartSpeed { get => _horizontalStartSpeed; set => _horizontalStartSpeed = value; }
    public float HorizontalTopSpeed { get => _horizontalTopSpeed; set => _horizontalTopSpeed = value; }
    public float FallStartSpeed { get => _fallStartSpeed; set => _fallStartSpeed = value; }
    public bool CoyoteTime { get => _coyoteTime; set => _coyoteTime = value; }

    public void ResetHorizontalOvertime() => _horizontalOvertime = 0f;
    public void ResetJumpOvertime() => _jumpOvertime = 0f;
    public void ResetFallOvertime() => _fallOvertime = 0f;
    public void ResetDashOvertime() => _dashOvertime = 0f;
    public void ResetDamageOvertime() => _damageOvertime = 0f;

    public void Initialize()
    {
        _accelerationLUT = CalculateLUT(_accelerationCurve);
        _jumpLUT = CalculateLUT(_jumpForceCurve);
        _fallLUT = CalculateLUT(_fallCurve);
        _dashLUT = CalculateLUT(_dashCurve);
        _damageLUT = CalculateLUT(_damageCurve);
    }

    private float[] CalculateLUT(AnimationCurve curve)
    {
        float[] lut = new float[128];
        for (int i = 0; i < lut.Length; i++)
        {
            float t = (float)i / (lut.Length - 1);
            lut[i] = curve.Evaluate(t);
        }
        return lut;
    }

    private float EvaluateLUT(float[] lut, float time)
    {
        time = Mathf.Clamp01(time);
        float index = time * (lut.Length - 1);
        int prev = (int)index;
        int next = Mathf.Min(prev + 1, lut.Length - 1);
        float frac = index - prev;
        return Mathf.Lerp(lut[prev], lut[next], frac);
    }

    public float GetHorizontalSpeedLerpOvertime(float deltaTime)
    {
        _horizontalOvertime += deltaTime / ACCELERATION_DURATION;
        _horizontalOvertime = Mathf.Clamp01(_horizontalOvertime);
        return EvaluateLUT(_accelerationLUT, _horizontalOvertime);
    }

    public float GetJumpSpeedLerpOvertime(float deltaTime)
    {
        _jumpOvertime += deltaTime / JUMP_DURATION;
        _jumpOvertime = Mathf.Clamp01(_jumpOvertime);
        return EvaluateLUT(_jumpLUT, _jumpOvertime);
    }

    public float GetFallSpeedLerpOvertime(float deltaTime)
    {
        _fallOvertime += deltaTime / FALL_DURATION;
        _fallOvertime = Mathf.Clamp01(_fallOvertime);
        return EvaluateLUT(_fallLUT, _fallOvertime);
    }

    public float GetDashSpeedLerpOvertime(float deltaTime)
    {
        _dashOvertime += deltaTime / DASH_DURATION;
        _dashOvertime = Mathf.Clamp01(_dashOvertime);
        return EvaluateLUT(_dashLUT, _dashOvertime);
    }

    public float GetDamageSpeedLerpOvertime(float deltaTime)
    {
        _damageOvertime += deltaTime / DAMAGE_IMPULSE_DURATION;
        _damageOvertime = Mathf.Clamp01(_damageOvertime);
        return EvaluateLUT(_damageLUT, _damageOvertime);
    }
}
