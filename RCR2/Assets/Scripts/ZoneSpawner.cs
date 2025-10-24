using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; // Ссылка на префаб, который нужно заспавнить
    //Vector3 spawnPosition = new Vector3(-47667f, 7222f, -50123f);
    public bool Stop = true;
    GameObject spawnedObject;
    public GameObject[] spawningObjects;
    public Vector3 rememberscale;

    private float timecount = 0f;
    private const float BreakTime = 10f;
    public int ZoneSpawnTimes = 0;

    public float Timecount { get { return timecount; } }


    void Start()
    {
        rememberscale = prefabToSpawn.transform.localScale;
        //Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
        if (prefabToSpawn == null)
        {
            Debug.LogError("Префаб для спавна не привязан!");
            return;
        }
        else
        {
            //Debug.Log("Время пошло");
        }
    }

    private void FixedUpdate()
    {
        if ( Stop )
        {
            SpawnObject();
            //Debug.Log( );
            Stop = false;
        }

        if (spawnedObject != null)
        {
            // Получаем скрипт Finish
            FinishTrigger finishScript = spawnedObject.GetComponent<FinishTrigger>();

            // Проверяем, что скрипт найден
            if (finishScript != null)
            {

                // Выводим время существования объекта
                //Debug.Log("Время существования объекта: " + Mathf.Round(finishScript.ExistenceTime) + " секунд");
            }
        }
        else
        {
            //Debug.Log("Объект уничтожен");
            timecount += Time.deltaTime;
            //Debug.Log("Время перерыва: " + Mathf.Round(timecount) + " секунд");

            // Проверяем, не превышено ли максимальное время существования
            if (timecount >= BreakTime)
            {
                // Уничтожаем объект после истечения времени
                //Destroy(gameObject);
                timecount = 0f;
                //Stop = true;

                ZoneSpawnTimes += 1;
                if (ZoneSpawnTimes == 3)
                {
                    //Debug.Log("Игра окончена");
                }
                else
                {
                    Stop = true;
                }
            }
        }
    }

    void SpawnObject()
    {
        // Создаем экземпляр префаба

        int Bones = Random.Range(0, 2);
        //Debug.Log(Bones);

        Vector3 CurrentZone = spawningObjects[Bones].transform.position;

        spawnedObject = Instantiate(prefabToSpawn, CurrentZone, Quaternion.Euler(-90f, 0f, 0f));



        // Уменьшаем масштаб в 2 раза
        rememberscale *= 0.8f;
        spawnedObject.transform.localScale = rememberscale; //= new Vector3( spawnedObject.transform.localScale * 0.8f, 0.8f, 0.8f);
    }
}
