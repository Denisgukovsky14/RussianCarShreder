using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CornerLanguageSelector : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button toggleButton;
    [SerializeField] private TextMeshProUGUI languageText;
    [SerializeField] private GameObject languagePanel;
    [SerializeField] private Button russianButton;
    [SerializeField] private Button englishButton;

    private bool isPanelOpen = false;

    private void Start()
    {
        // Инициализация UI
        InitializeSelector();

        // Настройка кнопок
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleLanguagePanel);
        }

        if (russianButton != null)
        {
            russianButton.onClick.AddListener(() => SetLanguage(SystemLanguage.Russian));
        }

        if (englishButton != null)
        {
            englishButton.onClick.AddListener(() => SetLanguage(SystemLanguage.English));
        }

        // Скрываем панель при старте
        if (languagePanel != null)
        {
            languagePanel.SetActive(false);
        }

        UpdateLanguageDisplay();
    }

    private void InitializeSelector()
    {
        // Автоматически находим элементы, если они не назначены
        if (toggleButton == null)
            toggleButton = GetComponent<Button>();

        if (languageText == null)
            languageText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void ToggleLanguagePanel()
    {
        isPanelOpen = !isPanelOpen;

        if (languagePanel != null)
        {
            languagePanel.SetActive(isPanelOpen);
        }
    }

    private void SetLanguage(SystemLanguage language)
    {
        if (LocalizationManager.Instance != null)
        {
            Debug.Log("It's OK");
            LocalizationManager.Instance.SetLanguage(language);
            UpdateLanguageDisplay();
            CloseLanguagePanel();
        }
        else
        {
            Debug.Log("Very BAD!!!");
        }
    }

    private void UpdateLanguageDisplay()
    {
        if (languageText != null && LocalizationManager.Instance != null)
        {
            // Показываем сокращение текущего языка (RU/EN)
            string currentLang = LocalizationManager.Instance.CurrentLanguage.ToString();
            languageText.text = currentLang.Substring(0, 2).ToUpper();
        }
    }

    private void CloseLanguagePanel()
    {
        isPanelOpen = false;
        if (languagePanel != null)
        {
            languagePanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от событий
        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveAllListeners();
        }
        if (russianButton != null)
        {
            russianButton.onClick.RemoveAllListeners();
        }
        if (englishButton != null)
        {
            englishButton.onClick.RemoveAllListeners();
        }
    }
}