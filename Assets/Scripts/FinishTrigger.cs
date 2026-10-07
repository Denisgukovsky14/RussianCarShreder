using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private bool playerInside = false;
    private float existenceTime = 0f;
    private const float maxExistenceTime = 50f; // Время существования объекта в секундах

    public float ExistenceTime {  get { return existenceTime; } }

    private void Start()
    {
        // Инициализация времени существования
        existenceTime = 0f;
    }

    private void Update()
    {
        //Debug.Log( Mathf.Round(existenceTime) );
        // Увеличиваем время существования объекта
        existenceTime += Time.deltaTime;
        //Debug.Log(existenceTime);

        // Проверяем, не превышено ли максимальное время существования
        if (existenceTime >= maxExistenceTime)
        {
            // Уничтожаем объект после истечения времени
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.transform.parent.tag.StartsWith("P"));
        // Проверяем, есть ли у объекта, вошедшего в триггер, тег "Player"
        if (other.transform.parent.tag.StartsWith("P"))
        {
            playerInside = true;
            Debug.Log("Player has entered the finish area!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.transform.parent.tag.StartsWith("P"));
        // Проверяем, есть ли у объекта, находящегося в триггере, тег "Player"
        //Debug.Log(playerInside);
        if (other.transform.parent.tag.StartsWith("P"))
        {
            // Здесь можно добавить дополнительную логику, если нужно
            // Например, можно постоянно обновлять какие-то данные или выводить сообщения
            if (playerInside)
            {
                playerInside = true;
                Debug.Log("Player is still inside the finish area!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log(other.transform.parent.tag.StartsWith("P"));
        // Проверяем, есть ли у объекта, вышедшего из триггера, тег "Player"
        if (other.transform.parent.tag.StartsWith("P"))
        {
            playerInside = false;
            Debug.Log("Player has left the finish area!");
        }
    }
}
