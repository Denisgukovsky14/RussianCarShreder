using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private string textKey;
    [SerializeField] private string fallbackText;
    [SerializeField] private bool useTextMeshPro = true;
    [SerializeField] private bool updateOnAwake = true;

    [Header("Formatting")]
    [SerializeField] private bool useFormatting = false;
    [SerializeField] private string[] formatParameters;

    // Компоненты
    private TextMeshProUGUI textMeshPro;
    private Text legacyText;

    private void Awake()
    {
        // Поиск компонентов текста
        textMeshPro = GetComponent<TextMeshProUGUI>();
        legacyText = GetComponent<Text>();

        if (textMeshPro == null && legacyText == null)
        {
            Debug.LogWarning($"[LocalizedText] No text component found on {gameObject.name}");
        }

        if (updateOnAwake)
        {
            UpdateText();
        }
    }

    private void OnEnable()
    {
        // Подписка на смену языка
        LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
    }

    public void UpdateText()
    {
        string localizedText = GetLocalizedText();

        if (useTextMeshPro && textMeshPro != null)
        {
            textMeshPro.text = localizedText;

            // Поддержка RTL языков
            if (LocalizationManager.Instance.IsRightToLeft)
            {
                textMeshPro.isRightToLeftText = true;
                textMeshPro.alignment = TextAlignmentOptions.Right;
            }
        }
        else if (legacyText != null)
        {
            legacyText.text = localizedText;

            // Поддержка RTL языков
            if (LocalizationManager.Instance.IsRightToLeft)
            {
                legacyText.alignment = TextAnchor.UpperRight;
            }
        }
    }

    private string GetLocalizedText()
    {
        if (useFormatting && formatParameters != null && formatParameters.Length > 0)
        {
            return LocalizationManager.Instance.GetTextFormat(textKey, formatParameters);
        }
        else
        {
            return LocalizationManager.Instance.GetText(textKey, fallbackText);
        }
    }

    private void OnLanguageChanged(SystemLanguage newLanguage)
    {
        UpdateText();
    }

    // Публичные методы для динамического обновления
    public void SetTextKey(string newKey)
    {
        textKey = newKey;
        UpdateText();
    }

    public void SetFormatParameters(params string[] parameters)
    {
        formatParameters = parameters;
        UpdateText();
    }

    // Метод для редактора
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Автоматическое определение типа текстового компонента
        if (textMeshPro == null && legacyText == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
            legacyText = GetComponent<Text>();

            if (textMeshPro != null) useTextMeshPro = true;
            else if (legacyText != null) useTextMeshPro = false;
        }
    }
#endif
}