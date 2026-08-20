public interface IJumpCapability
{
    bool CoyoteTime { get; set; }
    bool HasAirJump { get; }
    bool AirJumpIsAllowed { get; }
    void EnableAirJump();
    void DisableAirJump();
}