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

    // Пробный NetVar
    [SerializeField] private NetVar<int> _playerId = new NetVar<int>(0);

    private int _actorNumber;

    public int ActorNumber => _actorNumber;
    public string Nickname => _nickname.Value;

    public int PlayerId
    {
        get => _playerId.Value;
        set { if (photonView.IsMine) _playerId.Value = value; }
    }

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



    private bool _isInitialized = false;

    void Start()
    {
        _actorNumber = photonView.OwnerActorNr;

        if (photonView.IsMine)
        {
            _nickname.Value = PhotonNetwork.NickName ?? "Player_" + photonView.OwnerActorNr;
            _health.Value = 100;
            _isAlive.Value = true;

            _playerId.Value = ActorNumber;

            Debug.Log($"Игрок инициализирован: {_nickname.Value}, HP: {_health.Value}");
        }

        // Подписка на события NetVar
        _health.OnValueChanged += OnHealthChanged;
        _isAlive.OnValueChanged += OnAliveStatusChanged;
        _nickname.OnValueChanged += OnNicknameChanged;
        _playerId.OnValueChanged += OnPlayerIdChanged;

        // Регистрация в менеджере
        RegisterWithManager();

        _isInitialized = true;

        // Принудительное обновление отображения
        UpdateNicknameDisplay();
        UpdatePlayerUI();
    }

    private void OnNicknameChanged(string oldNickname, string newNickname)
    {
        Debug.Log($"Никнейм изменен: {oldNickname} -> {newNickname}");
        UpdateNicknameDisplay();
        UpdatePlayerUI();
    }

    private void OnPlayerIdChanged(int oldId, int newId)
    {
        Debug.Log($"ID игрока изменен: {oldId} -> {newId}");
        UpdatePlayerUI();
    }

    private void UpdateNicknameDisplay()
    {
        if (nick != null)
        {
            nick.text = _nickname.Value;
            Debug.Log($"Обновление отображения ника: {_nickname.Value}");
        }
        else
        {
            Debug.LogWarning("TMP_Text компонент 'nick' не назначен!");
        }
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        Debug.Log($"Здоровье изменилось: {oldHealth} -> {newHealth}");

        if (newHealth <= 0 && oldHealth > 0)
        {
            _isAlive.Value = false;
        }

        UpdatePlayerUI();
    }

    private void OnAliveStatusChanged(bool oldStatus, bool newStatus)
    {
        Debug.Log($"Статус жизни изменился: {oldStatus} -> {newStatus}");

        if (!newStatus)
        {
            OnPlayerDeath();
        }

        UpdatePlayerUI();
    }

    private void OnPlayerDeath()
    {
        Debug.Log($"Игрок {_nickname.Value} умер!");
    }

    private void UpdatePlayerUI()
    {
        PlayerInfoManager.Instance?.UpdatePlayerDisplay(this);
    }

    private void RegisterWithManager()
    {
        if (PlayerInfoManager.Instance != null)
        {
            PlayerInfoManager.Instance.RegisterPlayer(this);
        }
        else
        {
            Debug.LogWarning("PlayerInfoManager.Instance is null, повторная попытка...");
            Invoke(nameof(RegisterWithManager), 0.1f);
        }
    }

    public void SetNickname(string nickname)
    {
        if (photonView.IsMine)
            _nickname.Value = nickname;
    }

    public void TakeDamage(int damage)
    {
        if (photonView.IsMine && _isAlive.Value)
        {
            _health.Value = Mathf.Max(0, _health.Value - damage);
        }
    }

    public void Heal(int amount)
    {
        if (photonView.IsMine && _isAlive.Value)
        {
            _health.Value += amount;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_nickname.Value);
            stream.SendNext(_health.Value);
            stream.SendNext(_isAlive.Value);
            stream.SendNext(_playerId.Value);
        }
        else
        {
            string receivedNickname = (string)stream.ReceiveNext();
            int receivedHealth = (int)stream.ReceiveNext();
            bool receivedAlive = (bool)stream.ReceiveNext();
            int receivedPlayerId = (int)stream.ReceiveNext();

            // Обновляем значения только если они изменились
            if (_nickname.Value != receivedNickname)
            {
                _nickname.Value = receivedNickname;
            }

            if (_health.Value != receivedHealth)
            {
                _health.Value = receivedHealth;
            }

            
            _isAlive.Value = receivedAlive;

            _playerId.Value = receivedPlayerId;

            // Принудительное обновление при получении данных
            if (!_isInitialized)
            {
                UpdateNicknameDisplay();
                UpdatePlayerUI();
            }
        }
    }

    public void InitializeRemotePlayer(string nickname, int health, bool alive, int actorNumber)
    {
        _nickname.Value = nickname;
        _health.Value = health;
        _isAlive.Value = alive;
        _actorNumber = actorNumber;

        UpdateNicknameDisplay();
        UpdatePlayerUI();
    }

    public void SetPlayerId(int id)
    {
        if (photonView.IsMine)
            _playerId.Value = id;
    }

    void OnDestroy()
    {
        PlayerInfoManager.Instance?.UnregisterPlayer(this);
    }
}