using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayers2 : MonoBehaviour
{
    public GameObject playerPrefab;
    public float spawnRange = 3f;

    void Start()
    {
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
        //GameObject.FindGameObjectWithTag("Platform").GetComponent<DefaultStatics>().Count();
        //playerPrefab.tag = "P" + GameObject.FindGameObjectWithTag("Platform").GetComponent<DefaultStatics>().Counter;
        Vector3 spawnPosition = new Vector3(
            transform.position.x + Random.Range(-spawnRange, spawnRange),
            transform.position.y,
            transform.position.z + Random.Range(-spawnRange, spawnRange)
        );

        GameObject myPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        Debug.Log("PLAYER SPAWNED: " + PhotonNetwork.NickName);
        Debug.Log("Player ViewID: " + myPlayer.GetComponent<PhotonView>().ViewID);
        Debug.Log("IsMine: " + myPlayer.GetComponent<PhotonView>().IsMine);
    }
}