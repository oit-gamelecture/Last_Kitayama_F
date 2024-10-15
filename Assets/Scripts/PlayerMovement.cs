using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float leftRightSpeed = 4.0f;
    Rigidbody rb;
    public Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);

        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * Time.deltaTime * leftRightSpeed);
        }

        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.left * Time.deltaTime * leftRightSpeed * -1);
        }
    }
}
