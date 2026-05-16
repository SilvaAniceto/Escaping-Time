using UnityEngine;

[System.Serializable]
public class CharacterPhysicsManager
{
    [Header("Configuration")]
    [SerializeField] private CharacterPhysicsConfig _config;

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
        _accelerationLUT = CalculateLUT(_config.AccelerationCurve);
        _jumpLUT = CalculateLUT(_config.JumpForceCurve);
        _fallLUT = CalculateLUT(_config.FallCurve);
        _dashLUT = CalculateLUT(_config.DashCurve);
        _damageLUT = CalculateLUT(_config.DamageCurve);
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
        _horizontalOvertime += deltaTime / _config.AccelerationDuration;
        _horizontalOvertime = Mathf.Clamp01(_horizontalOvertime);
        return EvaluateLUT(_accelerationLUT, _horizontalOvertime);
    }

    public float GetJumpSpeedLerpOvertime(float deltaTime)
    {
        _jumpOvertime += deltaTime / _config.JumpDuration;
        _jumpOvertime = Mathf.Clamp01(_jumpOvertime);
        return EvaluateLUT(_jumpLUT, _jumpOvertime);
    }

    public float GetFallSpeedLerpOvertime(float deltaTime)
    {
        _fallOvertime += deltaTime / _config.FallDuration;
        _fallOvertime = Mathf.Clamp01(_fallOvertime);
        return EvaluateLUT(_fallLUT, _fallOvertime);
    }

    public float GetDashSpeedLerpOvertime(float deltaTime)
    {
        _dashOvertime += deltaTime / _config.DashDuration;
        _dashOvertime = Mathf.Clamp01(_dashOvertime);
        return EvaluateLUT(_dashLUT, _dashOvertime);
    }

    public float GetDamageSpeedLerpOvertime(float deltaTime)
    {
        _damageOvertime += deltaTime / _config.DamageImpulseDuration;
        _damageOvertime = Mathf.Clamp01(_damageOvertime);
        return EvaluateLUT(_damageLUT, _damageOvertime);
    }
}
