using Photon.Pun;
using UnityEngine;
using TMPro;

public class PlayerInfo : MonoBehaviourPun, IPunObservable
{
    [Header("Player Data")]
    [SerializeField] private TMP_Text nick;
    [SerializeField] private NetVar<string> _nickname = new NetVar<string>("Unknown");
    [SerializeField] private NetVar<int> _health = new NetVar<int>(100);
    [SerializeField] private NetVar<bool> _isAlive = new NetVar<bool>(true);
    private int _actorNumber;

    public int ActorNumber => _actorNumber;

    // СВОЙСТВА ДЛЯ ДОСТУПА ИЗВНЕ
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

    [Header("Network Sync")]
    public float syncInterval = 0.1f;
    private float lastSyncTime = 0f;
    private bool isNicknameInitialized = false;

    void Start()
    {
        // Устанавливаем данные для локального игрока
        if (photonView.IsMine)
        {
            _nickname.Value = PhotonNetwork.NickName ?? "Player_" + photonView.OwnerActorNr;
            _actorNumber = photonView.OwnerActorNr;
            _health.Value = 100;
            _isAlive.Value = true;

            Debug.Log($" Игрок создан: {_nickname.Value}, HP: {_health.Value}");

            // Синхронизируем никнейм сразу при создании
            photonView.RPC("RPC_SyncNickname", RpcTarget.AllBuffered, _nickname.Value, _actorNumber);

            UpdateNicknameDisplay();
        }

        // ПОДПИСКА НА СОБЫТИЯ NETVAR
        _health.OnValueChanged += OnHealthChanged;
        _isAlive.OnValueChanged += OnAliveStatusChanged;
        _nickname.OnValueChanged += OnNicknameChanged;

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

    void Update()
    {
        // Периодическая синхронизация критических данных
        if (photonView.IsMine && Time.time - lastSyncTime > syncInterval)
        {
            lastSyncTime = Time.time;
            // Принудительная синхронизация никнейма
            if (!isNicknameInitialized)
            {
                photonView.RPC("RPC_SyncNickname", RpcTarget.Others, _nickname.Value, _actorNumber);
                isNicknameInitialized = true;
            }
        }
    }

    [PunRPC]
    void RPC_SyncNickname(string nickname, int actorNumber)
    {
        Debug.Log($"RPC_SyncNickname received: {nickname} for actor {actorNumber}");

        _nickname.Value = nickname;
        _actorNumber = actorNumber;
        UpdateNicknameDisplay();

        // Обновляем в менеджере
        PlayerInfoManager.Instance?.UpdatePlayerDisplay(this);
    }

    private void OnNicknameChanged(string oldNickname, string newNickname)
    {
        Debug.Log($" Никнейм изменен: {oldNickname} -> {newNickname}");
        UpdateNicknameDisplay();

        // Синхронизируем изменение никнейма
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_SyncNickname", RpcTarget.AllBuffered, newNickname, _actorNumber);
        }
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

    private void UpdateNicknameDisplay()
    {
        if (nick != null)
        {
            nick.text = _nickname.Value;
            Debug.Log($" Никнейм отображен: {_nickname.Value}");
        }
        else
        {
            Debug.LogWarning(" TMP_Text компонент 'nick' не назначен!");
        }
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

    // ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ ИЗМЕНЕНИЯ ДАННЫХ
    public void SetNickname(string nickname)
    {
        if (photonView.IsMine)
        {
            _nickname.Value = nickname;
            // Автоматически синхронизируется через OnNicknameChanged
        }
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

    // СИНХРОНИЗАЦИЯ ЧЕРЕЗ PHOTON
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        _nickname.Serialize(stream);
        _health.Serialize(stream);
        _isAlive.Serialize(stream);

        if (stream.IsWriting)
        {
            stream.SendNext(_actorNumber);
        }
        else
        {
            _actorNumber = (int)stream.ReceiveNext();
        }
    }

    public void InitializeRemotePlayer(string nickname, int health, bool alive, int actorNumber)
    {
        _nickname.Value = nickname;
        _health.Value = health;
        _isAlive.Value = alive;
        _actorNumber = actorNumber;
        UpdateNicknameDisplay();
    }

    void OnDestroy()
    {
        PlayerInfoManager.Instance?.UnregisterPlayer(this);
    }
}