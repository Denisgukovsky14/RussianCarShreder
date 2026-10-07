using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SuperMobileAds
{
    public class SuperMobileAdsInterstitial
    {
        private bool _isInitialized;
        private string _adUnitId;
        private bool _isLoaded;
        private bool _isShowing;

        public Action onAdClicked;
        public Action onAdDismissed;
        public Action onAdLoaded;
        public Action onAdFailedToLoad;
        public Action onAdShown;

        public void Initialize(string adUnitId)
        {
            Debug.Log("SuperMobileAdsInterstitial Initialized");
            _isInitialized = true;
            _adUnitId = adUnitId;
            _isShowing = false;
            _isLoaded = false;
        }

        public void Load()
        {
            if (!_isInitialized)
            {
                Debug.Log("SuperMobileAdsInterstitial is not initialized");
                return;
            }

            LoadingBanner();
        }

        public void Show()
        {
            if (!_isInitialized)
            {
                Debug.Log("SuperMobileAdsInterstitial is not initialized");
                return;
            }

            if (_isShowing)
            {
                Debug.Log("SuperMobileAdsInterstitial is already showing");
                return;
            }

            if (!_isLoaded)
            {
                Debug.Log("SuperMobileAdsInterstitial is not loaded");
                return;
            }

            ShowingBanner();

            Debug.Log("SuperMobileAdsInterstitial Showed " + _adUnitId);
        }

        private async void LoadingBanner()
        {
            await Task.Delay(UnityEngine.Random.Range(100, 5000));

            if (UnityEngine.Random.Range(0f, 6f) > 2f)
            {
                onAdLoaded?.Invoke();
                _isLoaded = true;
                Debug.Log("SuperMobileAdsInterstitial Loaded " + _adUnitId);
            }
            else
            {
                onAdFailedToLoad?.Invoke();
                _isLoaded = false;
                Debug.Log("SuperMobileAdsInterstitial Failed to Load " + _adUnitId);
            }
        }

        private async void ShowingBanner()
        {
            await Task.Delay(UnityEngine.Random.Range(100, 5000));

            onAdShown?.Invoke();
            _isShowing = true;
            Debug.Log("SuperMobileAdsInterstitial Showed " + _adUnitId);

            if (UnityEngine.Random.Range(0f, 6f) > 5f)
            {
                await Task.Delay(UnityEngine.Random.Range(3000, 5000));
                _isShowing = false;
                onAdClicked?.Invoke();
            }
            else
            {
                await Task.Delay(UnityEngine.Random.Range(500, 5000));
                _isShowing = false;
                onAdDismissed?.Invoke();
            }
        }
    }
}
