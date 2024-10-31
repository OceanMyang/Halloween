using UnityEngine;

public class WormBehavior : MonoBehaviour
{
    public Rigidbody head;
    public Rigidbody knee;
    public float slideSpeed = 10f;
    public float springConstant = 4f;
    public float liftSpeed = 20f;
    public float shakeSpeed = 10f;
    public float maxDistance = 4f;
    public float minDistance = 2f;

    private CollisionHandler headCd;
    private CollisionHandler kneeCd;
    private Vector3 offset;
    private Vector3 direction;
    private float distance;
    private float hookeForce;

    private bool isPressed;
    private float potentialEnergy;

    void Start()
    {
        if (head == null)
        {
            head = GameObject.FindGameObjectWithTag("Head").GetComponent<Rigidbody>();
            headCd = head.gameObject.GetComponent<CollisionHandler>();
        }
        if (knee == null)
        {
            knee = GameObject.FindGameObjectWithTag("Knee").GetComponent<Rigidbody>();
            kneeCd = knee.gameObject.GetComponent<CollisionHandler>();
        }
        if (minDistance >= maxDistance)
        {
            Debug.LogError("minDistance can not be greater than maxDistance");
        }
    }

    void FixedUpdate()
    {
        offset = head.position - knee.position;
        direction = offset.normalized;
        distance = offset.magnitude;
        hookeForce = springConstant * (distance - maxDistance);

        Debug.Log(potentialEnergy);

        SlideHead();
        ClampPosition();
        LiftHead();
    }

    void SlideHead()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            isPressed = true;
            potentialEnergy = 0.5f * springConstant * (distance - maxDistance) * (distance - maxDistance);
            if (distance > minDistance)
            {
                head.AddForce(-direction * slideSpeed, ForceMode.Impulse);
            }
        }
        else
        {
             isPressed = false;
        }

        if (distance != maxDistance)
        {
            head.AddForce(-direction * hookeForce, ForceMode.Impulse);
        }
    }

    void LiftHead()
    {
        head.rotation = Quaternion.LookRotation(-direction, Vector3.up);
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        if (v != 0)
        {
            head.useGravity = false;
            if (v < 0 || XZ(offset).magnitude > 1)
            {
                head.position += v * liftSpeed * Time.deltaTime * head.transform.up;
            }
            //head.linearVelocity -= Vector3.Project(head.linearVelocity, Physics.gravity);
            //head.AddForce(-Physics.gravity, ForceMode.Acceleration);
        }
        else if (h != 0)
        {
            head.useGravity = false;
            head.position -= h * shakeSpeed * Time.deltaTime * head.transform.right;
            //head.linearVelocity -= Vector3.Project(head.linearVelocity, Physics.gravity);
            //head.AddForce(-Physics.gravity, ForceMode.Acceleration);
        }
        else
        {
            head.useGravity = true;
        }
    }

    void ClampPosition()
    {
        /* Clamp Position */
        Vector3 headVelocity = Vector3.Project(head.linearVelocity, direction);
        Vector3 kneeVelocity = Vector3.Project(knee.linearVelocity, -direction);
        if (distance <= minDistance && headVelocity.magnitude != 0)
        {
            head.AddForce(-headVelocity, ForceMode.VelocityChange);
        }
        if (distance >= maxDistance && headVelocity.magnitude != 0)
        {
            head.AddForce(-headVelocity, ForceMode.VelocityChange);
            if (kneeCd.isColliding && !isPressed)
            {
                knee.AddForce(direction * potentialEnergy, ForceMode.Impulse);
                potentialEnergy = 0;
            }
        } 
    }

    Vector3 XZ(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    Vector3 Y(Vector3 vector)
    {
        return new Vector3(0, vector.y, 0);
    }
}

