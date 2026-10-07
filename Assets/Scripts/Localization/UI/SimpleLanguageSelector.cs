using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleLanguageSelector : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button englishButton;
    [SerializeField] private Button russianButton;
    [SerializeField] private TextMeshProUGUI currentLanguageText;

    private void Start()
    {
        // Настройка кнопок
        if (englishButton != null)
        {
            englishButton.onClick.AddListener(() => SetLanguage(SystemLanguage.English));
        }

        if (russianButton != null)
        {
            russianButton.onClick.AddListener(() => SetLanguage(SystemLanguage.Russian));
        }

        // Обновляем отображение текущего языка
        UpdateLanguageDisplay();

        // Подписываемся на события смены языка
        LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
    }

    private void SetLanguage(SystemLanguage language)
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SetLanguage(language);
        }
    }

    private void OnLanguageChanged(SystemLanguage newLanguage)
    {
        UpdateLanguageDisplay();
    }

    private void UpdateLanguageDisplay()
    {
        if (currentLanguageText != null && LocalizationManager.Instance != null)
        {
            currentLanguageText.text = LocalizationManager.Instance.GetCurrentLanguageDisplayName();
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от событий
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
    }
}