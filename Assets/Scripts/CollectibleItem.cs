using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour, IInteractable
{
    public List<EInteractionType> Interactions { get ; set ; } = new List<EInteractionType>();
    public bool Activated { get; set; }
    public bool MovableObject { get; set; }

    private void Awake()
    {
        Interactions.Add(EInteractionType.TriggerEnter);
    }

    public void SetInteraction(GameObject p_gameObject, EInteractionType p_interactionType)
    {
        if (p_gameObject.TryGetComponent(out PlayerContextManager playercontextManager))
        {
            Destroy(gameObject);
            playercontextManager.KeyItem++;
        }
    }
    public void ConfirmInteraction()
    {

    }
}
