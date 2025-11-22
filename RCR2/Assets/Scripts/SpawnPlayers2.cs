using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayers2 : MonoBehaviour
{
    public GameObject playerPrefab;
    public float spawnRange = 3f;
    private BotManager botManager;

    void Start()
    {
        // Добавляем BotManager динамически на этот же GameObject
        botManager = gameObject.AddComponent<BotManager>();
        botManager.botPrefab = Resources.Load<GameObject>("botCarRoot");
        botManager.botsPerPlayer = 1;

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
        Vector3 spawnPosition = new Vector3(
            transform.position.x + Random.Range(-spawnRange, spawnRange),
            transform.position.y,
            transform.position.z + Random.Range(-spawnRange, spawnRange)
        );

        GameObject myPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        Debug.Log("PLAYER SPAWNED: " + PhotonNetwork.NickName);

        // ДОБАВЬ ЗАДЕРЖКУ - ждем пока игрок полностью загрузится
        Invoke("DelayedBotSpawn", 2f);
    }

    void DelayedBotSpawn()
    {
        botManager.InitializeBots();
        Debug.Log("Bots spawned after delay");
    }
}