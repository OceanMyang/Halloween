using UnityEngine;

public class BaseBehavior : MonoBehaviour
{
    public Rigidbody head;
    public Rigidbody knee;
    public float slideSpeed = 10f;
    public float springConstant = 4f;
    public float liftSpeed = 20f;
    public float shakeSpeed = 10f;
    public float maxDistance = 4f;
    public float minDistance = 2f;

    private CollisionDetector headCd;
    private CollisionDetector kneeCd;
    private Vector3 offset;
    private Vector3 direction;
    private float distance;
    private Vector3 hookeForce;

    void Start()
    {
        if (head == null)
        {
            head = GameObject.FindGameObjectWithTag("Head").GetComponent<Rigidbody>();
            headCd = head.gameObject.GetComponent<CollisionDetector>();
        }
        if (knee == null)
        {
            knee = GameObject.FindGameObjectWithTag("Knee").GetComponent<Rigidbody>();
            kneeCd = knee.gameObject.GetComponent<CollisionDetector>();
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
        hookeForce = springConstant * direction * (distance - maxDistance);

        SlideKnee();
        ClampPosition();
        LiftHead(); 
    }

    void SlideKnee()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (distance > minDistance)
            {
                head.AddForce(-direction * slideSpeed, ForceMode.Impulse);
            }
        }
        if (distance != maxDistance)
        {
            head.AddForce(-hookeForce, ForceMode.Impulse);
        }
    }

    void LiftHead()
    {
        head.rotation = Quaternion.LookRotation(-direction, Vector3.up);
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        if (v != 0)
        {
            if (v < 0 || XZ(offset).magnitude > 0.1)
            {
                head.position += v * liftSpeed * Time.deltaTime * head.transform.up;
            }
        }
        else if (h != 0)
        {
            head.position -= h * shakeSpeed * Time.deltaTime * head.transform.right;
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
            if (kneeCd.isColliding)
            {
                knee.AddForce(hookeForce, ForceMode.Impulse);
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

