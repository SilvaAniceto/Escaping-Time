using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerInputActions PlayerInputActions { get; set; }

    public float MoveInput { get => PlayerInputActions.PlayerActionMap.Move.ReadValue<float>(); }
    public bool HoldJumpInput { get => PlayerInputActions.PlayerActionMap.Jump.IsPressed(); }
    public bool StartJumpInput { get => PlayerInputActions.PlayerActionMap.Jump.WasPressedThisFrame(); }
    public bool InteractionInput { get => PlayerInputActions.PlayerActionMap.Interact.WasPressedThisFrame(); }
    public bool DashInput { get => PlayerInputActions.PlayerActionMap.Dash.WasPressedThisFrame(); }
    public float CameraTiltInput { get => PlayerInputActions.PlayerActionMap.CameraTilt.ReadValue<float>(); }

    void Start()
    {
        PlayerInputActions = new PlayerInputActions();
        PlayerInputActions.Enable();
    }

    private void OnDestroy()
    {
        PlayerInputActions.Disable();
    }
}
