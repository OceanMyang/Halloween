using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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

    public Rigidbody mover;
    public Rigidbody root;
    private CollisionHandler moverCd;
    private CollisionHandler rootCd;
    private Vector3 offset;
    private Vector3 direction;
    private float distance;
    private float hookeForce;
    private float angularSpeed;

    private bool isPressed;
    private float potentialEnergy;

    void Start()
    {
        if (head == null)
        {
            head = GameObject.FindGameObjectWithTag("Head").GetComponent<Rigidbody>();
        }
        if (knee == null)
        {
            knee = GameObject.FindGameObjectWithTag("Knee").GetComponent<Rigidbody>();
        }
        if (minDistance >= maxDistance)
        {
            Debug.LogError("minDistance can not be greater than maxDistance");
        }
        angularSpeed = 360 * liftSpeed / 100;
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            mover = knee;
            root = head;
        }
        else
        {
            mover = head;
            root = knee;
        }

        moverCd = mover.gameObject.GetComponent<CollisionHandler>();
        rootCd = root.gameObject.GetComponent<CollisionHandler>();

        offset = mover.position - root.position;
        direction = offset.normalized;
        distance = offset.magnitude;
        hookeForce = springConstant * (distance - maxDistance);

        SlideHead();
        ClampPosition();
        LiftHead();

        Debug.Log("Direction: " + direction);
    }

    void SlideHead()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            isPressed = true;
            potentialEnergy = 0.5f * springConstant * (distance - maxDistance) * (distance - maxDistance);
            if (distance > minDistance)
            {
                mover.AddForce(-direction * slideSpeed, ForceMode.Impulse);
            }
        }
        else
        {
            isPressed = false;
        }

        if (distance != maxDistance)
        {
            mover.AddForce(-direction * hookeForce, ForceMode.Impulse);
        }
    }

    void LiftHead()
    {
        //head.rotation = Quaternion.LookRotation(-direction, Vector3.up);
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        mover.rotation = Quaternion.LookRotation(direction);
        //if (v != 0)
        //{
        //    mover.useGravity = false;
        //    if (!(v > 0 && XZ(offset).magnitude <= 1))
        //    {
        //        head.position += v * liftSpeed * Time.deltaTime * head.transform.up;
        //    }
        //    head.linearVelocity -= Vector3.Project(head.linearVelocity, Physics.gravity);
        //    head.AddForce(-Physics.gravity, ForceMode.Acceleration);
        //}
        //else if (h != 0)
        //{
        //    mover.useGravity = false;
        //    head.position += h * liftSpeed * Time.deltaTime * head.transform.right;
        //    head.linearVelocity -= Vector3.Project(head.linearVelocity, Physics.gravity);
        //    head.AddForce(-Physics.gravity, ForceMode.Acceleration);
        //}

        float theta = 0f;
        float phi = 0f;
        if (v != 0 || h != 0)
        {
            mover.useGravity = false;

            float vDelta = v * angularSpeed * Time.deltaTime;
            float hDelta = h * angularSpeed * Time.deltaTime;

            mover.position = root.position + SphericalCoordinates(vDelta, hDelta, out theta, out phi);
        }
        else
        {
            mover.useGravity = false;
            SphericalCoordinates(0f, 0f, out theta, out phi);
        }

        Debug.Log("Theta: " + theta * Mathf.Rad2Deg);
        Debug.Log("Phi: " + phi * Mathf.Rad2Deg);
    }

    Vector3 SphericalCoordinates(float vDelta, float hDelta, out float theta, out float phi)
    {
        float phiDeg = Mathf.Abs(Vector3.SignedAngle(Vector3.up, direction, Vector3.right));
        float thetaDeg = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up) + 180;
        phi = (phiDeg - vDelta) * Mathf.Deg2Rad;
        theta = (thetaDeg + hDelta) * Mathf.Deg2Rad;

        float x = -Mathf.Sin(theta) * Mathf.Sin(phi);
        float y = Mathf.Cos(phi);
        float z = -Mathf.Cos(theta) * Mathf.Sin(phi);

        Vector3 newCoord = distance * new Vector3(x, y, z);

        return newCoord;
    }

    void ClampPosition()
    {
        /* Clamp Position */
        Vector3 headVelocity = Vector3.Project(mover.linearVelocity, direction);
        Vector3 kneeVelocity = Vector3.Project(root.linearVelocity, -direction);
        if (distance <= minDistance && headVelocity.magnitude != 0)
        {
            mover.AddForce(-headVelocity, ForceMode.VelocityChange);
        }
        if (distance >= maxDistance && headVelocity.magnitude != 0)
        {
            mover.AddForce(-headVelocity, ForceMode.VelocityChange);
            if (rootCd.isColliding && !isPressed)
            {
                root.AddForce(direction * potentialEnergy, ForceMode.Impulse);
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

