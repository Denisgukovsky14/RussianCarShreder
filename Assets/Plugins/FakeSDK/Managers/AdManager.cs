// AdManager.cs
using Photon.Pun.Demo.PunBasics;
using SuperMobileAds;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private SuperMobileAdsBanner bannerAd;
    private SuperMobileAdsInterstitial interstitialAd;
    private SuperMobileAdsRewarded rewardedAd;

    void Awake()
    {
        Instance = this;
        InitializeAds();
    }

    void InitializeAds()
    {
        // Инициализируем все типы рекламы
        bannerAd = new SuperMobileAdsBanner();
        interstitialAd = new SuperMobileAdsInterstitial();
        rewardedAd = new SuperMobileAdsRewarded();

        SetupBannerEvents();
        SetupInterstitialEvents();
        SetupRewardedEvents();
    }

    void SetupBannerEvents()
    {
        bannerAd.onAdLoaded += () => Debug.Log("Баннер загружен!");
        bannerAd.onAdFailedToLoad += () => Debug.Log("Баннер не загрузился");
        bannerAd.onAdClicked += () => {
            Debug.Log("Игрок кликнул по баннеру!");
            // Дать бонус игроку
        };
    }

    public void ShowBanner()
    {
        bannerAd.Show();
    }

    void SetupInterstitialEvents()
    {
        interstitialAd.onAdLoaded += () => Debug.Log("Interstitial загружен!");
        interstitialAd.onAdFailedToLoad += () => Debug.Log("Interstitial не загрузился");
        interstitialAd.onAdShown += () => {
            Debug.Log("Interstitial показан");
            // Пауза в игре
            Time.timeScale = 0f;
        };
        interstitialAd.onAdDismissed += () => {
            Debug.Log("Interstitial закрыт");
            // Возобновляем игру
            Time.timeScale = 1f;
        };
        interstitialAd.onAdClicked += () => Debug.Log("Клик по interstitial");
    }

    void SetupRewardedEvents()
    {
        rewardedAd.onAdLoaded += () => Debug.Log("Rewarded загружен!");
        rewardedAd.onAdFailedToLoad += () => Debug.Log("Rewarded не загрузился");
        rewardedAd.onAdShown += () => {
            Debug.Log("Rewarded показан");
            Time.timeScale = 0f;
        };
        rewardedAd.onAdDismissed += () => {
            Debug.Log("Rewarded закрыт");
            Time.timeScale = 1f;
        };
        rewardedAd.onAdRewarded += () => {
            Debug.Log("Игрок получил награду!");
            // Выдаём награду
            Debug.Log("Награда выдана");
        };
    }

    // ДОБАВЬ ЭТИ МЕТОДЫ ДЛЯ КНОПОК
    public void ShowInterstitial()
    {
        interstitialAd.Show();
    }

    public void ShowRewarded()
    {
        rewardedAd.Show();
    }

}
