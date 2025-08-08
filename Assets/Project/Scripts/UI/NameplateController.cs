using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UINamespace
{
    public class NameplateController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI nameText;

        private Transform cameraTransform;

        private void Awake()
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void LateUpdate()
        {
            if (cameraTransform != null)
            {
                transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);
            }
        }

        public void SetName(string name)
        {
            if (nameText != null)
            {
                nameText.text = name;
            }
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Debug.Log($"Hiding Nameplate for {this.name}");
        }

        public void Show()
        {
            gameObject.SetActive(true);
            Debug.Log($"Showing Nameplate for {this.name}");
        }
    }
}
