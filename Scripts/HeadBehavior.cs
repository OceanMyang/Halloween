using UnityEditor.Rendering.Analytics;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Assertions;

public class HeadBehavior : MonoBehaviour
{
    public Transform knee;
    public float slideSpeed = 20f;
    public float springConstant = 4f;
    public float liftSpeed = 5f;
    public float shakeSpeed = 10f;
    public float maxDistance = 4f;
    public float minDistance = 2f;

    private Rigidbody rb;
    private Vector3 offset;
    private Vector3 direction;
    private float distance;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (knee == null)
        {
            knee = GameObject.FindGameObjectWithTag("Knee").transform;
        }
        if (minDistance > maxDistance)
        {
            Debug.LogError("minDistance can not be greater than maxDistance");
        }
    }

    void FixedUpdate()
    {
        offset = transform.position - knee.position;
        direction = offset.normalized;
        distance = offset.magnitude;

        SlideKnee();
        ClampPosition();
        LiftHead();
    }

    void SlideKnee()
    {
        Vector3 hookeForce = springConstant * direction * (distance - maxDistance);
        if (Input.GetKey(KeyCode.Space))
        {
            if (distance >= minDistance && distance <= maxDistance)
            {
                knee.position += direction  * slideSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (distance != maxDistance)
            {
                rb.AddForce(-hookeForce, ForceMode.Impulse);
            }
        }
    }

    void LiftHead()
    {
        transform.rotation = Quaternion.LookRotation(-direction, Vector3.up);
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        if (v != 0)
        {
            if (v < 0 || XZ(offset).magnitude > 0.1)
            {
                transform.position += v * liftSpeed * Time.deltaTime * transform.up;
            }
        }
        else if (h != 0)
        {
            transform.position -= h * shakeSpeed * Time.deltaTime * transform.right;
        }
        else
        {
            transform.position -= liftSpeed *  Time.deltaTime * transform.up;
        }
    }

    void ClampPosition()
    {
        /* Clamp Position */
        if (distance > maxDistance)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = knee.position + maxDistance * direction;
            knee.position = transform.position - maxDistance * direction;
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
