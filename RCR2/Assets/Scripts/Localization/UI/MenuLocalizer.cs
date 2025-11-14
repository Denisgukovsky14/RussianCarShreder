using UnityEngine;
using TMPro;

public class MenuLocalizer : MonoBehaviour
{
    [Header("Menu Texts")]
    [SerializeField] private string startGameKey = "menu_start_game";
    [SerializeField] private string settingsKey = "menu_settings";
    [SerializeField] private string exitKey = "menu_exit";

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI startGameText;
    [SerializeField] private TextMeshProUGUI settingsText;
    [SerializeField] private TextMeshProUGUI exitText;

    private void Start()
    {
        UpdateMenuTexts();

        LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
    }

    private void UpdateMenuTexts()
    {
        if (startGameText != null)
            startGameText.text = LocalizationManager.Instance.GetText(startGameKey, "Start Game");

        if (settingsText != null)
            settingsText.text = LocalizationManager.Instance.GetText(settingsKey, "Settings");

        if (exitText != null)
            exitText.text = LocalizationManager.Instance.GetText(exitKey, "Exit");
    }

    private void OnLanguageChanged(SystemLanguage newLanguage)
    {
        UpdateMenuTexts();
    }
}