using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class AdButtonHandler : MonoBehaviour
{
    public enum AdType { Banner, Interstitial, Rewarded }
    public AdType adType;

    [Header("Ссылка на PlayerAdManager")]
    public PlayerAdManager playerAdManager;

    private PhotonView view;

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);

        // Находим PhotonView для проверки владения
        view = GetComponentInParent<PhotonView>();

        // Автоматически находим PlayerAdManager локального игрока
        if (playerAdManager == null)
            FindLocalPlayerAdManager();
    }

    void FindLocalPlayerAdManager()
    {
        // Ищем всех PlayerAdManager в сцене
        PlayerAdManager[] allAdManagers = FindObjectsOfType<PlayerAdManager>();

        foreach (PlayerAdManager manager in allAdManagers)
        {
            PhotonView playerView = manager.GetComponentInParent<PhotonView>();
            if (playerView != null && playerView.IsMine)
            {
                playerAdManager = manager;
                Debug.Log("Найден PlayerAdManager локального игрока");
                return;
            }
        }

        Debug.LogError("PlayerAdManager локального игрока не найден!");
    }

    public void OnButtonClick()
    {
        if (playerAdManager == null)
        {
            Debug.LogError("PlayerAdManager не назначен!");
            return;
        }

        // Дополнительная проверка для Photon
        PhotonView playerView = playerAdManager.GetComponentInParent<PhotonView>();
        if (playerView != null && !playerView.IsMine)
        {
            Debug.LogError("Попытка вызвать рекламу для чужого игрока!");
            return;
        }

        Debug.Log($"Кнопка нажата! Тип: {adType}");

        switch (adType)
        {
            case AdType.Banner: playerAdManager.ShowBanner(); break;
            case AdType.Interstitial: playerAdManager.ShowInterstitial(); break;
            case AdType.Rewarded: playerAdManager.ShowRewarded(); break;
        }
    }
}