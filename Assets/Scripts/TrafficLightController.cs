using UnityEngine;
using System.Collections;

public class TrafficLightController : MonoBehaviour
{
    [Header("Light Material")]
    public Material trafficLightMaterial;

    [Header("Timing Settings")]
    public float redDuration = 5f;
    public float yellowDuration = 2f;
    public float greenDuration = 5f;
    public float blinkSpeed = 3f;

    private enum LightState { Red, Yellow, Green }
    private LightState currentState = LightState.Red;

    void Start()
    {
        // Начинаем цикл светофора
        StartCoroutine(TrafficLightCycle());
    }

    IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            // Красный свет
            SetLightState(LightState.Red);
            yield return new WaitForSeconds(redDuration);

            // Желтый свет (мигает)
            SetLightState(LightState.Yellow);
            SetBlinking(true);
            yield return new WaitForSeconds(yellowDuration);
            SetBlinking(false);

            // Зеленый свет
            SetLightState(LightState.Green);
            yield return new WaitForSeconds(greenDuration);

            // Желтый свет (мигает)
            SetLightState(LightState.Yellow);
            SetBlinking(true);
            yield return new WaitForSeconds(yellowDuration);
            SetBlinking(false);
        }
    }

    void SetLightState(LightState state)
    {
        currentState = state;

        // Обновляем только один материал
        float stateValue = (float)state;

        if (trafficLightMaterial != null)
            trafficLightMaterial.SetFloat("_CurrentState", stateValue);
    }

    void SetBlinking(bool blinking)
    {
        float blinkValue = blinking ? 1f : 0f;

        if (trafficLightMaterial != null)
        {
            trafficLightMaterial.SetFloat("_IsBlinking", blinkValue);
            trafficLightMaterial.SetFloat("_BlinkSpeed", blinkSpeed);
        }
    }

    void OnDestroy()
    {
        // Сброс параметров при уничтожении
        if (trafficLightMaterial != null)
        {
            trafficLightMaterial.SetFloat("_IsBlinking", 0);
            trafficLightMaterial.SetFloat("_CurrentState", 0);
        }
    }
}