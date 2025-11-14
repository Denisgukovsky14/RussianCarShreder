using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private LocalizationData localizationData;

    [Header("Current Settings")]
    [SerializeField] private SystemLanguage currentLanguage;

    // Кэш для быстрого доступа
    private Dictionary<string, Dictionary<SystemLanguage, string>> stringCache;
    private Dictionary<string, Dictionary<SystemLanguage, Sprite>> spriteCache;

    // События
    public event Action<SystemLanguage> OnLanguageChanged;

    public static LocalizationManager Instance { get; private set; }
    public SystemLanguage CurrentLanguage => currentLanguage;
    public List<Language> SupportedLanguages => localizationData.supportedLanguages;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        BuildCache();

        // Определяем язык по умолчанию
        currentLanguage = DetectSystemLanguage();

        // Загружаем сохраненный язык, если есть
        string savedLanguage = PlayerPrefs.GetString("selected_language", "");
        if (!string.IsNullOrEmpty(savedLanguage))
        {
            SystemLanguage savedLang = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), savedLanguage);
            if (IsLanguageSupported(savedLang))
            {
                currentLanguage = savedLang;
            }
        }

        Debug.Log($"[LocalizationManager] Инициализирован с языком: {currentLanguage}");
        ApplyLocalization();
    }

    private void BuildCache()
    {
        stringCache = new Dictionary<string, Dictionary<SystemLanguage, string>>();
        spriteCache = new Dictionary<string, Dictionary<SystemLanguage, Sprite>>();

        // Кэшируем строки
        foreach (var localizedString in localizationData.localizedStrings)
        {
            var languageDict = new Dictionary<SystemLanguage, string>();
            foreach (var translation in localizedString.translations)
            {
                languageDict[translation.language] = translation.text;
            }
            stringCache[localizedString.key] = languageDict;
        }

        // Кэшируем спрайты
        foreach (var localizedSprite in localizationData.localizedSprites)
        {
            var languageDict = new Dictionary<SystemLanguage, Sprite>();
            foreach (var translation in localizedSprite.translations)
            {
                languageDict[translation.language] = translation.sprite;
            }
            spriteCache[localizedSprite.key] = languageDict;
        }
    }

    private SystemLanguage DetectSystemLanguage()
    {
        SystemLanguage systemLang = Application.systemLanguage;

        // Проверяем поддержку системного языка
        if (IsLanguageSupported(systemLang))
        {
            return systemLang;
        }

        // Если системный язык не поддерживается, используем английский
        if (IsLanguageSupported(SystemLanguage.English))
        {
            return SystemLanguage.English;
        }

        // Если английский не поддерживается, используем первый доступный
        return localizationData.supportedLanguages.Count > 0 ?
            localizationData.supportedLanguages[0].systemLanguage :
            SystemLanguage.English;
    }

    public bool IsLanguageSupported(SystemLanguage language)
    {
        return localizationData.supportedLanguages.Exists(lang => lang.systemLanguage == language);
    }

    // Основные методы получения локализованного контента
    public string GetLocalizedString(string key, string defaultValue = "")
    {
        if (stringCache.ContainsKey(key) && stringCache[key].ContainsKey(currentLanguage))
        {
            return stringCache[key][currentLanguage];
        }

        Debug.LogWarning($"[LocalizationManager] Локализация не найдена для ключа: {key}, язык: {currentLanguage}");
        return defaultValue;
    }

    public Sprite GetLocalizedSprite(string key)
    {
        if (spriteCache.ContainsKey(key) && spriteCache[key].ContainsKey(currentLanguage))
        {
            return spriteCache[key][currentLanguage];
        }

        Debug.LogWarning($"[LocalizationManager] Локализованный спрайт не найден для ключа: {key}, язык: {currentLanguage}");
        return null;
    }

    // Смена языка
    public void SetLanguage(SystemLanguage newLanguage)
    {
        if (!IsLanguageSupported(newLanguage))
        {
            Debug.LogError($"[LocalizationManager] Язык не поддерживается: {newLanguage}");
            return;
        }

        if (currentLanguage != newLanguage)
        {
            currentLanguage = newLanguage;
            PlayerPrefs.SetString("selected_language", newLanguage.ToString());
            PlayerPrefs.Save();

            ApplyLocalization();
            OnLanguageChanged?.Invoke(newLanguage);

            Debug.Log($"[LocalizationManager] Язык изменен на: {newLanguage}");
        }
    }

    public void SetLanguageByCode(string languageCode)
    {
        var language = localizationData.supportedLanguages.Find(lang => lang.languageCode == languageCode);
        if (language != null)
        {
            SetLanguage(language.systemLanguage);
        }
        else
        {
            Debug.LogError($"[LocalizationManager] Язык с кодом не найден: {languageCode}");
        }
    }

    // Применение локализации ко всем элементам
    private void ApplyLocalization()
    {
        var allLocalizableElements = FindObjectsOfType<LocalizedText>(true);
        foreach (var element in allLocalizableElements)
        {
            element.UpdateLocalization();
        }

        var allLocalizableSprites = FindObjectsOfType<LocalizedImage>(true);
        foreach (var element in allLocalizableSprites)
        {
            element.UpdateLocalization();
        }

        Debug.Log($"[LocalizationManager] Локализация применена к {allLocalizableElements.Length} текстовым и {allLocalizableSprites.Length} графическим элементам");
    }

    // Вспомогательные методы
    public string GetCurrentLanguageDisplayName()
    {
        var language = localizationData.supportedLanguages.Find(lang => lang.systemLanguage == currentLanguage);
        return language != null ? language.displayName : currentLanguage.ToString();
    }

    public bool IsCurrentLanguageRTL()
    {
        var language = localizationData.supportedLanguages.Find(lang => lang.systemLanguage == currentLanguage);
        return language != null ? language.isRightToLeft : false;
    }
}