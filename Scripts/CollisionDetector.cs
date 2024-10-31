using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public bool isColliding = false;
    public Vector3 normalVector = Vector3.zero;

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
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
        normalVector = Vector3.zero;
    }
}
