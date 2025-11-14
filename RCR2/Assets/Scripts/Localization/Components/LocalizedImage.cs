using UnityEngine;
using UnityEngine.UI;

public class LocalizedImage : MonoBehaviour
{
    [Header("Image Settings")]
    [SerializeField] private string imageKey;
    [SerializeField] private Sprite fallbackSprite;

    // Компоненты
    private Image image;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        image = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (image == null && spriteRenderer == null)
        {
            Debug.LogWarning($"[LocalizedImage] No image component found on {gameObject.name}");
        }

        UpdateImage();
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

    public void UpdateImage()
    {
        Sprite localizedSprite = LocalizationManager.Instance.GetSprite(imageKey) ?? fallbackSprite;

        if (image != null)
        {
            image.sprite = localizedSprite;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = localizedSprite;
        }
    }

    private void OnLanguageChanged(SystemLanguage newLanguage)
    {
        UpdateImage();
    }

    public void SetImageKey(string newKey)
    {
        imageKey = newKey;
        UpdateImage();
    }
}