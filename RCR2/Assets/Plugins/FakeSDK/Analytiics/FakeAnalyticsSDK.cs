using System;
using System.Collections.Generic;

using UnityEngine;

namespace FakeAnalytics
{
    public class FakeAnalyticsSDK
    {
        public bool IsInitialized()
        {
            return _isInitialized;
        }
        public void Initialize(
            string appKey,
            string userId)
        {
            Debug.Log("FakeAnalyticsSDK.Initialize with key: " + appKey + ", user id: " + userId);
            _isInitialized = true;
        }

        public void TrackEvent(string name, IDictionary<string, string> eventParams = null)
        {
            if (!_isInitialized)
            {
                Debug.LogError("FakeAnalyticsSDK.TrackEvent called before initialization");
                return;
            }

            Debug.Log("FakeAnalyticsSDK.TrackEvent " + name);
            if (eventParams != null)
            {
                foreach (KeyValuePair<string, string> pair in eventParams)
                {
                    Debug.Log(" " + pair.ToString());
                }
            }
        }

        public void TrackGameStartEvent()
        {
            if (!_isInitialized)
            {
                Debug.LogError("FakeAnalyticsSDK.TrackGameStartEvent called before initialization");
                return;
            }

            Debug.Log("FakeAnalyticsSDK.TrackGameStartEvent game started");
        }

        public void TrackLevelEvent(int level, IDictionary<string, string> eventParams = null)
        {
            if (!_isInitialized)
            {
                Debug.LogError("FakeAnalyticsSDK.TrackLevelEvent called before initialization");
                return;
            }

            Debug.Log("FakeAnalyticsSDK.TrackLevelEvent level " + level);
            if (eventParams != null)
            {
                foreach (KeyValuePair<string, string> pair in eventParams)
                {
                    Debug.Log(" " + pair.ToString());
                }
            }
        }

        public void Flush()
        {
            if (!_isInitialized)
            {
                Debug.LogError("FakeAnalyticsSDK.Flush called before initialization");
                return;
            }

            Debug.Log("FakeAnalyticsSDK.Flush");
        }

        private bool _isInitialized = false;
    }
}
