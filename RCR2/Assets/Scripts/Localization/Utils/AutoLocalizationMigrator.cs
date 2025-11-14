using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AutoLocalizationMigrator : MonoBehaviour
{
    [System.Serializable]
    public class TextMigration
    {
        public TextMeshProUGUI textComponent;
        public string localizationKey;
    }

    [Header("Auto Migration")]
    [SerializeField] private bool runMigrationOnStart = false;
    [SerializeField] private List<TextMigration> textMigrations = new List<TextMigration>();

    [Header("Key Generation")]
    [SerializeField] private string keyPrefix = "ui_";

    private void Start()
    {
        if (runMigrationOnStart)
        {
            AutoMigrateTexts();
        }
    }

    [ContextMenu("Auto Migrate Texts")]
    public void AutoMigrateTexts()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogError("[AutoLocalizationMigrator] LocalizationManager не инициализирован");
            return;
        }

        int migratedCount = 0;

        // Мигрируем заранее заданные тексты
        foreach (var migration in textMigrations)
        {
            if (migration.textComponent != null && !string.IsNullOrEmpty(migration.localizationKey))
            {
                AddLocalizedTextComponent(migration.textComponent, migration.localizationKey);
                migratedCount++;
            }
        }

        // Автоматически находим все тексты в UI
        var allTexts = FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (var text in allTexts)
        {
            // Проверяем, есть ли уже компонент локализации
            if (text.GetComponent<LocalizedText>() == null)
            {
                string generatedKey = GenerateKeyFromText(text.text);
                AddLocalizedTextComponent(text, generatedKey);
                migratedCount++;
            }
        }

        Debug.Log($"[AutoLocalizationMigrator] Мигрировано {migratedCount} текстовых элементов");
    }

    private void AddLocalizedTextComponent(TextMeshProUGUI textComponent, string localizationKey)
    {
        var localizedText = textComponent.gameObject.AddComponent<LocalizedText>();
        localizedText.SetLocalizationKey(localizationKey, textComponent.text);

        // Используем public методы для настройки
        localizedText.SetTextComponent(textComponent);
        localizedText.SetUpdateOnAwake(true);

        Debug.Log($"[AutoLocalizationMigrator] Добавлена локализация для: {textComponent.text} → ключ: {localizationKey}");
    }

    private string GenerateKeyFromText(string text)
    {
        if (string.IsNullOrEmpty(text)) return "empty_key";

        // Генерируем ключ из текста (упрощенная версия)
        string key = text.ToLower()
                       .Replace(" ", "_")
                       .Replace(",", "")
                       .Replace(".", "")
                       .Replace("!", "")
                       .Replace("?", "")
                       .Replace(":", "")
                       .Replace(";", "")
                       .Replace("-", "_")
                       .Replace("(", "")
                       .Replace(")", "");

        return keyPrefix + key;
    }

    [ContextMenu("Export Current Texts")]
    public void ExportCurrentTexts()
    {
        var allTexts = FindObjectsOfType<TextMeshProUGUI>(true);
        var exportList = new List<string>();

        foreach (var text in allTexts)
        {
            if (!string.IsNullOrEmpty(text.text))
            {
                string key = GenerateKeyFromText(text.text);
                exportList.Add($"\"{key}\": \"{text.text}\"");
            }
        }

        // Выводим в консоль для копирования
        Debug.Log("[AutoLocalizationMigrator] Экспорт текстов:\n" + string.Join("\n", exportList));
    }

    [ContextMenu("Find All UI Texts")]
    public void FindAllUITexts()
    {
        var allTexts = FindObjectsOfType<TextMeshProUGUI>(true);
        Debug.Log($"[AutoLocalizationMigrator] Найдено {allTexts.Length} текстовых элементов UI");

        foreach (var text in allTexts)
        {
            Debug.Log($"Текст: '{text.text}'", text.gameObject);
        }
    }
}