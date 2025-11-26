using Photon.Pun;
using System;
using UnityEngine;

[System.Serializable]
public class NetVar<T>
{
    [SerializeField] private T _value;
    private T _lastSyncedValue;
    private bool _forceSync;

    public T Value
    {
        get => _value;
        set
        {
            if (!Equals(_value, value))
            {
                T oldValue = _value;
                _value = value;
                _forceSync = true;
                OnValueChanged?.Invoke(oldValue, value);
            }
        }
    }

    public event Action<T, T> OnValueChanged;

    public NetVar(T initialValue = default(T))
    {
        _value = initialValue;
        _lastSyncedValue = _value;
    }

    public bool ShouldSync()
    {
        return _forceSync || !Equals(_value, _lastSyncedValue);
    }

    public void Serialize(PhotonStream stream)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_value);
            _lastSyncedValue = _value;
            _forceSync = false;
        }
        else
        {
            try
            {
                T receivedValue = (T)stream.ReceiveNext();
                if (!Equals(_value, receivedValue))
                {
                    T oldValue = _value;
                    _value = receivedValue;
                    _lastSyncedValue = _value;
                    OnValueChanged?.Invoke(oldValue, receivedValue);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"NetVar deserialization error: {e}");
            }
        }
    }

    public void ForceSync()
    {
        _forceSync = true;
    }
}