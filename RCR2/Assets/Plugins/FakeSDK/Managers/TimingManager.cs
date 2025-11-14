// TimingManager.cs
using UnityEngine;

public class TimingManager : MonoBehaviour
{
    public float interstitialInterval = 60f; // раз в минуту
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interstitialInterval)
        {
            AdManager.Instance.ShowInterstitial();
            timer = 0f;
        }
    }
}