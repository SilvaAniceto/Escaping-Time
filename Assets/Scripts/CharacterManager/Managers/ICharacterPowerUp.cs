public enum EPowerUpType
{
    Infinity,
    Temporary
}
public enum EPowerUp
{
    AirJump,
    Dash,
    WallMove
}
public interface ICharacterPowerUp
{
    public EPowerUpType PowerUpType { get; set; }
    public EPowerUp PowerUp { get; set; }
    public void RechargePowerUpInteractable() { }
    public void DestroyPowerUpInteractable() { }
}
