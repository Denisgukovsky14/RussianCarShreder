using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public TMP_InputField playerNameInput;
    public InputField createInput;
    public InputField joinInput;
    public TMP_InputField roomPasswordInput; // Новое поле для пароля комнаты
    public Text connectionStatusText;
    public Button createButton;
    public Button joinButton;
    public Button retryButton;
    public Button searchGameButton; // Новая кнопка поиска игры
    //public GameObject loadingPanel;
    public Toggle IfOffline;

    // КЭШИРОВАНИЕ
    private static readonly System.Text.RegularExpressions.Regex regex =
    new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_\- ]+$");

    private Coroutine _keepAliveCoroutine;
    private bool _isKeepAliveRunning = false;

    [Header("Room Browser UI")]
    public GameObject roomBrowserPanel; // Панель браузера комнат
    public ScrollRect roomScrollView; // ScrollView для списка комнат
    public GameObject roomEntryPrefab; // Префаб элемента комнаты
    public GameObject passwordPromptPanel; // Панель ввода пароля
    public InputField passwordInputField; // Поле ввода пароля
    public Button passwordSubmitButton; // Кнопка подтверждения пароля
    public Button passwordCancelButton; // Кнопка отмены ввода пароля

    private bool isConnected = false;
    private bool isInLobby = false;
    private float lastKeepAliveTime = 0f;
    private const float keepAliveInterval = 10f;

    private int connectionRetries = 0;
    private const int maxRetries = 3;

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private string selectedRoomName = "";
    private bool selectedRoomHasPassword = false;

    void Start()
    {
        if (playerNameInput != null && string.IsNullOrEmpty(playerNameInput.text))
        {
            playerNameInput.text = "Player_" + Random.Range(1000, 9999);
        }

        IfOffline.isOn = false;
        Debug.Log("Initializing Photon...");

        // Важные настройки
        Application.runInBackground = true;

        // Настройки Photon
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1.0";

        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 10;
        PhotonNetwork.KeepAliveInBackground = 60;

        InitializeUI();
        ConnectToPhoton();
    }

    // ИСПОЛЬЗОВАНИЕ СОБЫТИЙ ВМЕСТО UPDATE

    // В Целях оптимизации, логика, проверяющая, есть ли подключение к серверам фотона, вынесена в Корутину, и исполняется реже

    //void Update()
    //{
    //    if (PhotonNetwork.IsConnected && Time.time - lastKeepAliveTime > keepAliveInterval)
    //    {
    //        lastKeepAliveTime = Time.time;
    //    }
    //}

    private bool CheckUIReferences()
    {
        bool allGood = true;
        if (createInput == null) { Debug.LogError("createInput is not assigned!"); allGood = false; }
        if (joinInput == null) { Debug.LogError("joinInput is not assigned!"); allGood = false; }
        if (connectionStatusText == null) { Debug.LogError("connectionStatusText is not assigned!"); allGood = false; }
        if (createButton == null) { Debug.LogError("createButton is not assigned!"); allGood = false; }
        if (joinButton == null) { Debug.LogError("joinButton is not assigned!"); allGood = false; }
        if (retryButton == null) { Debug.LogError("retryButton is not assigned!"); allGood = false; }
        //if (loadingPanel == null) { Debug.LogError("loadingPanel is not assigned!"); allGood = false; }
        if (searchGameButton == null) { Debug.LogError("searchGameButton is not assigned!"); allGood = false; }
        if (roomBrowserPanel == null) { Debug.LogError("roomBrowserPanel is not assigned!"); allGood = false; }
        if (roomScrollView == null) { Debug.LogError("roomScrollView is not assigned!"); allGood = false; }
        if (roomEntryPrefab == null) { Debug.LogError("roomEntryPrefab is not assigned!"); allGood = false; }
        if (passwordPromptPanel == null) { Debug.LogError("passwordPromptPanel is not assigned!"); allGood = false; }
        if (passwordInputField == null) { Debug.LogError("passwordInputField is not assigned!"); allGood = false; }
        if (passwordSubmitButton == null) { Debug.LogError("passwordSubmitButton is not assigned!"); allGood = false; }
        if (passwordCancelButton == null) { Debug.LogError("passwordCancelButton is not assigned!"); allGood = false; }
        return allGood;
    }

    private void InitializeUI()
    {
        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetButtonInteractable(searchGameButton, false);
        SafeSetGameObjectActive(retryButton?.gameObject, false);
        //SafeSetGameObjectActive(loadingPanel, true);
        SafeSetGameObjectActive(roomBrowserPanel, false);
        SafeSetGameObjectActive(passwordPromptPanel, false);

        if (retryButton != null)
            retryButton.onClick.AddListener(ManualReconnect);

        if (searchGameButton != null)
            searchGameButton.onClick.AddListener(ShowRoomBrowser);

        if (passwordSubmitButton != null)
            passwordSubmitButton.onClick.AddListener(JoinRoomWithPassword);

        if (passwordCancelButton != null)
            passwordCancelButton.onClick.AddListener(CancelPasswordJoin);

        UpdateConnectionStatus("Initializing...");
    }

    // ВАЛИДАТОР НИКНЕЙМА
    public string ValidateAndSanitizeNickname(string nickname)
    {
        if (string.IsNullOrEmpty(nickname))
        {
            return "Player_" + Random.Range(1000, 9999);
        }

        // Ограничение длины (3-20 символов)
        if (nickname.Length > 20)
        {
            nickname = nickname.Substring(0, 20);
            Debug.LogWarning("Nickname too long, truncated to 20 characters");
        }

        if (nickname.Length < 3)
        {
            nickname = "Player_" + Random.Range(1000, 9999);
            Debug.LogWarning("Nickname too short, using generated name");
        }

        // Проверка на разрешенные символы (только буквы, цифры, подчеркивания и дефисы)
        //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_\- ]+$");
        if (!regex.IsMatch(nickname))
        {
            Debug.LogWarning("Nickname contains invalid characters, sanitizing...");
            // Удаляем запрещенные символы
            System.Text.RegularExpressions.Regex sanitizeRegex = new System.Text.RegularExpressions.Regex(@"[^a-zA-Z0-9_\- ]");
            nickname = sanitizeRegex.Replace(nickname, "");

            // Если после очистки строка пустая, генерируем ник
            if (string.IsNullOrEmpty(nickname.Trim()))
            {
                nickname = "Player_" + Random.Range(1000, 9999);
            }
        }

        // Убираем пробелы в начале и конце
        nickname = nickname.Trim();

        // Заменяем множественные пробелы на один
        while (nickname.Contains("  "))
        {
            nickname = nickname.Replace("  ", " ");
        }

        return nickname;
    }

    // Метод для проверки никнейма в реальном времени (можно привязать к событию OnValueChanged)
    public void OnNicknameValueChanged()
    {
        if (playerNameInput != null)
        {
            string validatedNickname = ValidateAndSanitizeNickname(playerNameInput.text);
            if (validatedNickname != playerNameInput.text)
            {
                playerNameInput.text = validatedNickname;
            }
        }
    }

    private void SafeSetButtonInteractable(Button button, bool interactable)
    {
        if (button != null) button.interactable = interactable;
    }

    private void SafeSetGameObjectActive(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }

    public void ConnectToPhoton()
    {
        if (PhotonNetwork.IsConnected)
        {
            OnConnectedToMaster();
            return;
        }

        UpdateConnectionStatus("Connecting to Photon...");
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = null;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Successfully connected to Master Server in region: " + PhotonNetwork.CloudRegion);
        connectionRetries = 0;
        isConnected = true;
        UpdateConnectionStatus("Connected! Joining lobby...");

        StartKeepAliveCoroutine();

        PhotonNetwork.JoinLobby();
    }

    private void StartKeepAliveCoroutine()
    {
        // Останавливаем предыдущую корутину, если она работает
        StopKeepAliveCoroutine();

        // Запускаем новую
        _keepAliveCoroutine = StartCoroutine(KeepAliveCoroutine());
        _isKeepAliveRunning = true;
        Debug.Log("KeepAlive корутина запущена");
    }

    private IEnumerator KeepAliveCoroutine()
    {
        const float keepAliveInterval = 10f; // 10 секунд

        while (_isKeepAliveRunning && PhotonNetwork.IsConnected)
        {
            yield return new WaitForSeconds(keepAliveInterval);

            if (PhotonNetwork.IsConnected)
            {
                // Простая проверка соединения
                // Можно добавить ping или другую логику
                lastKeepAliveTime = Time.time;

                // Опционально: отправка пустого RPC для поддержания соединения
                // photonView.RPC("KeepAlivePing", RpcTarget.All);
            }
            else
            {
                // Если отключились, выходим из цикла
                break;
            }
        }

        Debug.Log("KeepAlive корутина завершена");
        _isKeepAliveRunning = false;
    }

    private void StopKeepAliveCoroutine()
    {
        if (_keepAliveCoroutine != null)
        {
            StopCoroutine(_keepAliveCoroutine);
            _keepAliveCoroutine = null;
        }
        _isKeepAliveRunning = false;
        Debug.Log("KeepAlive корутина остановлена");
    }


    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        isInLobby = true;
        UpdateConnectionStatus("Ready to create or join rooms!");

        SafeSetButtonInteractable(createButton, true);
        SafeSetButtonInteractable(joinButton, true);
        SafeSetButtonInteractable(searchGameButton, true);
        //SafeSetGameObjectActive(loadingPanel, false);
        SafeSetGameObjectActive(retryButton?.gameObject, false);
    }

    // ОБНОВЛЕННЫЙ МЕТОД ДЛЯ СОЗДАНИЯ КОМНАТЫ С ПАРОЛЕМ
    public void CreateRoom()
    {
        // Валидация никнейма
        if (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text))
        {
            string validatedNickname = ValidateAndSanitizeNickname(playerNameInput.text);
            PhotonNetwork.NickName = validatedNickname;
            playerNameInput.text = validatedNickname; // Обновляем поле ввода
            Debug.Log($"Никнейм установлен: {PhotonNetwork.NickName}");
        }
        else
        {
            PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
            Debug.Log($"Случайный ник: {PhotonNetwork.NickName}");
        }

        if (IfOffline.isOn)
        {
            OnDisconnected(DisconnectCause.None);
            CreateOfflineRoom();
            return;
        }

        if (!isInLobby)
        {
            UpdateConnectionStatus("Not in lobby! Please wait...");
            return;
        }

        string roomName = createInput != null ? createInput.text.Trim() : "";
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room_" + Random.Range(1000, 9999);
            if (createInput != null) createInput.text = roomName;
        }

        string password = roomPasswordInput != null ? roomPasswordInput.text.Trim() : "";
        bool hasPassword = !string.IsNullOrEmpty(password);

        UpdateConnectionStatus("Creating room...");
        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetButtonInteractable(searchGameButton, false);
        //SafeSetGameObjectActive(loadingPanel, true);

        // Настройки комнаты с возможностью пароля
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        };

        // Добавляем информацию о пароле в свойства комнаты
        if (hasPassword)
        {
            roomOptions.CustomRoomProperties.Add("password", password);
            roomOptions.CustomRoomProperties.Add("hasPassword", true);
        }
        else
        {
            roomOptions.CustomRoomProperties.Add("hasPassword", false);
        }

        // Устанавливаем свойства, которые будут видны в лобби
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "hasPassword" };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    // БРАУЗЕР КОМНАТ
    public void ShowRoomBrowser()
    {
        if (!isInLobby)
        {
            UpdateConnectionStatus("Not in lobby! Please wait...");
            return;
        }

        SafeSetGameObjectActive(roomBrowserPanel, true);
        UpdateRoomList();
    }

    public void HideRoomBrowser()
    {
        SafeSetGameObjectActive(roomBrowserPanel, false);
    }

    private void UpdateRoomList()
    {
        // Очищаем существующий список
        foreach (Transform child in roomScrollView.content.transform)
        {
            Destroy(child.gameObject);
        }

        // Создаем элементы для каждой комнаты
        foreach (var roomInfo in cachedRoomList.Values)
        {
            GameObject roomEntry = Instantiate(roomEntryPrefab, roomScrollView.content.transform);
            RoomEntryUI entryUI = roomEntry.GetComponent<RoomEntryUI>();

            if (entryUI != null)
            {
                bool hasPassword = roomInfo.CustomProperties.ContainsKey("hasPassword") &&
                                  (bool)roomInfo.CustomProperties["hasPassword"];

                entryUI.Setup(roomInfo.Name, roomInfo.PlayerCount, roomInfo.MaxPlayers, hasPassword);

                // Добавляем обработчик двойного клика
                Button roomButton = roomEntry.GetComponent<Button>();
                if (roomButton != null)
                {
                    roomButton.onClick.AddListener(() => OnRoomDoubleClick(roomInfo.Name, hasPassword));
                }
            }
        }
    }

    private void OnRoomDoubleClick(string roomName, bool hasPassword)
    {
        selectedRoomName = roomName;
        selectedRoomHasPassword = hasPassword;

        if (hasPassword)
        {
            // Показываем диалог ввода пароля
            ShowPasswordPrompt();
        }
        else
        {
            // Присоединяемся напрямую
            JoinSelectedRoom("");
        }
    }

    private void ShowPasswordPrompt()
    {
        SafeSetGameObjectActive(passwordPromptPanel, true);
        if (passwordInputField != null)
        {
            passwordInputField.text = "";
        }
    }

    private void HidePasswordPrompt()
    {
        SafeSetGameObjectActive(passwordPromptPanel, false);
    }

    private void JoinRoomWithPassword()
    {
        string password = passwordInputField != null ? passwordInputField.text.Trim() : "";
        JoinSelectedRoom(password);
        HidePasswordPrompt();
    }

    private void CancelPasswordJoin()
    {
        HidePasswordPrompt();
        selectedRoomName = "";
    }

    private void JoinSelectedRoom(string password)
    {
        // Валидация никнейма
        if (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text))
        {
            string validatedNickname = ValidateAndSanitizeNickname(playerNameInput.text);
            PhotonNetwork.NickName = validatedNickname;
            playerNameInput.text = validatedNickname;
            Debug.Log($"Никнейм установлен: {PhotonNetwork.NickName}");
        }
        else
        {
            PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
            Debug.Log($"Случайный никнейм: {PhotonNetwork.NickName}");
        }

        if (!isInLobby)
        {
            UpdateConnectionStatus("Not in lobby! Please wait...");
            return;
        }

        UpdateConnectionStatus("Joining room...");
        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetButtonInteractable(searchGameButton, false);
        //SafeSetGameObjectActive(loadingPanel, true);

        // Здесь можно добавить дополнительную проверку пароля при присоединении
        PhotonNetwork.JoinRoom(selectedRoomName);
    }

    // ОБНОВЛЕННЫЙ МЕТОД ДЛЯ ПРИСОЕДИНЕНИЯ ПО ИМЕНИ
    public void JoinRoom()
    {
        // Валидация никнейма
        if (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text))
        {
            string validatedNickname = ValidateAndSanitizeNickname(playerNameInput.text);
            PhotonNetwork.NickName = validatedNickname;
            playerNameInput.text = validatedNickname;
            Debug.Log($"Никнейм установлен: {PhotonNetwork.NickName}");
        }
        else
        {
            PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
            Debug.Log($"Случайный никнейм: {PhotonNetwork.NickName}");
        }

        if (!isInLobby)
        {
            UpdateConnectionStatus("Not in lobby! Please wait...");
            return;
        }

        string roomName = joinInput != null ? joinInput.text.Trim() : "";
        if (string.IsNullOrEmpty(roomName))
        {
            UpdateConnectionStatus("Please enter room name!");
            return;
        }

        UpdateConnectionStatus("Joining room...");
        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetButtonInteractable(searchGameButton, false);
        //SafeSetGameObjectActive(loadingPanel, true);

        PhotonNetwork.JoinRoom(roomName);
    }

    // ОБНОВЛЕНИЕ СПИСКА КОМНАТ
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log($"Room list updated: {roomList.Count} rooms");

        // Обновляем кэшированный список комнат
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList)
            {
                cachedRoomList.Remove(roomInfo.Name);
            }
            else
            {
                cachedRoomList[roomInfo.Name] = roomInfo;
            }
        }

        // Если браузер комнат открыт, обновляем отображение
        if (roomBrowserPanel != null && roomBrowserPanel.activeInHierarchy)
        {
            UpdateRoomList();
        }
    }

    // ОСТАЛЬНЫЕ МЕТОДЫ ОСТАЮТСЯ БЕЗ ИЗМЕНЕНИЙ
    private void CreateOfflineRoom()
    {
        string roomName = createInput != null ? createInput.text.Trim() : "";
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "OfflineRoom_" + Random.Range(1000, 9999);
            if (createInput != null) createInput.text = roomName;
        }

        UpdateConnectionStatus("Creating offline room...");
        PhotonNetwork.CreateRoom(roomName);

        //AnalyticsManager.Instance.TrackLevelEvent(2, "start");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected: {cause}");

        if (connectionRetries < maxRetries)
        {
            connectionRetries++;
            UpdateConnectionStatus($"Reconnecting... Attempt {connectionRetries}/{maxRetries}");
            Invoke("ConnectToPhoton", 2f);
        }
        else
        {
            UpdateConnectionStatus($"Failed to connect after {maxRetries} attempts");
        }

        isConnected = false;
        isInLobby = false;

        SafeSetButtonInteractable(createButton, false);
        SafeSetButtonInteractable(joinButton, false);
        SafeSetButtonInteractable(searchGameButton, false);
        //SafeSetGameObjectActive(loadingPanel, false);

        UpdateConnectionStatus($"Disconnected: {cause}");
        SafeSetGameObjectActive(retryButton?.gameObject, true);

    }

    public void ManualReconnect()
    {
        UpdateConnectionStatus("Reconnecting...");
        SafeSetGameObjectActive(retryButton?.gameObject, false);
        //SafeSetGameObjectActive(loadingPanel, true);
        ConnectToPhoton();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully!");
        UpdateConnectionStatus("Room created! Waiting for players...");
        StartCoroutine(WaitForPlayersOrStart());
    }

    private IEnumerator WaitForPlayersOrStart()
    {
        float waitTime = 30f;
        float elapsed = 0f;

        while (elapsed < waitTime && PhotonNetwork.InRoom)
        {
            UpdateConnectionStatus($"Waiting for players... ({Mathf.RoundToInt(waitTime - elapsed)}s)");

            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && PhotonNetwork.IsMasterClient)
            {
                StartGame();
                yield break;
            }

            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        UpdateConnectionStatus("Starting game...");
        PhotonNetwork.LoadLevel("GAME");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room creation failed: {message}");
        UpdateConnectionStatus($"Create failed: {message}");

        SafeSetButtonInteractable(createButton, true);
        SafeSetButtonInteractable(joinButton, true);
        SafeSetButtonInteractable(searchGameButton, true);
        //SafeSetGameObjectActive(loadingPanel, false);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join room failed: {message}");
        UpdateConnectionStatus($"Join failed: {message}");

        SafeSetButtonInteractable(createButton, true);
        SafeSetButtonInteractable(joinButton, true);
        SafeSetButtonInteractable(searchGameButton, true);
        //SafeSetGameObjectActive(loadingPanel, false);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully!");
        UpdateConnectionStatus($"Joined room: {PhotonNetwork.CurrentRoom.Name}");

        if (PhotonNetwork.InRoom)
        {
            Debug.Log($"Players in room: {PhotonNetwork.CurrentRoom.PlayerCount}");
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                Debug.Log($" - {player.NickName} (Actor: {player.ActorNumber})");
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartGameWithDelay());
        }
    }

    private IEnumerator StartGameWithDelay()
    {
        yield return new WaitForSeconds(3f);
        PhotonNetwork.LoadLevel("GAME");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} joined the room");
        UpdateConnectionStatus($"Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");

        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    private void UpdateConnectionStatus(string status)
    {
        if (connectionStatusText != null)
            connectionStatusText.text = status;

        Debug.Log($"[MenuManager] {status}");
    }

    void OnDestroy()
    {
        // Останавливаем все корутины
        StopKeepAliveCoroutine();

        // Останавливаем все Invoke
        CancelInvoke();

        // Отписываемся от событий Photon
        PhotonNetwork.RemoveCallbackTarget(this);

        Debug.Log("MenuManager уничтожен, все корутины остановлены");
    }


}
