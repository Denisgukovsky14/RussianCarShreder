using UnityEngine;

public class LocalizedAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private string audioKey;
    [SerializeField] private AudioClip fallbackClip;
    [SerializeField] private bool playOnEnable = true;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        UpdateAudio();
    }

    private void OnEnable()
    {
        LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;

        if (playOnEnable)
        {
            PlayLocalizedAudio();
        }
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
    }

    public void UpdateAudio()
    {
        AudioClip localizedClip = LocalizationManager.Instance.GetAudioClip(audioKey) ?? fallbackClip;

        if (audioSource != null)
        {
            audioSource.clip = localizedClip;
        }
    }

    public void PlayLocalizedAudio()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private void OnLanguageChanged(SystemLanguage newLanguage)
    {
        UpdateAudio();
    }

    public void SetAudioKey(string newKey)
    {
        audioKey = newKey;
        UpdateAudio();
    }
}