using UnityEngine;

namespace CharacterNamespace
{
    public enum InteractionType
    {
        Instant,
        Pickup,
        Mining,
        Fishing
    }

    public interface IInteractable
    {
        string InteractionPrompt { get; }
        float InteractionDuration { get; }
        InteractionType Type { get; }

        bool Interact(PlayerInteractor interactor);
    }
}