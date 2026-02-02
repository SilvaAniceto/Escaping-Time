using UnityEngine;
using UnityEngine.Events;

public class CameraConfinerChanger : InteractableItem
{
    [SerializeField] private Collider2D _confinerCollider;
    [SerializeField] private UnityEvent OnConfinerChanged = new UnityEvent();

    public override void Awake()
    {
        base.Awake();

        Interactions.Add(EInteractionType.Enter);
    }

    public override void ConfirmInteraction()
    {
        
    }

    public override void InteractablePauseState(bool value)
    {
        base.Awake();
    }

    public override void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {
        CameraBehaviourController.Instance.SetCameraConfiner2D(_confinerCollider);

        OnConfinerChanged?.Invoke();
    }
}
