using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private LocalizationData localizationData;
    [SerializeField] private SystemLanguage currentLanguage = SystemLanguage.English;
    [SerializeField] private bool autoDetectLanguage = true;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    // Текущие данные языка
    private LanguageData currentLanguageData;
    private Dictionary<string, string> textLookup;
    private Dictionary<string, Sprite> spriteLookup;
    private Dictionary<string, AudioClip> audioLookup;

    // События
    public event Action<SystemLanguage> OnLanguageChanged;

    public static LocalizationManager Instance { get; private set; }
    public SystemLanguage CurrentLanguage => currentLanguage;
    public bool IsRightToLeft => currentLanguageData?.isRightToLeft ?? false;

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
        if (localizationData == null)
        {
            Debug.LogError("[LocalizationManager] LocalizationData not assigned!");
            return;
        }

        localizationData.InitializeLookup();

        // Автоопределение языка
        if (autoDetectLanguage)
        {
            currentLanguage = DetectSystemLanguage();
        }

        LoadLanguage(currentLanguage);

        Log($"Localization initialized. Current language: {currentLanguage}");
    }

    private SystemLanguage DetectSystemLanguage()
    {
        var systemLanguage = Application.systemLanguage;

        // Проверяем поддержку системного языка
        if (localizationData.GetLanguageData(systemLanguage) != null)
        {
            return systemLanguage;
        }

        // Fallback на английский
        return SystemLanguage.English;
    }

    public void LoadLanguage(SystemLanguage language)
    {
        currentLanguageData = localizationData.GetLanguageData(language);
        if (currentLanguageData == null)
        {
            Debug.LogError($"[LocalizationManager] Language data not found for: {language}");
            return;
        }

        currentLanguage = language;

        // Построение lookup таблиц для быстрого доступа
        BuildLookupTables();

        // Обновление всех локализованных компонентов
        UpdateAllLocalizedComponents();

        // Сохранение выбора языка
        PlayerPrefs.SetString("selected_language", language.ToString());
        PlayerPrefs.Save();

        Log($"Language changed to: {language}");
        OnLanguageChanged?.Invoke(language);
    }

    private void BuildLookupTables()
    {
        textLookup = new Dictionary<string, string>();
        spriteLookup = new Dictionary<string, Sprite>();
        audioLookup = new Dictionary<string, AudioClip>();

        // Тексты
        foreach (var entry in currentLanguageData.textEntries)
        {
            if (!string.IsNullOrEmpty(entry.key))
            {
                textLookup[entry.key] = entry.value;
            }
        }

        // Спрайты
        foreach (var entry in currentLanguageData.imageEntries)
        {
            if (!string.IsNullOrEmpty(entry.key) && entry.sprite != null)
            {
                spriteLookup[entry.key] = entry.sprite;
            }
        }

        // Аудио
        foreach (var entry in currentLanguageData.audioEntries)
        {
            if (!string.IsNullOrEmpty(entry.key) && entry.audioClip != null)
            {
                audioLookup[entry.key] = entry.audioClip;
            }
        }
    }

    // Основные методы получения локализованных данных
    public string GetText(string key, string defaultValue = null)
    {
        if (textLookup != null && textLookup.ContainsKey(key))
        {
            return textLookup[key];
        }

        LogWarning($"Text key not found: {key}");
        return defaultValue ?? $"[{key}]";
    }

    public string GetTextFormat(string key, params object[] args)
    {
        var format = GetText(key);
        try
        {
            return string.Format(format, args);
        }
        catch (FormatException e)
        {
            LogError($"Format error for key '{key}': {e.Message}");
            return format;
        }
    }

    public Sprite GetSprite(string key)
    {
        if (spriteLookup != null && spriteLookup.ContainsKey(key))
        {
            return spriteLookup[key];
        }

        LogWarning($"Sprite key not found: {key}");
        return null;
    }

    public AudioClip GetAudioClip(string key)
    {
        if (audioLookup != null && audioLookup.ContainsKey(key))
        {
            return audioLookup[key];
        }

        LogWarning($"Audio clip key not found: {key}");
        return null;
    }

    // Культурные форматы
    public string FormatDate(DateTime date)
    {
        try
        {
            return date.ToString(currentLanguageData.dateFormat);
        }
        catch
        {
            return date.ToString("dd/MM/yyyy");
        }
    }

    public string FormatTime(DateTime time)
    {
        try
        {
            return time.ToString(currentLanguageData.timeFormat);
        }
        catch
        {
            return time.ToString("HH:mm");
        }
    }

    public string FormatCurrency(float amount)
    {
        return $"{amount}{currentLanguageData.currencySymbol}";
    }

    // Обновление всех компонентов
    private void UpdateAllLocalizedComponents()
    {
        var components = FindObjectsOfType<LocalizedText>(true);
        foreach (var component in components)
        {
            component.UpdateText();
        }

        var imageComponents = FindObjectsOfType<LocalizedImage>(true);
        foreach (var component in imageComponents)
        {
            component.UpdateImage();
        }

        var audioComponents = FindObjectsOfType<LocalizedAudio>(true);
        foreach (var component in audioComponents)
        {
            component.UpdateAudio();
        }
    }

    // Вспомогательные методы
    public List<SystemLanguage> GetAvailableLanguages()
    {
        var languages = new List<SystemLanguage>();
        foreach (var langData in localizationData.languages)
        {
            languages.Add(langData.language);
        }
        return languages;
    }

    public bool HasLanguage(SystemLanguage language)
    {
        return localizationData.GetLanguageData(language) != null;
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
            Debug.Log($"[LocalizationManager] {message}");
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning($"[LocalizationManager] {message}");
    }

    private void LogError(string message)
    {
        Debug.LogError($"[LocalizationManager] {message}");
    }

    // Методы для UI
    public void SetLanguageFromName(string languageName)
    {
        if (Enum.TryParse<SystemLanguage>(languageName, out var language))
        {
            LoadLanguage(language);
        }
        else
        {
            LogError($"Invalid language name: {languageName}");
        }
    }
}