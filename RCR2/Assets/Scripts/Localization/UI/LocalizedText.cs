using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    [Header("Localization Settings")]
    [SerializeField] private string localizationKey;
    [SerializeField] private bool updateOnAwake = true;

    [Header("Text Components")]
    [SerializeField] public TextMeshProUGUI textMeshPro; // Изменили на public
    [SerializeField] private UnityEngine.UI.Text legacyText;

    [Header("Fallback")]
    [SerializeField] private string fallbackText = "";

    // Public свойства для доступа из других скриптов
    public string LocalizationKey => localizationKey;
    public bool UpdateOnAwake => updateOnAwake;

    private void Awake()
    {
        // Автоматически находим текстовые компоненты, если не назначены
        if (textMeshPro == null)
            textMeshPro = GetComponent<TextMeshProUGUI>();
        if (legacyText == null)
            legacyText = GetComponent<UnityEngine.UI.Text>();

        if (updateOnAwake)
        {
            UpdateLocalization();
        }
    }

    private void OnEnable()
    {
        // Подписываемся на событие смены языка
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }
    }

    private void OnDisable()
    {
        // Отписываемся от события
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
    }

    private void Start()
    {
        // Обновляем при старте, если не обновили в Awake
        if (!updateOnAwake)
        {
            UpdateLocalization();
        }
    }

    public void UpdateLocalization()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogWarning("[LocalizedText] LocalizationManager не инициализирован");
            return;
        }

        string localizedText = LocalizationManager.Instance.GetLocalizedString(localizationKey, fallbackText);

        if (textMeshPro != null)
        {
            textMeshPro.text = localizedText;

            // Обработка RTL языков
            if (LocalizationManager.Instance.IsCurrentLanguageRTL())
            {
                textMeshPro.isRightToLeftText = true;
            }
        }

        if (legacyText != null)
        {
            legacyText.text = localizedText;
        }
    }

    private void OnLanguageChanged(SystemLanguage newLanguage)
    {
        UpdateLocalization();
    }

    // Метод для изменения ключа в runtime
    public void SetLocalizationKey(string newKey, string newFallback = "")
    {
        localizationKey = newKey;
        if (!string.IsNullOrEmpty(newFallback))
        {
            fallbackText = newFallback;
        }
        UpdateLocalization();
    }

    // Public методы для настройки из других скриптов
    public void SetTextComponent(TextMeshProUGUI newTextComponent)
    {
        textMeshPro = newTextComponent;
    }

    public void SetUpdateOnAwake(bool shouldUpdate)
    {
        updateOnAwake = shouldUpdate;
    }

    // Вспомогательные методы для отладки
    public string GetCurrentKey() => localizationKey;
    public string GetCurrentText()
    {
        if (textMeshPro != null) return textMeshPro.text;
        if (legacyText != null) return legacyText.text;
        return "";
    }
}