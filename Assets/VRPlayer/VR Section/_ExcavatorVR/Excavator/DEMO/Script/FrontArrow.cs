using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class FrontArrow : MonoBehaviour
{

    public KeyCode KeyFOR1;
    public KeyCode KeyAB1;
    public KeyCode KeyBAK1;
    public KeyCode KeyFOR2;
    public KeyCode KeyAB2;
    public KeyCode KeyBAK2;
    public Transform Det;
    public Transform arm;
    public Transform plate;
    private Vector3 armRot;
    private Vector3 plateRot;
    private Vector3 DetRot;
    public float DetSpeed;
    public float armSpeed;
    public float plateSpeed;
    public float minArmAngle;
    public float maxArmAngle;
    private float minDetAngle;
    private float maxDetAngle;
    public float minPlateAngle;
    public float maxPlateAngle;
    public Transform Pis1T;
    public Transform Pis2T;
    public Transform Pis1J;
    public Transform Pis2J;
    public Transform Pis1K;
    public Transform Pis2K;
    public Transform Pis1H;
    public Transform Pis2H;
    public Transform Pis1C;
    public Transform Pis2C;
    public AudioSource audioF;

    void Start()
    {
        armRot = arm.localEulerAngles;
        plateRot = plate.localEulerAngles;
        DetRot = Det.localEulerAngles;
        audioF = GetComponent<AudioSource>();
    }

    void Update2()
    {

        if (minArmAngle > maxArmAngle)
        {
            float t = maxArmAngle;
            maxArmAngle = minArmAngle;
            minArmAngle = t;
        }
        if (minPlateAngle > maxPlateAngle)
        {
            float t = maxPlateAngle;
            maxPlateAngle = minPlateAngle;
            minPlateAngle = t;
        }
        if (minDetAngle > maxDetAngle)
        {
            float t = maxDetAngle;
            maxDetAngle = minDetAngle;
            minDetAngle = t;
        }
        if (Input.GetKey(KeyAB1) && Input.GetKey(KeyFOR1) && armRot.x + armSpeed * Time.deltaTime >= minArmAngle && armRot.x + armSpeed * Time.deltaTime <= maxArmAngle)
        {
            armRot.x = armRot.x + armSpeed * Time.deltaTime;
            plateRot.x = plateRot.x - armSpeed * Time.deltaTime;
            DetRot.x = DetRot.x - armSpeed * Time.deltaTime;
            audioF.pitch = 1.14f;
        }
        else if (Input.GetKey(KeyAB1) && Input.GetKey(KeyBAK1) && armRot.x - armSpeed * Time.deltaTime >= minArmAngle && armRot.x - armSpeed * Time.deltaTime <= maxArmAngle)
        {
            armRot.x = armRot.x - armSpeed * Time.deltaTime;
            plateRot.x = plateRot.x + armSpeed * Time.deltaTime;
            DetRot.x = DetRot.x + armSpeed * Time.deltaTime;
            audioF.pitch = 1.14f;
        }
        if (Input.GetKey(KeyAB2) && Input.GetKey(KeyFOR2) && plateRot.x + plateSpeed * Time.deltaTime >= minPlateAngle && plateRot.x + plateSpeed * Time.deltaTime <= maxPlateAngle)
        {
            plateRot.x = plateRot.x + plateSpeed * Time.deltaTime;
            audioF.pitch = 1.14f;
        }
        else if (Input.GetKey(KeyAB2) && Input.GetKey(KeyBAK2) && plateRot.x - plateSpeed * Time.deltaTime >= minPlateAngle && plateRot.x - plateSpeed * Time.deltaTime <= maxPlateAngle)
        {
            plateRot.x = plateRot.x - plateSpeed * Time.deltaTime;
            audioF.pitch = 1.14f;
        }
        if (Input.GetKeyUp(KeyFOR1))
        {
            audioF.pitch = 1f;
        }
        else if (Input.GetKeyUp(KeyBAK1))
        {
            audioF.pitch = 1f;
        }
        else if (Input.GetKeyUp(KeyFOR2))
        {
            audioF.pitch = 1f;
        }
        else if (Input.GetKeyUp(KeyBAK2))
        {
            audioF.pitch = 1f;
        }
        arm.localEulerAngles = armRot;
        plate.localEulerAngles = plateRot;
        Det.localEulerAngles = DetRot;

    }

    void Update()
    {
        arm.localEulerAngles = armRot;
        plate.localEulerAngles = plateRot;
        Det.localEulerAngles = DetRot;

    }


    public void BucketUp()
    {
        if (armRot.x + armSpeed * Time.deltaTime >= minArmAngle && armRot.x + armSpeed * Time.deltaTime <= maxArmAngle)
        {
            armRot.x = armRot.x + armSpeed * Time.deltaTime;
            // plateRot.x = plateRot.x - armSpeed * Time.deltaTime;
            DetRot.x = DetRot.x - armSpeed * Time.deltaTime;
            audioF.pitch = 1.14f;
        }
    }

	public void BucketDown(){
		 if (armRot.x - armSpeed * Time.deltaTime >= minArmAngle && armRot.x - armSpeed * Time.deltaTime <= maxArmAngle)
        {
            armRot.x = armRot.x - armSpeed * Time.deltaTime;
            // plateRot.x = plateRot.x + armSpeed * Time.deltaTime;
            DetRot.x = DetRot.x + armSpeed * Time.deltaTime;
            audioF.pitch = 1.14f;
        }
	}

	public void BucketPlateUp()
    {
        if (plateRot.x + plateSpeed * Time.deltaTime >= minPlateAngle && plateRot.x + plateSpeed * Time.deltaTime <= maxPlateAngle)
        {
            plateRot.x = plateRot.x + plateSpeed * Time.deltaTime;
            audioF.pitch = 1.14f;
        }
    }

	public void BucketPlateDown(){
		if ( plateRot.x - plateSpeed * Time.deltaTime >= minPlateAngle && plateRot.x - plateSpeed * Time.deltaTime <= maxPlateAngle)
        {
            plateRot.x = plateRot.x - plateSpeed * Time.deltaTime;
            audioF.pitch = 1.14f;
        }
	}

    void LateUpdate()
    {

        if (Pis1T != null && Pis2T != null)
        {
            Pis1T.LookAt(Pis2T.position, Pis1T.up);
            Pis2T.LookAt(Pis1T.position, Pis2T.up);
        }
        if (Pis1J != null && Pis2J != null)
        {
            Pis1J.LookAt(Pis2J.position, Pis1J.up);
            Pis2J.LookAt(Pis1J.position, Pis2J.up);
        }
        if (Pis1H != null && Pis2H != null)
        {
            Pis1H.LookAt(Pis2H.position, Pis1H.up);
            Pis2H.LookAt(Pis1H.position, Pis2H.up);
        }
        if (Pis1K != null && Pis2K != null)
        {
            Pis1K.LookAt(Pis2K.position, Pis1K.up);
            Pis2K.LookAt(Pis1K.position, Pis2K.up);
        }
        if (Pis1C != null && Pis2C != null)
        {
            Pis1C.LookAt(Pis2C.position, Pis1C.up);
            Pis2C.LookAt(Pis1C.position, Pis2C.up);
        }
    }
}