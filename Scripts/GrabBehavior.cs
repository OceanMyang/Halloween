using UnityEngine;

public class GrabBehavior : MonoBehaviour
{
    bool isGrabbing = false;
    Vector3 normalVector = Vector3.zero;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical");
        transform.position += 5 *v * Vector3.forward * Time.deltaTime;

        float h = Input.GetAxis("Horizontal");
        transform.position -= 5 * h * Vector3.right * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += 10 * Vector3.up * Time.deltaTime;
        }

        if (Input.GetMouseButton(0))
        {
            Debug.Log("IS GRABBING");
            isGrabbing = true;
        }
        else
        {
            Debug.Log("IS NOT GRABBING");
            isGrabbing = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (isGrabbing)
        {
            if (collision.contactCount > 0)
            {
                normalVector = collision.contacts[0].normal;
                rb.AddForce(-2 * normalVector, ForceMode.VelocityChange);
            }
            else
            {
                normalVector = Vector3.zero;
            }
        }
    }
}
