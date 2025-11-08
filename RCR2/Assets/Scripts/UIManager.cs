using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Screen Panels")]
    public GameObject connectionPanel;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public GameObject loadingPanel;
    public GameObject errorPanel;

    [Header("Room UI Elements")]
    public Text roomNameText;
    public Text playerCountText;
    public GameObject playerListContent;
    public GameObject playerListItemPrefab;
    public Button startGameButton;

    public static UIManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowConnectionPanel()
    {
        HideAllPanels();
        connectionPanel.SetActive(true);
    }

    public void ShowLobbyPanel()
    {
        HideAllPanels();
        lobbyPanel.SetActive(true);
    }

    public void ShowRoomPanel()
    {
        HideAllPanels();
        roomPanel.SetActive(true);
    }

    public void ShowLoadingPanel()
    {
        HideAllPanels();
        loadingPanel.SetActive(true);
    }

    public void ShowErrorPanel(string errorMessage)
    {
        HideAllPanels();
        errorPanel.SetActive(true);
        errorPanel.GetComponentInChildren<Text>().text = errorMessage;
    }

    private void HideAllPanels()
    {
        connectionPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
        loadingPanel.SetActive(false);
        errorPanel.SetActive(false);
    }

    public void UpdateRoomInfo(string roomName, int currentPlayers, int maxPlayers)
    {
        if (roomNameText != null)
            roomNameText.text = $"Room: {roomName}";

        if (playerCountText != null)
            playerCountText.text = $"Players: {currentPlayers}/{maxPlayers}";

        // Обновляем кнопку старта (только для мастера-клиента)
        if (startGameButton != null)
            startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void UpdatePlayerList(Player[] players)
    {
        // Очищаем список
        foreach (Transform child in playerListContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Заполняем заново
        foreach (Player player in players)
        {
            GameObject item = Instantiate(playerListItemPrefab, playerListContent.transform);
            Text playerText = item.GetComponentInChildren<Text>();
            playerText.text = player.IsMasterClient ? $"{player.NickName} (Host)" : player.NickName;
        }
    }
}