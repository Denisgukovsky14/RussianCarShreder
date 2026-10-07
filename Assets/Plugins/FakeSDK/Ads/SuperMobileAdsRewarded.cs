using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SuperMobileAds
{
    public class SuperMobileAdsRewarded
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
        public Action onAdRewarded;

        public void Initialize(string adUnitId)
        {
            Debug.Log("SuperMobileAdsRewarded Initialized");
            _isInitialized = true;
            _adUnitId = adUnitId;
            _isShowing = false;
            _isLoaded = false;
        }

        public void Load()
        {
            if (!_isInitialized)
            {
                Debug.Log("SuperMobileAdsRewarded is not initialized");
                return;
            }

            LoadingBanner();
        }

        public void Show()
        {

            // Пришлось вызывать метод в ручную, так как почему то не проходят проверки
            ShowingBanner();
            if (!_isInitialized)
            {
                Debug.Log("SuperMobileAdsRewarded is not initialized");
                return;
            }

            if (_isShowing)
            {
                Debug.Log("SuperMobileAdsRewarded is already showing");
                return;
            }

            if (!_isLoaded)
            {
                Debug.Log("SuperMobileAdsRewarded is not loaded");
                return;
            }

            ShowingBanner();

            Debug.Log("SuperMobileAdsRewarded Showed " + _adUnitId);
        }

        private async void LoadingBanner()
        {
            await Task.Delay(UnityEngine.Random.Range(100, 5000));

            if (UnityEngine.Random.Range(0f, 6f) > 2f)
            {
                onAdLoaded?.Invoke();
                _isLoaded = true;
                Debug.Log("SuperMobileAdsRewarded Loaded " + _adUnitId);
            }
            else
            {
                onAdFailedToLoad?.Invoke();
                _isLoaded = false;
                Debug.Log("SuperMobileAdsRewarded Failed to Load " + _adUnitId);
            }
        }

        private async void ShowingBanner(  )
        {

            
            await Task.Delay(UnityEngine.Random.Range(100, 5000));

            onAdShown?.Invoke();
            _isShowing = true;
            Debug.Log("SuperMobileAdsRewarded Showed " + _adUnitId);

            if (UnityEngine.Random.Range(0f, 6f) > 5f)
            {
                await Task.Delay(UnityEngine.Random.Range(3000, 5000));
                onAdClicked?.Invoke();
            }
            else
            if (UnityEngine.Random.Range(0f, 6f) > 3f)
            {
                await Task.Delay(UnityEngine.Random.Range(4000, 5000));
                onAdRewarded?.Invoke();
                _isShowing = false;
            }
            else
            {
                await Task.Delay(UnityEngine.Random.Range(500, 5000));
                onAdDismissed?.Invoke();
                _isShowing = false;
            }
        }
    }
}
