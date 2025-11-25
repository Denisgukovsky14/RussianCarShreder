using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;

public class PlayerListUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform playerListContent;  // Перетащи сюда Content из ScrollView
    public GameObject playerListItem;  // Перетащи сюда префаб PlayerListItemPrefab

    [Header("UI Texts")]
    public TMP_Text playerCountText;  // Опционально: для отображения общего количества
    public TMP_Text roomInfoText;     // Опционально: для информации о комнате
    //public GameObject playerListPanel;

    private Dictionary<int, GameObject> playerListItems = new Dictionary<int, GameObject>();

    void Awake()
    {
        //  УБИРАЕМ ПОВТОРНУЮ ПОДПИСКУ - ОСТАВЛЯЕМ ТОЛЬКО ЭТОТ ВЫЗОВ
        SubscribeToEvents();
        //playerListPanel = transform.Find("PlayerListUI")?.gameObject;
        //Debug.Log(playerListPanel);
    }

   

    void SubscribeToEvents()
    {
        if (PlayerInfoManager.Instance != null)
        {
            //  ПОДПИСЫВАЕМСЯ ТОЛЬКО ОДИН РАЗ
            PlayerInfoManager.Instance.OnPlayerAdded += OnPlayerAdded;
            PlayerInfoManager.Instance.OnPlayerRemoved += OnPlayerRemoved;
            PlayerInfoManager.Instance.OnPlayerUpdated += OnPlayerUpdated;
            PlayerInfoManager.Instance.OnPlayerCountChanged += OnPlayerCountChanged;

            Debug.Log(" PlayerListUI подписан на события");

            //  СРАЗУ ПОКАЗЫВАЕМ УЖЕ ЗАРЕГИСТРИРОВАННЫХ ИГРОКОВ
            ShowExistingPlayers();
        }
        else
        {
            // Если менеджер еще не создан, пробуем снова через мгновение
            Invoke("SubscribeToEvents", 0.1f);
        }
    }

    //  ДОБАВЛЯЕМ МЕТОД ДЛЯ ПОКАЗА СУЩЕСТВУЮЩИХ ИГРОКОВ
    void ShowExistingPlayers()
    {
        var allPlayers = PlayerInfoManager.Instance.GetAllPlayers();
        foreach (var player in allPlayers.Values)
        {
            OnPlayerAdded(player);
        }
        Debug.Log($" Показано существующих игроков: {allPlayers.Count}");

        foreach (var player in allPlayers.Values)
        {
            Debug.Log($" Обрабатываю игрока: {player.Nickname} (ID: {player.photonView.OwnerActorNr}, IsMine: {player.photonView.IsMine})");
            OnPlayerAdded(player);
        }

    }



    void OnPlayerAdded(PlayerInfo playerInfo)
    {
        int actorNumber = playerInfo.ActorNumber;

        //  ПРОВЕРЯЕМ ЧТО ИГРОК ЕЩЕ НЕ ДОБАВЛЕН
        if (playerListItems.ContainsKey(actorNumber))
        {
            Debug.Log($" Игрок {playerInfo.Nickname} уже есть в UI! Пропускаем...");
            return;
        }

        // Скрипт создания блока с игроком в листе игроков
        GameObject listItem = Instantiate(playerListItem, playerListContent);
        PlayerListItemUI itemUI = listItem.GetComponent<PlayerListItemUI>();
        itemUI.Setup(playerInfo.Nickname, playerInfo.Health, playerInfo.IsAlive, actorNumber);
        playerListItems[actorNumber] = listItem;
        Debug.Log($" Добавлен в UI: {playerInfo.Nickname} (ID: {actorNumber})");



        UpdateRoomInfo();
    }

    void OnPlayerRemoved(PlayerInfo playerInfo)
    {
        int actorNumber = playerInfo.ActorNumber;

        if (playerListItems.ContainsKey(actorNumber))
        {
            Destroy(playerListItems[actorNumber]);
            playerListItems.Remove(actorNumber);
            Debug.Log($" Удален из UI: {playerInfo.Nickname} (ID: {actorNumber})");
        }

        UpdateRoomInfo();
    }

    void OnPlayerUpdated(PlayerInfo playerInfo)
    {
        int actorNumber = playerInfo.ActorNumber;

        if (playerListItems.ContainsKey(actorNumber))
        {
            PlayerListItemUI itemUI = playerListItems[actorNumber].GetComponent<PlayerListItemUI>();
            itemUI.UpdateInfo(playerInfo.Nickname, playerInfo.Health,
                            playerInfo.IsAlive);
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

    //  ОТПИСЫВАЕМСЯ ПРИ УНИЧТОЖЕНИИ
    void OnDestroy()
    {
        if (PlayerInfoManager.Instance != null)
        {
            PlayerInfoManager.Instance.OnPlayerAdded -= OnPlayerAdded;
            PlayerInfoManager.Instance.OnPlayerRemoved -= OnPlayerRemoved;
            PlayerInfoManager.Instance.OnPlayerUpdated -= OnPlayerUpdated;
            PlayerInfoManager.Instance.OnPlayerCountChanged -= OnPlayerCountChanged;
        }
    }
}