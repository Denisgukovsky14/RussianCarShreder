using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Photon.Pun;

public class PlayerListUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform playerListContent;
    public GameObject playerListItem;

    [Header("UI Texts")]
    public TMP_Text playerCountText;
    public TMP_Text roomInfoText;

    private Dictionary<int, GameObject> playerListItems = new Dictionary<int, GameObject>();
    private bool isSubscribed = false;

    void OnEnable()
    {
        SubscribeToEvents();
        RefreshPlayerList();
    }

    void SubscribeToEvents()
    {
        if (isSubscribed) return;

        if (PlayerInfoManager.Instance != null)
        {
            PlayerInfoManager.Instance.OnPlayerAdded += OnPlayerAdded;
            PlayerInfoManager.Instance.OnPlayerRemoved += OnPlayerRemoved;
            PlayerInfoManager.Instance.OnPlayerUpdated += OnPlayerUpdated;
            PlayerInfoManager.Instance.OnPlayerCountChanged += OnPlayerCountChanged;

            isSubscribed = true;
            Debug.Log("PlayerListUI подписан на события");

            // Показываем существующих игроков
            ShowExistingPlayers();
        }
        else
        {
            Debug.LogWarning("PlayerInfoManager.Instance is null, повторная попытка подписки...");
            Invoke("SubscribeToEvents", 0.5f);
        }
    }

    void ShowExistingPlayers()
    {
        // Очищаем существующие элементы
        foreach (var item in playerListItems.Values)
        {
            if (item != null) Destroy(item);
        }
        playerListItems.Clear();

        // Добавляем всех существующих игроков
        var allPlayers = PlayerInfoManager.Instance?.GetAllPlayers();
        if (allPlayers != null)
        {
            foreach (var player in allPlayers.Values)
            {
                OnPlayerAdded(player);
            }
        }
    }

    void RefreshPlayerList()
    {
        ShowExistingPlayers();
        UpdateRoomInfo();
    }

    void OnPlayerAdded(PlayerInfo playerInfo)
    {
        if (playerInfo == null) return;

        //int actorNumber = playerInfo.ActorNumber;

        if (playerListItems.ContainsKey(playerInfo.ActorNumber))
        {
            // Обновляем существующий элемент
            OnPlayerUpdated(playerInfo);
            return;
        }

        if (playerListItem != null && playerListContent != null)
        {
            GameObject listItem = Instantiate(playerListItem, playerListContent);
            PlayerListItemUI itemUI = listItem.GetComponent<PlayerListItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(playerInfo.Nickname, playerInfo.Health, playerInfo.IsAlive, playerInfo.ActorNumber);
            }
            playerListItems[playerInfo.ActorNumber] = listItem;

            Debug.Log($"Добавлен игрок в UI: {playerInfo.Nickname} (ID: {playerInfo.ActorNumber})");
        }
        else
        {
            Debug.LogError("PlayerListItem или PlayerListContent не назначены в инспекторе!");
        }

        UpdateRoomInfo();
    }

    void OnPlayerRemoved(PlayerInfo playerInfo)
    {
        if (playerInfo == null) return;

        int actorNumber = playerInfo.ActorNumber;

        if (playerListItems.ContainsKey(actorNumber))
        {
            Destroy(playerListItems[actorNumber]);
            playerListItems.Remove(actorNumber);
            Debug.Log($"Удален игрок из UI: {playerInfo.Nickname} (ID: {actorNumber})");
        }

        UpdateRoomInfo();
    }

    void OnPlayerUpdated(PlayerInfo playerInfo)
    {
        if (playerInfo == null) return;

        int actorNumber = playerInfo.ActorNumber;

        if (playerListItems.ContainsKey(actorNumber))
        {
            PlayerListItemUI itemUI = playerListItems[actorNumber].GetComponent<PlayerListItemUI>();
            if (itemUI != null)
            {
                itemUI.UpdateInfo(playerInfo.Nickname, playerInfo.Health, playerInfo.IsAlive, playerInfo.PlayerId);
            }
        }
        else
        {
            // Если элемента нет, добавляем его
            OnPlayerAdded(playerInfo);
        }
    }

    void OnPlayerCountChanged(int newCount)
    {
        UpdateRoomInfo();
    }

    void UpdateRoomInfo()
    {
        if (playerCountText != null)
        {
            int totalPlayers = PlayerInfoManager.Instance?.PlayerCount ?? 0;
            int alivePlayers = PlayerInfoManager.Instance?.AlivePlayerCount ?? 0;
            playerCountText.text = $"Игроков: {alivePlayers}/{totalPlayers}";
        }

        if (roomInfoText != null && PhotonNetwork.InRoom)
        {
            roomInfoText.text = $"Комната: {PhotonNetwork.CurrentRoom.Name}";
        }
    }

    void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    void UnsubscribeFromEvents()
    {
        if (!isSubscribed) return;

        if (PlayerInfoManager.Instance != null)
        {
            PlayerInfoManager.Instance.OnPlayerAdded -= OnPlayerAdded;
            PlayerInfoManager.Instance.OnPlayerRemoved -= OnPlayerRemoved;
            PlayerInfoManager.Instance.OnPlayerUpdated -= OnPlayerUpdated;
            PlayerInfoManager.Instance.OnPlayerCountChanged -= OnPlayerCountChanged;
        }

        isSubscribed = false;
    }
}