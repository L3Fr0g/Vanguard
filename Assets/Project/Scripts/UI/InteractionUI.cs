using UnityEngine;
using UnityEngine.UI;

namespace CharacterNamespace
{
    public class InteractionUI : MonoBehaviour
    {

        public static InteractionUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject promptContainer;
        [SerializeField] private TMPro.TextMeshProUGUI promptText;
        [SerializeField] private Image progressRadialImage;

        private Camera mainCamera;
        private Transform currentTarget;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            mainCamera = Camera.main;
        }

        private void Start()
        {
            Hide();
        }

        private void LateUpdate()
        {
            if (currentTarget != null && promptContainer.activeSelf)
            {
                promptContainer.transform.position = mainCamera.WorldToScreenPoint(currentTarget.position);
            }
        }

        public void Show(IInteractable interactable)
        {
            if (interactable == null) return;

            MonoBehaviour interactableMono = interactable as MonoBehaviour;
            if (interactableMono != null)
            {
                currentTarget = interactableMono.transform;
                if (promptText != null)
                {
                    promptText.text = "[F]";
                }

                if (progressRadialImage != null) progressRadialImage.fillAmount = 0;

                promptContainer.SetActive(true);
            }
        }

        public void Hide()
        {
            currentTarget = null;
            promptContainer.SetActive(false);
            if (progressRadialImage != null) progressRadialImage.fillAmount = 0;
        }

        public void UpdateProgress(float progress)
        {
            if (progressRadialImage != null)
            {
                progressRadialImage.fillAmount = progress;
            }
        }
    }
}
