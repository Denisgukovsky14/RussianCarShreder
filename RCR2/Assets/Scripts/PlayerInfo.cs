using Photon.Pun;
using UnityEngine;

public class PlayerInfo : MonoBehaviourPun, IPunObservable
{
    [Header("Player Data")]
    [SerializeField] private NetVar<string> _nickname = new NetVar<string>("Unknown");
    [SerializeField] private NetVar<int> _health = new NetVar<int>(100);
    [SerializeField] private NetVar<bool> _isAlive = new NetVar<bool>(true);

    //  СВОЙСТВА ДЛЯ ДОСТУПА ИЗВНЕ
    public string Nickname => _nickname.Value;
    public int Health
    {
        get => _health.Value;
        set { if (photonView.IsMine) _health.Value = value; }
    }
    public bool IsAlive
    {
        get => _isAlive.Value;
        set { if (photonView.IsMine) _isAlive.Value = value; }
    }

    //public NetVar<string> playerNickname => _nickname;
    //public NetVar<int> playerHealth => _health;
    //public NetVar<bool> isAlive => _isAlive;

    public void MyDeath(string nickname)
    {
        if (photonView.IsMine)
        {
            Health = 0;
            IsAlive = false;
        }
    }

    void Start()
    {
        // Устанавливаем данные для локального игрока
        if (photonView.IsMine)
        {
            _nickname.Value = PhotonNetwork.NickName ?? "Player_" + photonView.OwnerActorNr;
            _health.Value = 100;
            _isAlive.Value = true;

            Debug.Log($" Игрок создан: {_nickname.Value}, HP: {_health.Value}");
        }

        //  ПОДПИСКА НА СОБЫТИЯ NETVAR
        _health.OnValueChanged += OnHealthChanged;
        _isAlive.OnValueChanged += OnAliveStatusChanged;

        // Регистрируем в менеджере
        if (PlayerInfoManager.Instance != null)
        {
            PlayerInfoManager.Instance.RegisterPlayer(this);
        }
        else
        {
            Debug.LogError(" PlayerInfoManager.Instance is null!");
        }

        Debug.Log(" PlayerInfo инициализирован!");
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        Debug.Log($" Здоровье: {oldHealth} -> {newHealth}");

        // Проверяем смерть
        if (newHealth <= 0 && oldHealth > 0)
        {
            _isAlive.Value = false;
        }

        UpdatePlayerUI();
    }

    private void OnAliveStatusChanged(bool oldStatus, bool newStatus)
    {
        Debug.Log($" Статус: {oldStatus} -> {newStatus}");

        if (!newStatus)
        {
            OnPlayerDeath();
        }

        UpdatePlayerUI();
    }

    private void OnPlayerDeath()
    {
        Debug.Log($" Игрок {_nickname.Value} умер!");
        // Дополнительная логика смерти...
    }

    private void UpdatePlayerUI()
    {
        PlayerInfoManager.Instance?.UpdatePlayerDisplay(this);
    }

    //  ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ ИЗМЕНЕНИЯ ДАННЫХ
    public void SetNickname(string nickname)
    {
        if (photonView.IsMine)
            _nickname.Value = nickname;
    }

    public void TakeDamage(int damage)
    {
        if (photonView.IsMine && _isAlive.Value)
        {
            _health.Value -= damage;
        }
    }

    public void Heal(int amount)
    {
        if (photonView.IsMine && _isAlive.Value)
        {
            _health.Value += amount;
        }
    }

    //  СИНХРОНИЗАЦИЯ ЧЕРЕЗ PHOTON
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        _nickname.Serialize(stream);
        _health.Serialize(stream);
        _isAlive.Serialize(stream);
    }

    // В PlayerInfo.cs добавь:
    public void InitializeRemotePlayer(string nickname, int health, bool alive)
    {
        _nickname.Value = nickname;
        _health.Value = health;
        _isAlive.Value = alive;
    }

    void OnDestroy()
    {
        PlayerInfoManager.Instance?.UnregisterPlayer(this);
    }
}