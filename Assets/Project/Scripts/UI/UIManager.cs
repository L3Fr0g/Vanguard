using UnityEngine;
using UnityEngine.UI;
//using System.Collections; //potentially need this for a coroutine fade

namespace UINamespace
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private CanvasGroup inventoryCanvasGroup;
        //add other CanvasGroups as UI is expanded

        private bool isInventoryOpen = false;

        private void Awake()
        {
            if(Instance !=null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void SetCanvasGroupState(CanvasGroup canvasGroup, bool visible)
        {
            if (canvasGroup == null)
            {
                Debug.Log($"CanvasGroup for '{canvasGroup.name}' is not assigned!");
                return;
            }

            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        public void ToggleInventoryUI()
        {
            isInventoryOpen = !isInventoryOpen;
            SetCanvasGroupState(inventoryCanvasGroup, isInventoryOpen);

            //Potentially add logic here to close other UI panels
        }
    }
}
