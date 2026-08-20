using System.Linq;
using UnityEngine;

public class HourglassBehavior : MonoBehaviour, IInteractableBehavior
{
    [SerializeField] private string _soundName = "";

    private EInteractionType[] _interactionType = new[] { EInteractionType.Enter };
    public EInteractionType[] InteractionType { get => _interactionType; }

    void IInteractableBehavior.Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        if (!_interactionType.Contains(interactionType))
        {
            return;
        }

        gameObject.SetActive(false);

        GameContextManager.Instance.ScoreManager.AddCollectedHourglass();

        GameContextManager.Instance.AudioManager.StopSFX();
        GameContextManager.Instance.AudioManager.PlaySFX(_soundName);
    }

}
