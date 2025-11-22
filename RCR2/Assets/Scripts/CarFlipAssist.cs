
using UnityEngine;
using Photon.Pun;

public class CarFlipAssist : MonoBehaviour
{
    [Header("Flip Assist Settings")]
    public bool enableFlipAssist = true;
    public float flipForce = 800f;   // ”величил дл€ надежности
    public float flipTorque = 400f;  // ”величил дл€ надежности
    public float flipCooldown = 3f;

    private PhotonView view;
    private Rigidbody carRigidbody;
    private bool isFlipping = false;
    private float lastFlipTime = 0f;

    void Start()
    {
        view = GetComponent<PhotonView>();
        carRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!enableFlipAssist)
            return;

        // ƒл€ отладки можно убрать проверки и принудительно переворачивать через Debug клавишу
        if (!isFlipping && Time.time - lastFlipTime > flipCooldown)
        {
            // ѕровер€ем насколько машина наклонена
            float angle = Vector3.Angle(transform.up, Vector3.up);

            // ≈сли машина перевернута или на боку и почти не двигаетс€
            if ((angle > 75f) && carRigidbody.linearVelocity.magnitude < 0.1f)
            {
                FlipCarBack();
            }
        }
    }

    void FlipCarBack()
    {
        // —разу исправл€ем вращение машины
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        // ћожно сбросить скорость и угловую скорость дл€ устойчивости
        carRigidbody.linearVelocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
    }

    private void ResetFlipFlag()
    {
        isFlipping = false;
        Debug.Log("Flip assist ready again");
    }

    // ƒл€ теста: принудительный переворот через меню компонента
    [ContextMenu("Force Flip Car")]
    public void ForceFlipCar()
    {
        if (enableFlipAssist && !isFlipping)
        {
            FlipCarBack();
        }
    }
}
