using UnityEngine;
using UnityEngine.UI;

public class LocalizedImage : MonoBehaviour
{
    [Header("Localization Settings")]
    [SerializeField] private string localizationKey;
    [SerializeField] private bool updateOnAwake = true;

    [Header("Image Component")]
    [SerializeField] private Image imageComponent;

    private void Awake()
    {
        if (imageComponent == null)
            imageComponent = GetComponent<Image>();

        if (updateOnAwake)
        {
            UpdateLocalization();
        }
    }

    private void OnEnable()
    {
        LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
    }

    private void Start()
    {
        if (!updateOnAwake)
        {
            UpdateLocalization();
        }
    }

    public void UpdateLocalization()
    {
        if (LocalizationManager.Instance == null || imageComponent == null)
            return;

        Sprite localizedSprite = LocalizationManager.Instance.GetLocalizedSprite(localizationKey);
        if (localizedSprite != null)
        {
            imageComponent.sprite = localizedSprite;
        }
    }

    private void OnLanguageChanged(SystemLanguage newLanguage)
    {
        UpdateLocalization();
    }

    public void SetLocalizationKey(string newKey)
    {
        localizationKey = newKey;
        UpdateLocalization();
    }
}