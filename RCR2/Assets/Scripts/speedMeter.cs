using TMPro;
using UnityEngine;

public class speedMeter : MonoBehaviour
{
    [SerializeField] GameObject Car;
    private RearWheelDrive drivescript;
    float angle = 0;

    private void Start()
    {
        this.transform.eulerAngles = new Vector3(0, 0, angle);
        drivescript = Car.GetComponent<RearWheelDrive>();
    }

    private void Update()
    {
        if (drivescript != null)
        {
            angle = (int)Mathf.Round( Mathf.Abs(drivescript.GetCurrentSpeed()) * -1.8f)   ;
            this.transform.eulerAngles = new Vector3(0, 0, angle + 90);
        }
    }

}
