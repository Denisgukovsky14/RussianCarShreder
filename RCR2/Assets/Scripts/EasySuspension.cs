using UnityEngine;
using System.Collections;
using Photon.Pun;

[ExecuteInEditMode()]
public class EasySuspension : MonoBehaviour
{
    public CarConfig config;



    [Range(0, 20)]
    public float naturalFrequency = 10;

    [Range(0, 3)]
    public float dampingRatio = 0.8f;

    [Range(-1, 1)]
    public float forceShift = 0.03f;

    private PhotonView view;
    public bool setSuspensionDistance = true;

    void Start()
    {
        view = GetComponent<PhotonView>();
        LoadConfigValues();
    }

    void Update()
    {
        // Загружаем значения из конфига каждый кадр (на случай изменения в реальном времени)
        LoadConfigValues();

        if (view != null && view.IsMine)
        {
            ApplySuspensionSettings();
        }
    }

    /// <summary>
    /// Загружает значения из конфига если он назначен
    /// </summary>
    private void LoadConfigValues()
    {
        if (config != null)
        {
            naturalFrequency = config.naturalFrequency;
            dampingRatio = config.dampingRatio;
            forceShift = config.forceShift;
            setSuspensionDistance = config.setSuspensionDistance;
        }
    }

    /// <summary>
    /// Применяет настройки подвески к колесам
    /// </summary>
    private void ApplySuspensionSettings()
    {
        foreach (WheelCollider wc in GetComponentsInChildren<WheelCollider>())
        {
            JointSpring spring = wc.suspensionSpring;

            spring.spring = Mathf.Pow(Mathf.Sqrt(wc.sprungMass) * naturalFrequency, 2);
            spring.damper = 2 * dampingRatio * Mathf.Sqrt(spring.spring * wc.sprungMass);

            wc.suspensionSpring = spring;

            Vector3 wheelRelativeBody = transform.InverseTransformPoint(wc.transform.position);
            float distance = GetComponent<Rigidbody>().centerOfMass.y - wheelRelativeBody.y + wc.radius;

            wc.forceAppPointDistance = distance - forceShift;

            // the following line makes sure the spring force at maximum droop is exactly zero
            if (spring.targetPosition > 0 && setSuspensionDistance)
                wc.suspensionDistance = wc.sprungMass * Physics.gravity.magnitude / (spring.targetPosition * spring.spring);
        }
    }

    // Добавляем метод для принудительного применения конфига
    [ContextMenu("Apply Config Settings")]
    public void ApplyConfigSettings()
    {
        LoadConfigValues();
        ApplySuspensionSettings();
        Debug.Log("Config settings applied!");
    }

    // Метод для сброса к значениям по умолчанию
    [ContextMenu("Reset to Default Values")]
    public void ResetToDefaultValues()
    {
        naturalFrequency = 10f;
        dampingRatio = 0.8f;
        forceShift = 0.03f;
        setSuspensionDistance = true;
        Debug.Log("Reset to default values!");
    }
}