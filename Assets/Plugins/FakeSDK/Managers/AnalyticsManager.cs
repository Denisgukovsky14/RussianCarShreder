using UnityEngine;
using System.Collections.Generic;
using FakeAnalytics;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private FakeAnalyticsSDK _fakeSdk; // Теперь храним экземпляр

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAnalytics();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAnalytics()
    {
        // Создаем экземпляр SDK
        _fakeSdk = new FakeAnalyticsSDK();

        // Инициализируем с двумя параметрами
        _fakeSdk.Initialize("your_game_key", "user_123");

        // Сразу отправляем событие о запуске игры
        TrackGameStart();
    }

    public void TrackGameStart()
    {
        var parameters = new Dictionary<string, string>
        {
            {"version", Application.version},
            {"platform", Application.platform.ToString()}
        };

        _fakeSdk.TrackEvent("game_start", parameters);
    }

    public void TrackLevelEvent(int levelId, string eventType)
    {
        var parameters = new Dictionary<string, string>
        {
            {"level_id", levelId.ToString()}, // Все значения должны быть string!
            {"event_type", eventType}
        };

        _fakeSdk.TrackEvent("level_event", parameters);
    }

    public void TrackUserEvent(string eventName, Dictionary<string, string> customParams = null)
    {
        _fakeSdk.TrackEvent(eventName, customParams);
    }

    // Вызывается при сворачивании игры
    private void OnApplicationFocus(bool hasFocus)
    {
        Debug.Log($"Focus changed: {hasFocus}");

        if (!hasFocus)
        {
            _fakeSdk.Flush();
            Debug.Log("Flush called - data sent to server");
        }
    }
}