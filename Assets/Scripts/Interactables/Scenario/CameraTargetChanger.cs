using System.Collections.Generic;
using UnityEngine;

public class CameraTargetChanger : InteractableItem
{
    public override void Awake()
    {
        base.Awake();

        Interactions.Add(EInteractionType.Enter);
        Interactions.Add(EInteractionType.Exit);
    }

    public override void ConfirmInteraction()
    {

    }

    public override void InteractablePauseState(bool value)
    {
        base.InteractablePauseState(value);
    }

    public override void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {
        switch (interactionType)
        {
            case EInteractionType.Enter:
                characterContextManager.CameraTarget.SetParent(null);
                characterContextManager.CameraTarget.position = transform.position;
                break;
            case EInteractionType.Stay:
                break;
            case EInteractionType.Exit:
                if (characterContextManager.CameraTarget.parent == null)
                {
                    characterContextManager.CameraTarget.SetParent(characterContextManager.transform.GetChild(0));
                    characterContextManager.CameraTarget.localPosition = Vector3.zero;
                    characterContextManager.CameraTarget.rotation = characterContextManager.CameraTarget.parent.rotation;
                }
                break;
            default:
                break;
        }
    }
}
