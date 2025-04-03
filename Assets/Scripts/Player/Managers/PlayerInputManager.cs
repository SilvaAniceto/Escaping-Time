using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager PlayerInputInstance;

    private PlayerInputActions PlayerInputActions { get; set; }

    public float MoveInput { get => PlayerInputActions.PlayerActionMap.Move.ReadValue<float>(); }
    public bool HoldJumpInput { get => PlayerInputActions.PlayerActionMap.Jump.IsPressed(); }
    public bool StartJumpInput { get => PlayerInputActions.PlayerActionMap.Jump.WasPressedThisFrame(); }
    public bool InteractionInput { get => PlayerInputActions.PlayerActionMap.Interact.WasPressedThisFrame(); }
    public bool DashInput { get => PlayerInputActions.PlayerActionMap.Dash.WasPressedThisFrame(); }
    public float CameraTiltInput { get => PlayerInputActions.PlayerActionMap.CameraTilt.ReadValue<float>(); }

    void Awake()
    {
        if (PlayerInputInstance == null)
        {
            PlayerInputInstance = this;
        }
        else
        {
            Destroy(PlayerInputInstance);
        }

        if (PlayerInputActions == null)
        {
            PlayerInputActions = new PlayerInputActions();
        }
    }

    private void OnEnable()
    {
        PlayerInputActions.Enable();
    }

    private void OnDisable()
    {
        PlayerInputActions.Disable();
    }

    private void OnDestroy()
    {
        PlayerInputActions.Disable();
    }
}