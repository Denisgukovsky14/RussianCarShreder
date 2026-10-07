using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LanguageSelector : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private Button nextLanguageButton;
    [SerializeField] private Button previousLanguageButton;
    [SerializeField] private TextMeshProUGUI currentLanguageText;

    private List<SystemLanguage> availableLanguages = new List<SystemLanguage>();
    private int currentLanguageIndex = 0;

    private void Start()
    {
        InitializeSelector();

        // Подписываемся на события
        if (languageDropdown != null)
        {
            languageDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        if (nextLanguageButton != null)
        {
            nextLanguageButton.onClick.AddListener(NextLanguage);
        }

        if (previousLanguageButton != null)
        {
            previousLanguageButton.onClick.AddListener(PreviousLanguage);
        }

        // Обновляем текущий язык
        UpdateCurrentLanguageDisplay();
    }

    private void InitializeSelector()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogError("[LanguageSelector] LocalizationManager не инициализирован");
            return;
        }

        availableLanguages.Clear();

        // Заполняем список поддерживаемых языков
        foreach (var language in LocalizationManager.Instance.SupportedLanguages)
        {
            availableLanguages.Add(language.systemLanguage);
        }

        // Настраиваем dropdown
        if (languageDropdown != null)
        {
            languageDropdown.ClearOptions();

            var options = new List<TMP_Dropdown.OptionData>();
            foreach (var language in LocalizationManager.Instance.SupportedLanguages)
            {
                options.Add(new TMP_Dropdown.OptionData(language.displayName));
            }

            languageDropdown.AddOptions(options);

            // Устанавливаем текущий выбранный язык
            currentLanguageIndex = availableLanguages.IndexOf(LocalizationManager.Instance.CurrentLanguage);
            if (currentLanguageIndex >= 0)
            {
                languageDropdown.SetValueWithoutNotify(currentLanguageIndex);
            }
        }
        else
        {
            // Если dropdown нет, используем кнопки
            currentLanguageIndex = availableLanguages.IndexOf(LocalizationManager.Instance.CurrentLanguage);
        }
    }

    private void OnDropdownValueChanged(int index)
    {
        if (index >= 0 && index < availableLanguages.Count)
        {
            LocalizationManager.Instance.SetLanguage(availableLanguages[index]);
            currentLanguageIndex = index;
            UpdateCurrentLanguageDisplay();
        }
    }

    public void NextLanguage()
    {
        if (availableLanguages.Count == 0) return;

        currentLanguageIndex = (currentLanguageIndex + 1) % availableLanguages.Count;
        LocalizationManager.Instance.SetLanguage(availableLanguages[currentLanguageIndex]);
        UpdateCurrentLanguageDisplay();

        // Обновляем dropdown, если он есть
        if (languageDropdown != null)
        {
            languageDropdown.SetValueWithoutNotify(currentLanguageIndex);
        }
    }

    public void PreviousLanguage()
    {
        if (availableLanguages.Count == 0) return;

        currentLanguageIndex = (currentLanguageIndex - 1 + availableLanguages.Count) % availableLanguages.Count;
        LocalizationManager.Instance.SetLanguage(availableLanguages[currentLanguageIndex]);
        UpdateCurrentLanguageDisplay();

        // Обновляем dropdown, если он есть
        if (languageDropdown != null)
        {
            languageDropdown.SetValueWithoutNotify(currentLanguageIndex);
        }
    }

    private void UpdateCurrentLanguageDisplay()
    {
        if (currentLanguageText != null)
        {
            currentLanguageText.text = LocalizationManager.Instance.GetCurrentLanguageDisplayName();
        }
    }

    public void SetLanguageByCode(string languageCode)
    {
        LocalizationManager.Instance.SetLanguageByCode(languageCode);
        InitializeSelector(); // Переинициализируем селектор
        UpdateCurrentLanguageDisplay();
    }
}