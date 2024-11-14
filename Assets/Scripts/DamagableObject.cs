using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableObject : MonoBehaviour, IInteractable
{
    public List<EInteractionType> Interactions { get; set; } = new List<EInteractionType>();
    public bool Activated { get; set; }

    private void Awake()
    {
        Interactions.Add(EInteractionType.Enter);
    }

    public void SetInteraction(GameObject p_gameObject, EInteractionType p_interactionType)
    {
        if (p_gameObject.TryGetComponent(out PlayerContextManager playercontextManager))
        {
            playercontextManager.Damaged = true;
        }
    }
}
