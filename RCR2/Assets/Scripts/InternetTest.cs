using System.Collections;
using UnityEngine;

public class InternetTest : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(TestInternet());
    }

    IEnumerator TestInternet()
    {
        // Пинг до Российских Photon серверов
        string[] russianServers = {
        "5.101.64.30",     // Moscow Photon server 1
        "5.101.64.31",     // Moscow Photon server 2  
        "5.101.64.32",     // Moscow Photon server 3
        "185.3.213.175",   // Alternative RU server
    };

        foreach (string server in russianServers)
        {
            Ping ping = new Ping(server);
            float timer = 0;

            while (!ping.isDone && timer < 3f)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (ping.isDone)
                Debug.Log($"круто {server}: {ping.time}ms");
            else
                Debug.LogError($"хуево {server}: No response");
        }
    }
}
