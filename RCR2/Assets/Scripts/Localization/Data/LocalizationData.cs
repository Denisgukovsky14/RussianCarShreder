using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/Localization Data")]
public class LocalizationData : ScriptableObject
{
    public SystemLanguage defaultLanguage = SystemLanguage.English;
    public List<LanguageData> languages = new List<LanguageData>();

    [NonSerialized]
    private Dictionary<SystemLanguage, LanguageData> languageLookup;

    public void InitializeLookup()
    {
        languageLookup = new Dictionary<SystemLanguage, LanguageData>();
        foreach (var lang in languages)
        {
            languageLookup[lang.language] = lang;
        }
    }

    public LanguageData GetLanguageData(SystemLanguage language)
    {
        if (languageLookup == null) InitializeLookup();

        if (languageLookup.ContainsKey(language))
            return languageLookup[language];

        // Fallback to default language
        if (languageLookup.ContainsKey(defaultLanguage))
            return languageLookup[defaultLanguage];

        return languages.Count > 0 ? languages[0] : null;
    }
}

[Serializable]
public class LanguageData
{
    public SystemLanguage language;
    public List<TextEntry> textEntries = new List<TextEntry>();
    public List<ImageEntry> imageEntries = new List<ImageEntry>();
    public List<AudioEntry> audioEntries = new List<AudioEntry>();
    public List<GameObjectEntry> gameObjectEntries = new List<GameObjectEntry>();

    // Культурные настройки
    public string dateFormat = "dd/MM/yyyy";
    public string timeFormat = "HH:mm";
    public string currencySymbol = "$";
    public bool isRightToLeft = false;
}

[Serializable]
public class TextEntry
{
    public string key;
    public string value;
    [TextArea(3, 5)]
    public string description;
}

[Serializable]
public class ImageEntry
{
    public string key;
    public Sprite sprite;
    public Texture2D texture;
}

[Serializable]
public class AudioEntry
{
    public string key;
    public AudioClip audioClip;
}

[Serializable]
public class GameObjectEntry
{
    public string key;
    public GameObject prefab;
}