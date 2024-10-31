using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
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
    }
}
