using UnityEngine;
using System.Collections;
using Photon.Pun;
using RootMotion;

public class RearWheelDrive : MonoBehaviour {
	
	private PhotonView view;
    private WheelCollider[] wheels;

	public float maxAngle = 30;
	public float maxTorque = 300;
    public float brakeTorque = 5000; // Усилие торможения

    public GameObject wheelShape;
	public GameObject Camera;
    public GameObject Drone;

    private Rigidbody rb;

    // here we find all the WheelColliders down in the hierarchy
    public void Start()
	{
        //transform.parent.GetComponent<Camera>().GetComponent<CameraController>().target = transform;

        rb = GetComponentInParent<Rigidbody>();

        view = GetComponent<PhotonView>();
		//wheels = GetComponentsInChildren<WheelCollider>();

		//if (view.IsMine)
		//{
			wheels = GetComponentsInChildren<WheelCollider>();
			//Debug.Log("BaboRab");
			for (int i = 0; i < wheels.Length; ++i)
			{
				var wheel = wheels[i];

				// create wheel shapes only when needed
				if (wheelShape != null)
				{
					Vector3 wheelPosition;
					Quaternion wheelRotation;
					wheel.GetWorldPose(out wheelPosition, out wheelRotation);

					Debug.Log(wheelPosition);

					var ws = GameObject.Instantiate(wheelShape);

					ws.transform.parent = wheel.transform;
					ws.transform.position = wheelPosition; // Устанавливаем позицию
					ws.transform.rotation = wheelRotation;
				    
					//ws.transform.localScale = new Vector3(ws.transform.localScale.x * -1 , ws.transform.localScale.y, ws.transform.localScale.z);

                
				if (((i + 1) % 2) == 0)
				{
					Debug.Log("Rescale");
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

    // this is a really simple approach to updating wheels
    // here we simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero
    // this helps us to figure our which wheels are front ones and which are rear
    public void FixedUpdate()
	{
		if (view.IsMine )
		{
		Debug.Log(view.IsMine);

			float angle = maxAngle * Input.GetAxis("Horizontal");
			float torque = maxTorque * Input.GetAxis("Vertical");

			bool isHandbrakeActive = Input.GetKey(KeyCode.Space);

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
						wheel.brakeTorque = brakeTorque; // Применяем тормозное усилие
						wheel.motorTorque = 0; // Отключаем двигатель
					}
					else
					{
						wheel.brakeTorque = 0; // Отключаем тормоз
						wheel.motorTorque = torque; // Применяем крутящий момент
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

	private void OnCollisionEnter(Collision collision)
	{
		if (view.IsMine)
		{

            Rigidbody otherRb = collision.rigidbody; // Получаем Rigidbody объекта, с которым произошло столкновение

            if (otherRb != null)
            {
                // Получаем массу и скорость другого объекта
                float otherMass = otherRb.mass;
                Vector3 otherVelocity = otherRb.velocity;

                // Рассчитываем силу удара
                Vector3 impactForce = rb.velocity * rb.mass + otherVelocity * otherMass; // Учитываем массу обоих объектов
                otherRb.AddForce(impactForce, ForceMode.Impulse);
            }
        }
	}
	
    private void OnDestroy()
    {
        if (view.IsMine)
		{

			Debug.Log("Заспавнен пожилой дрон");

			gameObject.GetComponent<Explosion>().Explode();
			var drone = GameObject.Instantiate(Drone, transform.position, transform.rotation);
			drone.tag = transform.tag;
			//drone.GetComponent<DroneMovement>().Initialize(view);
            Camera.transform.SetParent(drone.transform);	
            Camera.transform.localPosition = Vector3.zero; 


        }
    }
	

}
