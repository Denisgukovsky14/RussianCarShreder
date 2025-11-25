using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInfoManager : MonoBehaviourPun, IPunObservable
{
    public static PlayerInfoManager Instance;

    [Header("Player Tracking")]
    private Dictionary<int, PlayerInfo> players = new Dictionary<int, PlayerInfo>();

    //  ДЛЯ СИНХРОНИЗАЦИИ
    private List<int> actorNumbers = new List<int>();
    private List<string> nicknames = new List<string>();
    private List<int> healths = new List<int>();
    private List<bool> aliveStatuses = new List<bool>();

    // ВСЕ события - и старые и новые
    public System.Action<PlayerInfo> OnPlayerAdded;
    public System.Action<PlayerInfo> OnPlayerRemoved;
    public System.Action<PlayerInfo> OnPlayerUpdated;
    public System.Action<int> OnPlayerCountChanged;
    public System.Action<int> OnAlivePlayerCountChanged;

    // Свойства
    public int PlayerCount => players.Count;

    public int AlivePlayerCount
    {
        get
        {
            int count = 0;
            foreach (var player in players.Values)
            {
                if (player.IsAlive)
                    count++;
            }
            return count;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log(" PlayerInfoManager создан!");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //  ПРИ СТАРТЕ ОБНОВЛЯЕМ ДАННЫЕ ДЛЯ СИНХРОНИЗАЦИИ
        UpdateSyncData();
    }

    public void RegisterPlayer(PlayerInfo playerInfo)
    {
        int oldCount = players.Count;
        int actorNumber = playerInfo.photonView.OwnerActorNr;

        if (!players.ContainsKey(actorNumber))
        {
            players[actorNumber] = playerInfo;

            Debug.Log($" PlayerInfoManager: Игрок {playerInfo.Nickname} зарегистрирован (ID: {actorNumber})");

            //  ОБНОВЛЯЕМ ДАННЫЕ ДЛЯ СИНХРОНИЗАЦИИ
            UpdateSyncData();

            OnPlayerAdded?.Invoke(playerInfo);
            Debug.Log($" PlayerInfoManager: OnPlayerAdded вызван (подписчиков: {OnPlayerAdded?.GetInvocationList().Length ?? 0})");

            if (oldCount != players.Count)
            {
                OnPlayerCountChanged?.Invoke(players.Count);
                OnAlivePlayerCountChanged?.Invoke(AlivePlayerCount);
            }
        }
        else
        {
            Debug.Log($" PlayerInfoManager: Игрок {actorNumber} уже зарегистрирован");
        }
    }

    public void UnregisterPlayer(PlayerInfo playerInfo)
    {
        int oldCount = players.Count;
        int actorNumber = playerInfo.photonView.OwnerActorNr;

        if (players.ContainsKey(actorNumber))
        {
            Debug.Log($" PlayerInfoManager: Удаляем игрока {playerInfo.Nickname} (ID: {actorNumber})");

            players.Remove(actorNumber);

            //  ОБНОВЛЯЕМ ДАННЫЕ ДЛЯ СИНХРОНИЗАЦИИ
            UpdateSyncData();

            OnPlayerRemoved?.Invoke(playerInfo);

            if (oldCount != players.Count)
            {
                OnPlayerCountChanged?.Invoke(players.Count);
                OnAlivePlayerCountChanged?.Invoke(AlivePlayerCount);
            }
        }
    }

    //  ОБНОВЛЯЕМ ДАННЫЕ ДЛЯ СИНХРОНИЗАЦИИ
    void UpdateSyncData()
    {
        actorNumbers.Clear();
        nicknames.Clear();
        healths.Clear();
        aliveStatuses.Clear();

        foreach (var kvp in players)
        {
            actorNumbers.Add(kvp.Key);
            nicknames.Add(kvp.Value.Nickname);
            healths.Add(kvp.Value.Health);
            aliveStatuses.Add(kvp.Value.IsAlive);
        }
    }

    //  СИНХРОНИЗАЦИЯ ЧЕРЕЗ PHOTON
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //  ОТПРАВЛЯЕМ ДАННЫЕ ВСЕХ ИГРОКОВ
            stream.SendNext(actorNumbers.Count);

            for (int i = 0; i < actorNumbers.Count; i++)
            {
                stream.SendNext(actorNumbers[i]);
                stream.SendNext(nicknames[i]);
                stream.SendNext(healths[i]);
                stream.SendNext(aliveStatuses[i]);
            }

            Debug.Log($" Отправлено игроков: {actorNumbers.Count}");
        }
        else
        {
            //  ПОЛУЧАЕМ ДАННЫЕ ВСЕХ ИГРОКОВ
            int count = (int)stream.ReceiveNext();
            Debug.Log($" Получено игроков: {count}");

            // Временный словарь для новых данных
            var newPlayers = new Dictionary<int, PlayerInfo>();

            for (int i = 0; i < count; i++)
            {
                int actorNumber = (int)stream.ReceiveNext();
                string nickname = (string)stream.ReceiveNext();
                int health = (int)stream.ReceiveNext();
                bool alive = (bool)stream.ReceiveNext();

                //  СОЗДАЕМ ИЛИ ОБНОВЛЯЕМ ИГРОКА
                if (players.ContainsKey(actorNumber))
                {
                    // Обновляем существующего
                    var player = players[actorNumber];
                    // Данные уже синхронизируются через NetVar, но можем обновить тут если нужно
                    newPlayers[actorNumber] = player;
                }
                else
                {
                    //  СОЗДАЕМ ВИРТУАЛЬНОГО ИГРОКА ДЛЯ ОТОБРАЖЕНИЯ
                    Debug.Log($" Создан виртуальный игрок: {nickname} (ID: {actorNumber})");
                    var virtualPlayer = CreateVirtualPlayer(actorNumber, nickname, health, alive);
                    newPlayers[actorNumber] = virtualPlayer;
                }
            }

            //  ОБНОВЛЯЕМ СПИСОК ИГРОКОВ
            UpdatePlayersFromSync(newPlayers);
        }
    }

    //  СОЗДАЕМ ВИРТУАЛЬНОГО ИГРОКА ДЛЯ ОТОБРАЖЕНИЯ
    PlayerInfo CreateVirtualPlayer(int actorNumber, string nickname, int health, bool alive)
    {
        GameObject playerObj = new GameObject($"VirtualPlayer_{actorNumber}");
        PlayerInfo playerInfo = playerObj.AddComponent<PlayerInfo>();

        //  ИНИЦИАЛИЗИРУЕМ ДАННЫЕ (нужно добавить методы для этого в PlayerInfo)
        // playerInfo.InitializeRemotePlayer(nickname, health, alive);

        return playerInfo;
    }

    //  ОБНОВЛЯЕМ СПИСОК ИГРОКОВ ИЗ СИНХРОНИЗИРОВАННЫХ ДАННЫХ
    void UpdatePlayersFromSync(Dictionary<int, PlayerInfo> newPlayers)
    {
        // Удаляем игроков которых больше нет
        List<int> toRemove = new List<int>();
        foreach (var kvp in players)
        {
            if (!newPlayers.ContainsKey(kvp.Key))
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (int actorNumber in toRemove)
        {
            if (players.ContainsKey(actorNumber))
            {
                var player = players[actorNumber];
                players.Remove(actorNumber);
                OnPlayerRemoved?.Invoke(player);
            }
        }

        // Добавляем новых игроков
        foreach (var kvp in newPlayers)
        {
            if (!players.ContainsKey(kvp.Key))
            {
                players[kvp.Key] = kvp.Value;
                OnPlayerAdded?.Invoke(kvp.Value);
            }
        }

        Debug.Log($" Синхронизировано игроков: {players.Count}");
    }

    public void UpdatePlayerDisplay(PlayerInfo playerInfo)
    {
        OnPlayerUpdated?.Invoke(playerInfo);
    }

    // Остальные методы без изменений
    public PlayerInfo GetPlayerInfo(int actorNumber)
    {
        players.TryGetValue(actorNumber, out PlayerInfo info);
        return info;
    }

    public Dictionary<int, PlayerInfo> GetAllPlayers()
    {
        Debug.Log($" GetAllPlayers: возвращаю {players.Count} игроков");
        foreach (var kvp in players)
        {
            Debug.Log($"   - ID: {kvp.Key}, Ник: {kvp.Value.Nickname}, IsMine: {kvp.Value.photonView.IsMine}");
        }
        return new Dictionary<int, PlayerInfo>(players);
    }

    public string GetPlayerNickname(int actorNumber)
    {
        if (players.TryGetValue(actorNumber, out PlayerInfo info))
        {
            return info.Nickname;
        }
        return "Unknown Player";
    }

    public int GetPlayerHealth(int actorNumber)
    {
        if (players.TryGetValue(actorNumber, out PlayerInfo info))
        {
            return info.Health;
        }
        return 0;
    }

    public bool GetPlayerAliveStatus(int actorNumber)
    {
        if (players.TryGetValue(actorNumber, out PlayerInfo info))
        {
            return info.IsAlive;
        }
        return false;
    }

    public void PrintAllPlayers()
    {
        Debug.Log($"=== Всего игроков: {PlayerCount} ===");
        foreach (var kvp in players)
        {
            PlayerInfo player = kvp.Value;
            Debug.Log($"ID: {kvp.Key}, Ник: {player.Nickname}, HP: {player.Health}, Жив: {player.IsAlive}");
        }
    }
}