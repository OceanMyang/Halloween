using UnityEngine;

public class MainCameraBehavior : MonoBehaviour
{
    public Transform mainCam;
    public Transform head;
    public Transform knee;
    public float distance = 5;
    public float elevation = 45;

    private Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (mainCam == null)
        {
            mainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
        if (head == null)
        {
            head = GameObject.FindGameObjectWithTag("Head").transform;
        }
        if (knee == null)
        {
            knee = GameObject.FindGameObjectWithTag("Knee").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        offset = head.position - knee.position;
        Vector3 xz = -Vector3.forward.normalized * distance;
        Vector3 y = Vector3.up * distance;
        mainCam.position = knee.position - xz + y;
        mainCam.LookAt(knee.position);
    }
}
