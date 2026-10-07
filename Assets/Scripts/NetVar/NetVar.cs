using Photon.Pun;
using System;

[System.Serializable]
public class NetVar<T>
{
    private T _value;
    private T _lastSentValue;

    public T Value
    {
        get => _value;
        set
        {
            if (!Equals(_value, value))
            {
                T oldValue = _value;
                _value = value;
                OnValueChanged?.Invoke(oldValue, value);
            }
        }
    }

    public event Action<T, T> OnValueChanged;

    // йНМЯРПСЙРНП
    public NetVar(T initialValue = default(T))
    {
        _value = initialValue;
        _lastSentValue = _value;
    }

    // лЕРНДШ ДКЪ ЯХМУПНМХГЮЖХХ
    public void Serialize(PhotonStream stream)
    {
        if (stream.IsWriting)
        {
            //  опнярн нропюбкъел рейсыее гмювемхе
            stream.SendNext(_value);
        }
        else
        {
            //  опнярн вхрюел аег кчашу опнбепнй
            try
            {
                T receivedValue = (T)stream.ReceiveNext();
                if (!Equals(_value, receivedValue))
                {
                    T oldValue = _value;
                    _value = receivedValue;
                    OnValueChanged?.Invoke(oldValue, receivedValue);
                }
            }
            catch
            {
                //  хцмнпхпсел ньхайх - Photon яюл пюгаеп╗ряъ
            }
        }
    }
}