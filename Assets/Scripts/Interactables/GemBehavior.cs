using System.Linq;
using UnityEngine;

public class GemBehavior : MonoBehaviour, IInteractableBehavior
{
    [SerializeField] private int _scoreValue;
    [SerializeField] private string _soundName = "";

    private EInteractionType[] _interactionType = new[] { EInteractionType.Enter };
    public EInteractionType[] InteractionType { get => _interactionType; }

    public void Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        if (!_interactionType.Contains(interactionType))
        {
            return;
        }

        ServiceLocator.AudioManager.StopSFX();
        ServiceLocator.AudioManager.PlaySFX(_soundName);

        if (_scoreValue > 0)
        {
            ServiceLocator.ScoreManager.AddGemScore(_scoreValue);
        }

        gameObject.SetActive(false);
    }
}