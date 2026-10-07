using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviourPun, IPunObservable
{
    public static PlayerInfoManager Instance;

    private Dictionary<int, PlayerInfo> players = new Dictionary<int, PlayerInfo>();

    public System.Action<PlayerInfo> OnPlayerAdded;
    public System.Action<PlayerInfo> OnPlayerRemoved;
    public System.Action<PlayerInfo> OnPlayerUpdated;
    public System.Action<int> OnPlayerCountChanged;
    public System.Action<int> OnAlivePlayerCountChanged;

    public int PlayerCount => players.Count;

    public int AlivePlayerCount
    {
        get
        {
            int count = 0;
            foreach (var player in players.Values)
                if (player.IsAlive)
                    count++;
            return count;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("PlayerInfoManager создан!");
        }
        else
            Destroy(gameObject);
    }

    public void RegisterPlayer(PlayerInfo playerInfo)
    {
        int oldCount = players.Count;
        int actorNumber = playerInfo.PlayerId;

        if (!players.ContainsKey(actorNumber))
        {
            players[actorNumber] = playerInfo;

            Debug.Log($"PlayerInfoManager: Игрок {playerInfo.Nickname} зарегистрирован (ID: {actorNumber})");

            OnPlayerAdded?.Invoke(playerInfo);

            if (oldCount != players.Count)
            {
                OnPlayerCountChanged?.Invoke(players.Count);
                OnAlivePlayerCountChanged?.Invoke(AlivePlayerCount);
            }

            Debug.Log($"Всего игроков: {players.Count}");
            PrintAllPlayers();
        }
    }

    public void UnregisterPlayer(PlayerInfo playerInfo)
    {
        int oldCount = players.Count;
        int actorNumber = playerInfo.ActorNumber;

        if (players.ContainsKey(actorNumber))
        {
            players.Remove(actorNumber);

            OnPlayerRemoved?.Invoke(playerInfo);

            if (oldCount != players.Count)
            {
                OnPlayerCountChanged?.Invoke(players.Count);
                OnAlivePlayerCountChanged?.Invoke(AlivePlayerCount);
            }
        }
    }

    public PlayerInfo CreateVirtualPlayer(int actorNumber, string nickname, int health, bool alive)
    {
        GameObject playerObj = new GameObject($"VirtualPlayer_{actorNumber}");
        PlayerInfo playerInfo = playerObj.AddComponent<PlayerInfo>();
        playerInfo.InitializeRemotePlayer(nickname, health, alive, actorNumber);
        return playerInfo;
    }

    public void UpdatePlayerDisplay(PlayerInfo playerInfo)
    {
        OnPlayerUpdated?.Invoke(playerInfo);
    }

    public Dictionary<int, PlayerInfo> GetAllPlayers()
    {
        Debug.Log($"GetAllPlayers: возвращаю {players.Count} игроков");

        Dictionary<int, PlayerInfo> copy = new Dictionary<int, PlayerInfo>(players);

        foreach (var kvp in copy)
        {
            Debug.Log($"   - ID: {kvp.Key}, Ник: {kvp.Value.Nickname}, IsMine: {kvp.Value.photonView?.IsMine}");
        }

        return copy;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Упрощенная синхронизация - менеджер теперь в основном полагается на синхронизацию отдельных PlayerInfo
        if (stream.IsWriting)
        {
            // Мы пишем данные
            stream.SendNext(players.Count);
            foreach (var kvp in players)
            {
                stream.SendNext(kvp.Key);
                stream.SendNext(kvp.Value.Nickname);
                stream.SendNext(kvp.Value.Health);
                stream.SendNext(kvp.Value.IsAlive);
            }
        }
        else
        {
            // Мы отправляем данные
            int count = (int)stream.ReceiveNext();
            var receivedPlayers = new Dictionary<int, (string nickname, int health, bool alive, int id)>();

            for (int i = 0; i < count; i++)
            {
                int actorNumber = (int)stream.ReceiveNext();
                string nickname = (string)stream.ReceiveNext();
                int health = (int)stream.ReceiveNext();
                bool alive = (bool)stream.ReceiveNext();
                int id = (int)stream.ReceiveNext();

                receivedPlayers[actorNumber] = (nickname, health, alive, id);
            }

            // Обновляем существующих игроков и добавляем недостающих
            foreach (var kvp in receivedPlayers)
            {
                if (players.ContainsKey(kvp.Key))
                {
                    // Обновляем данные существующего игрока
                    var player = players[kvp.Key];
                    if (player.Nickname != kvp.Value.nickname)
                    {
                        player.InitializeRemotePlayer(kvp.Value.nickname, kvp.Value.health, kvp.Value.alive, kvp.Key);
                    }
                }
                else
                {
                    // Создаем виртуального игрока для отсутствующего
                    var virtualPlayer = CreateVirtualPlayer(kvp.Key, kvp.Value.nickname, kvp.Value.health, kvp.Value.alive);
                    RegisterPlayer(virtualPlayer);
                }
            }
        }
    }
}