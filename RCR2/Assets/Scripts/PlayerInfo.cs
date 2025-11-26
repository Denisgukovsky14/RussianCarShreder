using Photon.Pun;
using UnityEngine;
using TMPro;

public class PlayerInfo : MonoBehaviourPun, IPunObservable
{
    [Header("Player Data")]
    [SerializeField] private TMP_Text nick;

    // NetVar для автоматической синхронизации
    [SerializeField] private NetVar<string> _nickname = new NetVar<string>("Unknown");
    [SerializeField] private NetVar<int> _health = new NetVar<int>(100);
    [SerializeField] private NetVar<bool> _isAlive = new NetVar<bool>(true);
    [SerializeField] private NetVar<int> _kills = new NetVar<int>(0);
    [SerializeField] private NetVar<int> _deaths = new NetVar<int>(0);

    private int _actorNumber;
    private bool _isInitialized = false;

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

<<<<<<< Updated upstream
    [Header("Network Sync")]
    public float syncInterval = 0.1f;
    private float lastSyncTime = 0f;
    private bool isNicknameInitialized = false;
=======
    public int Kills => _kills.Value;
    public int Deaths => _deaths.Value;
>>>>>>> Stashed changes

    void Start()
    {
        // Устанавливаем данные для локального игрока
        if (photonView.IsMine)
        {
<<<<<<< Updated upstream
            _nickname.Value = PhotonNetwork.NickName ?? "Player_" + photonView.OwnerActorNr;
            _actorNumber = photonView.OwnerActorNr;
=======
            // Установка никнейма с сервера
            _nickname.Value = string.IsNullOrEmpty(PhotonNetwork.NickName)
                ? "Player_" + photonView.OwnerActorNr
                : PhotonNetwork.NickName;

>>>>>>> Stashed changes
            _health.Value = 100;
            _isAlive.Value = true;
            _kills.Value = 0;
            _deaths.Value = 0;

            Debug.Log($" Игрок создан: {_nickname.Value}, HP: {_health.Value}");

            // Синхронизируем никнейм сразу при создании
            photonView.RPC("RPC_SyncNickname", RpcTarget.AllBuffered, _nickname.Value, _actorNumber);

            UpdateNicknameDisplay();
        }

<<<<<<< Updated upstream
        // ПОДПИСКА НА СОБЫТИЯ NETVAR
=======
        // Подписка на события NetVar
        _nickname.OnValueChanged += OnNicknameChanged;
>>>>>>> Stashed changes
        _health.OnValueChanged += OnHealthChanged;
        _isAlive.OnValueChanged += OnAliveStatusChanged;
        _kills.OnValueChanged += OnKillsChanged;
        _deaths.OnValueChanged += OnDeathsChanged;

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

<<<<<<< Updated upstream
        // Синхронизируем изменение никнейма
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_SyncNickname", RpcTarget.AllBuffered, newNickname, _actorNumber);
        }
    }

=======
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
=======
    private void OnKillsChanged(int oldKills, int newKills)
    {
        Debug.Log($"Количество убийств изменилось: {oldKills} -> {newKills}");
        UpdatePlayerUI();
    }

    private void OnDeathsChanged(int oldDeaths, int newDeaths)
    {
        Debug.Log($"Количество смертей изменилось: {oldDeaths} -> {newDeaths}");
        UpdatePlayerUI();
    }

>>>>>>> Stashed changes
    private void UpdateNicknameDisplay()
    {
        if (nick != null)
        {
            nick.text = _nickname.Value;
<<<<<<< Updated upstream
            Debug.Log($" Никнейм отображен: {_nickname.Value}");
        }
        else
        {
            Debug.LogWarning(" TMP_Text компонент 'nick' не назначен!");
=======
            Debug.Log($"Обновление отображения ника: {_nickname.Value}");
        }
        else
        {
            Debug.LogWarning("TMP_Text компонент 'nick' не назначен!");
>>>>>>> Stashed changes
        }
    }

    private void OnPlayerDeath()
    {
<<<<<<< Updated upstream
        Debug.Log($" Игрок {_nickname.Value} умер!");
        // Дополнительная логика смерти...
=======
        Debug.Log($"Игрок {_nickname.Value} умер!");
        // Здесь можно добавить визуальные эффекты смерти
>>>>>>> Stashed changes
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

    public void TakeDamage(int damage, int attackerActorNumber = -1)
    {
        if (photonView.IsMine && _isAlive.Value)
        {
<<<<<<< Updated upstream
            _health.Value -= damage;
=======
            _health.Value = Mathf.Max(0, _health.Value - damage);

            // Если игрок умер и указан атакующий
            if (_health.Value <= 0 && attackerActorNumber != -1 && attackerActorNumber != _actorNumber)
            {
                // Награждаем убийцу
                PlayerInfo attackerInfo = PlayerInfoManager.Instance?.GetPlayer(attackerActorNumber);
                if (attackerInfo != null)
                {
                    attackerInfo.AddKill();
                }
            }
>>>>>>> Stashed changes
        }
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, -1);
    }

    public void AddKill()
    {
        if (photonView.IsMine)
            _kills.Value++;
    }

    public void Heal(int amount)
    {
        if (photonView.IsMine && _isAlive.Value)
        {
            _health.Value = Mathf.Min(100, _health.Value + amount);
        }
    }

    public void Respawn()
    {
        if (photonView.IsMine)
        {
            _health.Value = 100;
            _isAlive.Value = true;
            Debug.Log($"Игрок {_nickname.Value} возродился");

            // Здесь можно добавить логику респавна позиции
            // Например: transform.position = GetSpawnPosition();
        }
    }

    // СИНХРОНИЗАЦИЯ ЧЕРЕЗ PHOTON
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
<<<<<<< Updated upstream
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
=======
        // Синхронизация только через NetVar
        _nickname.Serialize(stream);
        _health.Serialize(stream);
        _isAlive.Serialize(stream);
        _kills.Serialize(stream);
        _deaths.Serialize(stream);

        // Принудительное обновление при получении данных
        if (!stream.IsWriting && !_isInitialized)
        {
            UpdateNicknameDisplay();
            UpdatePlayerUI();
>>>>>>> Stashed changes
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
        // Отписываемся от событий
        _nickname.OnValueChanged -= OnNicknameChanged;
        _health.OnValueChanged -= OnHealthChanged;
        _isAlive.OnValueChanged -= OnAliveStatusChanged;
        _kills.OnValueChanged -= OnKillsChanged;
        _deaths.OnValueChanged -= OnDeathsChanged;

        PlayerInfoManager.Instance?.UnregisterPlayer(this);
    }
}