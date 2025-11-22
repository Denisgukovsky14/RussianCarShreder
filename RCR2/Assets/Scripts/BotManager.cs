using UnityEngine;

public class BotManager : MonoBehaviour
{
    [Header("Bot Settings")]
    public GameObject botPrefab;
    public int botsPerPlayer = 1;
    public Vector3 protectedDestination; // Точка, которую защищают боты

    [Header("Spawn Settings")]
    public float spawnRadius = 10f;
    public Transform[] botSpawnPoints;

    public void InitializeBots()
    {
        Debug.Log("Starting bot spawn...");

        int botCount = GetBotCount();

        for (int i = 0; i < botCount; i++)
        {
            SpawnSingleBot(i);
        }

        Debug.Log($"Spawned {botCount} bots");
    }

    void SpawnSingleBot(int botIndex)
    {
        Vector3 spawnPos = GetBotSpawnPosition(botIndex);
        GameObject bot = Instantiate(botPrefab, spawnPos, Quaternion.identity);

        // Только устанавливаем точку защиты, игрока бот найдет сам
        SetupBotAI(bot, botIndex);
        Debug.Log($"Bot {botIndex} spawned at {spawnPos}");
    }

    void SetupBotAI(GameObject bot, int botIndex)
    {
        BotCarAI botAI = bot.GetComponent<BotCarAI>();
        if (botAI != null)
        {
            // Устанавливаем только точку защиты
            botAI.targetDestination = protectedDestination;

            // Разные характеристики для разнообразия
            botAI.attackDistance = 10f + (botIndex * 2f);
            botAI.rammingSpeed = 0.7f + (botIndex * 0.1f);
            botAI.maxSteerAngle = 25f + (botIndex * 5f);
            botAI.reverseDistance = 3f + (botIndex * 0.5f);

            Debug.Log($"Bot {botIndex} configured");
        }
        else
        {
            Debug.LogError($"BotCarAI component not found on bot {botIndex}!");
        }
    }

    Vector3 GetBotSpawnPosition(int index)
    {
        if (botSpawnPoints != null && botSpawnPoints.Length > 0)
        {
            Transform spawnPoint = botSpawnPoints[index % botSpawnPoints.Length];
            return spawnPoint.position;
        }

        // Спавн по кругу
        float angle = index * (360f / Mathf.Max(1, botsPerPlayer));
        Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        return transform.position + dir * spawnRadius;
    }

    int GetBotCount()
    {
        return botsPerPlayer;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        if (botSpawnPoints != null)
        {
            Gizmos.color = Color.blue;
            foreach (Transform spawnPoint in botSpawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireCube(spawnPoint.position, Vector3.one * 2f);
                }
            }
        }

        if (protectedDestination != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(protectedDestination, Vector3.one * 3f);
        }
    }
}