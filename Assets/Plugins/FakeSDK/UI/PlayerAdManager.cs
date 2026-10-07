using UnityEngine;
using Photon.Pun;
using SuperMobileAds;
using Photon.Pun.Demo.PunBasics;

public class PlayerAdManager : MonoBehaviour
{
    private SuperMobileAdsBanner bannerAd;
    private SuperMobileAdsInterstitial interstitialAd;
    private SuperMobileAdsRewarded rewardedAd;

    [Header("Fake Ad Unit IDs")]
    public string bannerAdUnitId = "fake_banner_123";
    public string interstitialAdUnitId = "fake_interstitial_456";
    public string rewardedAdUnitId = "fake_rewarded_789";


    private PhotonView view;

    void Start()
    {
        view = GetComponentInParent<PhotonView>();

        // Инициализируем рекламу только для своего игрока
        if (view != null && view.IsMine)
        {
            bannerAd = new SuperMobileAdsBanner();
            interstitialAd = new SuperMobileAdsInterstitial();
            rewardedAd = new SuperMobileAdsRewarded();

            LoadAllAds();
            InitializeAds();
            SetupRewardedEvents();
            Debug.Log("Реклама для локального игрока готова!");
        }
        else
        {
            // Отключаем для чужих игроков
            enabled = false;
            return;
        }
    }

    void InitializeAds()
    {
        bannerAd = new SuperMobileAdsBanner();
        interstitialAd = new SuperMobileAdsInterstitial();
        rewardedAd = new SuperMobileAdsRewarded();

        // Инициализируем каждый тип рекламы
        bannerAd.Initialize(bannerAdUnitId);       // если есть такой метод
        interstitialAd.Initialize(interstitialAdUnitId); // если есть такой метод  
        rewardedAd.Initialize(rewardedAdUnitId);     // если есть такой метод

        Debug.Log("Вся реклама инициализирована!");
    }

    public void ShowBanner()
    {
        if (view != null && !view.IsMine) return;

        Debug.Log("Показываем баннер");
        bannerAd?.Show();
    }

    public void ShowInterstitial()
    {
        if (view != null && !view.IsMine) return;

        Debug.Log("Показываем interstitial");
        interstitialAd?.Show();
    }

    public void ShowRewarded()
    {
        if (view != null && !view.IsMine) return;

        Debug.Log("Показываем rewarded");
        rewardedAd?.Show();
    }

    void LoadAllAds()
    {
        // Загружаем рекламу
        bannerAd.Load();       //  ЕСЛИ ЕСТЬ МЕТОД Load()
        interstitialAd.Load(); // ЕСЛИ ЕСТЬ МЕТОД Load()  
        rewardedAd.Load();     //  ЕСЛИ ЕСТЬ МЕТОД Load()

        Debug.Log("Реклама загружается...");

        // Или используй корутину для задержки
        StartCoroutine(SimulateAdLoading());
    }

    System.Collections.IEnumerator SimulateAdLoading()
    {
        // Имитируем загрузку рекламы
        yield return new WaitForSeconds(1f);
        Debug.Log("Реклама должна быть загружена!");
    }

    void SetupRewardedEvents()
    {
        if (rewardedAd != null)
        {
            rewardedAd.onAdShown += () => {
                Debug.Log("Rewarded реклама началась!");
                Time.timeScale = 0f; // Пауза в игре
            };

            rewardedAd.onAdRewarded += () => {
                AnalyticsManager.Instance.TrackUserEvent("ad_watched", null);
                Debug.Log("Игрок получил награду!");
                // Выдай игроку валюту/бонусы
                Debug.Log("Игрок получил свои бонусы");
                Time.timeScale = 1f; // Возобновляем игру
            };

            rewardedAd.onAdDismissed += () => {
                Debug.Log("Игрок закрыл рекламу");
                Time.timeScale = 1f; // Возобновляем игру
            };
        }
    }

}