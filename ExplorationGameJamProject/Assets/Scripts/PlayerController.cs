using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 100f;
    [SerializeField] private float movementSmoothTime = 0.4f;

    private Rigidbody rb;
    private Vector3 velocity;
    private float xInput;
    private float zInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float targetVelocityX = baseSpeed * xInput * Time.fixedDeltaTime;
        float targetVelocityZ = baseSpeed * zInput * Time.fixedDeltaTime;
        Vector3 targetVelocity = new Vector3(targetVelocityX, rb.velocity.y, targetVelocityZ);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothTime);
    }

    public void OnMove(InputValue value)
    {
        Vector2 motionVector = value.Get<Vector2>();
        xInput = motionVector.x;
        zInput = motionVector.y;
    }
}
