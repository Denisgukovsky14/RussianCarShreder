using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/Localization Data")]
public class LocalizationData : ScriptableObject
{
    public SystemLanguage defaultLanguage = SystemLanguage.English;
    public List<Language> supportedLanguages = new List<Language>();
    public List<LocalizedString> localizedStrings = new List<LocalizedString>();
    public List<LocalizedSprite> localizedSprites = new List<LocalizedSprite>();
}

[Serializable]
public class Language
{
    public SystemLanguage systemLanguage;
    public string displayName;
    public string languageCode;
    public bool isRightToLeft = false;
}

[Serializable]
public class LocalizedString
{
    public string key;
    public List<LanguageText> translations = new List<LanguageText>();
}

[Serializable]
public class LocalizedSprite
{
    public string key;
    public List<LanguageSprite> translations = new List<LanguageSprite>();
}

[Serializable]
public class LanguageText
{
    public SystemLanguage language;
    [TextArea(1, 3)] public string text;
}

[Serializable]
public class LanguageSprite
{
    public SystemLanguage language;
    public Sprite sprite;
}