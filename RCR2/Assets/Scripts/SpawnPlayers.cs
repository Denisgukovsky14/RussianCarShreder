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



        Vector3 randomPosition = new Vector3( transform.position.x + Random.Range(minX, minY), transform.position.y + Random.Range(maxX, maxY),  transform.position.z);
        PhotonNetwork.Instantiate(player.name, randomPosition, Quaternion.identity);
        

        //player.gameObject.GetComponent<CameraController>().target = player.transform.transform ;
    }

    
}
