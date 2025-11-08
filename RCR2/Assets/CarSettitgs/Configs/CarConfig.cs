using UnityEngine;

[CreateAssetMenu(fileName = "New Car Config", menuName = "Game/Car Config")]
public class CarConfig : ScriptableObject
{
    [Header("BASIC SETTINGS")]
    public string vehicleName = "Car";

    [Header("SUSPENSION SETTINGS")]
    [Tooltip("Natural frequency of the suspension")]
    public float naturalFrequency = 9.0f;
    [Tooltip("Damping ratio of the suspension")]
    public float dampingRatio = 0.8f;
    [Tooltip("Force shift of the suspension")]
    public float forceShift = 0.32f;
    public bool setSuspensionDistance = true;

    [Header("STEERING & CONTROL")]
    //[Range(0, 60)]
    public float maxAngle = 30f;
    //[Range(0, 2000)]
    public float maxTorque = 500f;
    //[Range(0, 100000)]
    public float brakeTorque = 500000f;
    //[Range(1, 5)]
    public float accelerationMultiplier = 1.5f;
    public float handbrakeTorque = 10000;

    [Header("PERFORMANCE SETTINGS")]
    //[Range(1, 30)]
    public float targetAccelerationTime = 15f;
    //[Range(50, 400)]
    public float maxSpeed = 230f;
    //[Range(1, 3)]
    public float accelerationCurve = 1.5f;
    //[Range(10, 50)]
    public float maxReverseSpeed = 20f;

    [Header("REFERENCES")]
    public GameObject wheelShape;
    // Убрал camera и drone - они обычно не должны быть в конфиге
    // Убрал joystick, speedmeter, BrakeButton - это UI элементы, лучше настраивать в сцене
}
