using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LanguageSelectorUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject languagePanel;
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform languageButtonContainer;
    [SerializeField] private GameObject languageButtonPrefab;

    [Header("Localized Texts")]
    [SerializeField] private string titleTextKey = "language_selection_title";
    [SerializeField] private string closeButtonTextKey = "close";

    private List<GameObject> languageButtons = new List<GameObject>();

    private void Start()
    {
        // Настройка кнопок
        openButton.onClick.AddListener(OpenLanguagePanel);
        closeButton.onClick.AddListener(CloseLanguagePanel);

        // Создание кнопок выбора языка
        CreateLanguageButtons();

        // Закрытие панели при старте
        languagePanel.SetActive(false);
    }

    private void CreateLanguageButtons()
    {
        // Очистка старых кнопок
        foreach (var button in languageButtons)
        {
            Destroy(button);
        }
        languageButtons.Clear();

        // Получение доступных языков
        var availableLanguages = LocalizationManager.Instance.GetAvailableLanguages();

        foreach (var language in availableLanguages)
        {
            GameObject buttonObj = Instantiate(languageButtonPrefab, languageButtonContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            // Установка названия языка
            buttonText.text = GetLanguageDisplayName(language);

            // Настройка обработчика клика
            SystemLanguage lang = language; // Capture for closure
            button.onClick.AddListener(() => SelectLanguage(lang));

            // Выделение текущего языка
            if (language == LocalizationManager.Instance.CurrentLanguage)
            {
                var colors = button.colors;
                colors.normalColor = Color.green;
                button.colors = colors;
            }

            languageButtons.Add(buttonObj);
        }
    }

    private string GetLanguageDisplayName(SystemLanguage language)
    {
        // Локализованные названия языков
        switch (language)
        {
            case SystemLanguage.English: return LocalizationManager.Instance.GetText("language_english", "English");
            case SystemLanguage.Russian: return LocalizationManager.Instance.GetText("language_russian", "Russian");
            case SystemLanguage.Spanish: return LocalizationManager.Instance.GetText("language_spanish", "Spanish");
            case SystemLanguage.French: return LocalizationManager.Instance.GetText("language_french", "French");
            case SystemLanguage.German: return LocalizationManager.Instance.GetText("language_german", "German");
            default: return language.ToString();
        }
    }

    private void SelectLanguage(SystemLanguage language)
    {
        LocalizationManager.Instance.LoadLanguage(language);
        CloseLanguagePanel();
        CreateLanguageButtons(); // Обновление кнопок для выделения выбранного языка
    }

    private void OpenLanguagePanel()
    {
        languagePanel.SetActive(true);
    }

    private void CloseLanguagePanel()
    {
        languagePanel.SetActive(false);
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

    private void OnLanguageChanged(SystemLanguage newLanguage)
    {
        // Обновление UI при смене языка
        CreateLanguageButtons();
    }
}