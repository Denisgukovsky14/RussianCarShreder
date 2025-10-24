using UnityEngine;
using System.Collections;
using Photon.Pun;
using RootMotion;
//using Unity.UI;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class RearWheelDrive : MonoBehaviour {
	
	private PhotonView view;
    private WheelCollider[] wheels;
    [SerializeField] private FixedJoystick joystick;

	public TextMeshProUGUI speedmeter;
	public float maxAngle = 30;
	public float maxTorque = 300;
    public float brakeTorque = 5000; // Усилие торможения
    public float accelerationMultiplier = 1.5f;
    bool isHandbrakeActive = false;

    //public float maxBrakeTorque = 3000f;        // Максимальный тормозной момент
    //public float brakeForceMultiplier = 0.8f;

    //private float deceleration;
    private float previousSpeed;
    private float currentSpeed;
	private float currentAcceleration = 1.5f;

    public GameObject wheelShape;
	public GameObject Camera;
    public GameObject Drone;

    private Rigidbody rb;

    public float targetAccelerationTime = 5f; // 20 секунд до 100 км/ч
    //public float decelerationRate = 1.5f;
    //public float accelerationFalloff = 0.8f; // Насколько сильно падает ускорение (0-1)
    private bool breakActivate = false;

    private float accelerationTimer = 0f;
    public float maxSpeed = 230f; // км/ч
    public float accelerationCurve = 1.5f;

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    // here we find all the WheelColliders down in the hierarchy
    public void Start()
    {
        //transform.parent.GetComponent<Camera>().GetComponent<CameraController>().target = transform;
        rb = GetComponentInParent<Rigidbody>();
        previousSpeed = rb.linearVelocity.magnitude;


        view = GetComponent<PhotonView>();
        //wheels = GetComponentsInChildren<WheelCollider>();

        if (view.IsMine)
        {
            wheels = GetComponentsInChildren<WheelCollider>();

            for (int i = 0; i < wheels.Length; ++i)
            {
                var wheel = wheels[i];

                // create wheel shapes only when needed
                if (wheelShape != null)
                {
                    Vector3 wheelPosition;
                    Quaternion wheelRotation;
                    wheel.GetWorldPose(out wheelPosition, out wheelRotation);

                    //Debug.Log(wheelPosition);

                    var ws = GameObject.Instantiate(wheelShape);

                    ws.transform.parent = wheel.transform;
                    ws.transform.position = wheelPosition; // Устанавливаем позицию
                    ws.transform.rotation = wheelRotation;
                    ws.transform.localScale = new Vector3(100f, 100f, 100f);


                    //ws.transform.localScale = new Vector3(ws.transform.localScale.x * -1 , ws.transform.localScale.y, ws.transform.localScale.z);


                    if (((i + 1) % 2) == 0)
                    {
                        //Debug.Log("Rescale");
                        ws.transform.localScale = new Vector3(ws.transform.localScale.x * -1, ws.transform.localScale.y, ws.transform.localScale.z);
                    }


                    //ws.GetComponent<PhotonView>().RPC("SetOwner", RpcTarget.AllBuffered, view.OwnerActorNr);
                }
            }
            //}

        }

        /*
        [PunRPC]
        public void UpdateWheelPose(Vector3 position, Quaternion rotation)
        {
            foreach (WheelCollider wheel in wheels)
            {
                Transform shapeTransform = wheel.transform.GetChild(0);
                shapeTransform.position = position;
                shapeTransform.rotation = rotation;
            }
        }
        */
    }

    // this is a really simple approach to updating wheels
    // here we simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero
    // this helps us to figure our which wheels are front ones and which are rear
    public void FixedUpdate()
	{
		if (view.IsMine )
		{
            //Debug.Log(view.IsMine);

            

			speedmeter.text = "Speed: " + Mathf.Round(currentSpeed) ;

#if UNITY_IOS || UNITY_ANDROID
            if (joystick)
            {
                float angle = maxAngle * joystick.Horizontal;
                float torque = maxTorque * joystick.Vertical;
            }
#else
            float angle = maxAngle * Input.GetAxis("Horizontal");
            float torque = maxTorque * Input.GetAxis("Vertical");
#endif







            //rb.linearVelocity = new Vector3(joystick.Horizontal * currentSpeed, rb.linearVelocity.y, joystick.Vertical * currentSpeed);

            currentSpeed = rb.linearVelocity.magnitude * 3.6f ;
			//currentAcceleration = (currentSpeed - previousSpeed) / Time.deltaTime;
			//currentAcceleration = currentSpeed / Time.deltaTime;
            previousSpeed = currentSpeed;
            float deceleration = -currentAcceleration;


            if (!breakActivate)
            {
                isHandbrakeActive = Input.GetKey(KeyCode.Space);
            }
            //        if (torque > 0 && !isHandbrakeActive)
            //        {
            //            rb.AddForce(transform.forward * currentAcceleration , ForceMode.Acceleration);

            //            currentAcceleration -= 2f * Time.deltaTime;
            //            currentAcceleration = Mathf.Max(currentAcceleration, 0f);
            //        }
            //        else
            //        {
            //currentAcceleration = 1f;
            //        }

            if (torque > 0 && !isHandbrakeActive)
            {
                float forwardVelocity = Vector3.Dot(rb.linearVelocity, transform.forward);

                if (forwardVelocity < -1f && currentSpeed <= 20)
                {
                    // Торможение когда едем вперед
                    angle *= 0.5f;
                    rb.AddForce(transform.forward * -forwardVelocity * 4.0f * rb.mass, ForceMode.Force);
                  
                }
                else
                {
                    angle /= 0.5f;
                    float currentSpeed = rb.linearVelocity.magnitude * 3.6f;
                    float speedRatio = currentSpeed / maxSpeed;

                    // Ускорение уменьшается по мере приближения к максимальной скорости
                    float accelerationMultiplier = 1f - Mathf.Pow(speedRatio, accelerationCurve);
                    float baseAcceleration = (100f / 3.6f) * 1.5f / targetAccelerationTime;
                    float currentAcceleration = baseAcceleration * accelerationMultiplier;

                    rb.AddForce(transform.forward * currentAcceleration * rb.mass, ForceMode.Force);
                }
            }
            //&& Vector3.Dot(rb.linearVelocity, transform.forward) < 0
            else if (torque < 0 && !isHandbrakeActive )
            {
                float forwardVelocity = Vector3.Dot(rb.linearVelocity, transform.forward);

                // Всегда сначала тормозим, потом едем назад
                if (forwardVelocity > 1f && currentSpeed <= 20)
                {
                    angle *= 0.5f;
                    // Торможение когда едем вперед
                    rb.AddForce(transform.forward * -forwardVelocity * 4.0f * rb.mass, ForceMode.Force);
                   
                }
                else
                {
                    angle /= 0.5f;
                    // Движение назад когда почти остановились
                    float currentSpeed = Mathf.Abs(forwardVelocity) * 3.6f; // Только скорость назад
                    float speedRatio = currentSpeed / (maxSpeed * 0.25f);

                    float accelerationMultiplier = Mathf.Clamp01(1f - Mathf.Pow(speedRatio, accelerationCurve));
                    float baseAcceleration = (100f / 3.6f) / targetAccelerationTime;
                    float currentAcceleration = baseAcceleration * accelerationMultiplier * 3.5f;

                    rb.AddForce(-transform.forward * currentAcceleration * rb.mass, ForceMode.Force);
                }
            }

            bool LetsDeath = Input.GetKey(KeyCode.V);

			if (LetsDeath)
			{
				Destroy(gameObject);
                
            }

            foreach (WheelCollider wheel in wheels)
			{
				// a simple car where front wheels steer while rear ones drive
				if (wheel.transform.localPosition.z > 0)
					wheel.steerAngle = angle;

				if (wheel.transform.localPosition.z < 0)
					if (isHandbrakeActive)
					{
                        wheel.brakeTorque = brakeTorque * accelerationMultiplier;

                        // Сила всегда противоположна текущему направлению движения
                        Vector3 oppositeForce = -rb.linearVelocity.normalized * 2f;
                        rb.AddForce(oppositeForce, ForceMode.Acceleration);

                        wheel.motorTorque = 0;
                    }
					else
					{
						wheel.brakeTorque = 0; // Отключаем тормоз
						wheel.motorTorque = torque * accelerationMultiplier ; // Применяем крутящий момент
					}

				// update visual wheels if any
				if (wheelShape)
				{
					Quaternion q;
					Vector3 p;
					wheel.GetWorldPose(out p, out q);

					// assume that the only child of the wheelcollider is the wheel shape
					Transform shapeTransform = wheel.transform.GetChild(0);
					shapeTransform.position = p;
					shapeTransform.rotation = q;

                    //view.RPC("UpdateWheelPose", RpcTarget.Others, p, q);
                }

			}
		}
	}

    public void OnHandbrakeButtonDown()
    {
        isHandbrakeActive = true;
        breakActivate = true;
        // Логика включения ручника
    }

    public void OnHandbrakeButtonUp()
    {
        isHandbrakeActive = false;
        breakActivate = false;
        // Логика выключения ручника
    }

    private void OnCollisionEnter(Collision collision)
	{
		if (view.IsMine)
		{

            Rigidbody otherRb = collision.rigidbody; // Получаем Rigidbody объекта, с которым произошло столкновение

            if (otherRb != null)
            {
                // Получаем массу и скорость другого объекта
                float otherMass = otherRb.mass;
                Vector3 otherVelocity = otherRb.linearVelocity;

                // Рассчитываем силу удара
                Vector3 impactForce = rb.linearVelocity * rb.mass + otherVelocity * otherMass; // Учитываем массу обоих объектов
                otherRb.AddForce(impactForce, ForceMode.Impulse);
            }
        }
	}
	
    private void OnDestroy()
    {
        if (view.IsMine)
		{


			gameObject.GetComponent<Explosion>().Explode();
			var drone = GameObject.Instantiate(Drone, transform.position, transform.rotation);
			drone.tag = transform.tag;
			//drone.GetComponent<DroneMovement>().Initialize(view);
            Camera.transform.SetParent(drone.transform);	
            Camera.transform.localPosition = Vector3.zero; 


        }
    }
	

}
