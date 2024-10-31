using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public float minDamping = 1f;
    public float maxDamping = 10f;
    public bool isColliding = false;
    public Vector3 normalVector = Vector3.zero;

    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        isColliding = true;
        rb.angularDamping = maxDamping;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.contactCount  == 0)
        {
            isColliding = false;
            normalVector = Vector3.zero;
        }
        else
        {
            normalVector = collision.contacts[0].normal;
            rb.AddForce(-2 * normalVector, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
        normalVector = Vector3.zero;
        rb.angularDamping = minDamping;
    }
}
