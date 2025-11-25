using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayers2 : MonoBehaviour
{
    public GameObject playerPrefab;
    public float spawnRange = 3f;
    private BotManager botManager;
    int playerCount;

    void Start()
    {

        if (PlayerInfoManager.Instance == null)
        {
            CreatePlayerInfoManager();
        }

        // Добавляем BotManager динамически на этот же GameObject
        botManager = gameObject.AddComponent<BotManager>();
        botManager.botPrefab = Resources.Load<GameObject>("botCarRoot");

        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnMyPlayer();
        }
        else
        {
            Invoke("SpawnMyPlayer", 1f);
        }
    }

    void SpawnMyPlayer()
    {
        playerCount = PlayerInfoManager.Instance.PlayerCount;

        Vector3 spawnPosition = new Vector3(
            transform.position.x + Random.Range(-spawnRange, spawnRange),
            transform.position.y,
            transform.position.z + Random.Range(-spawnRange, spawnRange)
        );

            GameObject myPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        //Debug.Log("PLAYER SPAWNED: " + PhotonNetwork.NickName);

        // задержка на спавн бота после подключения игрока

        if (PhotonNetwork.IsMasterClient)
        {
            Invoke("DelayedBotSpawn", 2f);
        }
        
    }

    void CreatePlayerInfoManager()
    {
        if (PlayerInfoManager.Instance == null)
        {
            GameObject managerObj = new GameObject("PlayerInfoManager");
            managerObj.AddComponent<PhotonView>();
            managerObj.AddComponent<PlayerInfoManager>();
            Debug.Log("PlayerInfoManager создан!");
        }
    }

    void DelayedBotSpawn()
    {
        botManager.InitializeBots();
        //Debug.Log("Bots spawned after delay");
    }
}