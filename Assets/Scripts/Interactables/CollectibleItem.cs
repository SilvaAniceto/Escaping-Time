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
        Interactions.Add(EInteractionType.Stay);
    }

    public void SetInteraction(CharacterContextManager characterContextManager)
    {
        Destroy(gameObject);
    }
    public void ConfirmInteraction()
    {

    }
}
