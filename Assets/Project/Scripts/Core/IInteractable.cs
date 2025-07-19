namespace CharacterNamespace
{
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        float InteractionDuration { get; }

        bool Interact(PlayerInteractor interactor);
    }
}