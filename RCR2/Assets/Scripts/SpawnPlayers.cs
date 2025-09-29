using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using RootMotion;

public class SpawnPlayers : MonoBehaviour
{
    //public int Counter = 0;
    public GameObject player;
    public float minX, minY, maxX, maxY;

    void Start()
    {
        //GameObject scene = GameObject.FindGameObjectWithTag("Platform");
        GameObject.FindGameObjectWithTag("Platform").GetComponent<DefaultStatics>().Count()  ;
        player.tag = "P" + GameObject.FindGameObjectWithTag("Platform").GetComponent<DefaultStatics>().Counter;

        Vector2 randomPosition = new Vector2(Random.Range(minX, minY), Random.Range(maxX, maxY));
        PhotonNetwork.Instantiate(player.name, randomPosition, Quaternion.identity);
        

        //player.gameObject.GetComponent<CameraController>().target = player.transform.transform ;
    }

    
}
