using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    public float gravity = 5.0f;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        rb.AddForce(gravity * Vector3.down, ForceMode.Acceleration);
    }
}
