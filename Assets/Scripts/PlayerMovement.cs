using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("プレイヤー設定")]
    public float moveSpeed = 3.0f;
    public float leftRightSpeed = 4.0f;
    public float gravity = -3f;
    Rigidbody rb;
    private Animator animator;

    private bool canMove = false;
    private bool isFalling = false;
    private bool isWalking = false;


    public Image blackOverlay; 
    public float delayBeforeHiding = 3.3f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Color color = blackOverlay.color;
        color.a = 0.5f; 
        blackOverlay.color = color;

        blackOverlay.gameObject.SetActive(true);
        StartCoroutine(EnableMovementAfterDelay(3.3f));
        StartCoroutine(HideOverlayAfterDelay(delayBeforeHiding));

        Debug.Log(moveSpeed);
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(0f, gravity, 0f)); //= new Vector3(rb.velocity.x, rb.velocity.y * gravity * Time.fixedDeltaTime, rb.velocity.z);

        if (canMove)
        {
            // 前方移動
            Vector3 moveDirection = transform.forward * moveSpeed ;
            rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, rb.velocity.z);

            // 左右移動
            if (isWalking)
            {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    Vector3 leftDirection = -transform.right * leftRightSpeed * Time.fixedDeltaTime;
                    rb.MovePosition(rb.position + leftDirection);
                }

                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    Vector3 rightDirection = transform.right * leftRightSpeed * Time.fixedDeltaTime;
                    rb.MovePosition(rb.position + rightDirection);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("enemy") && !isFalling)
        {
            isFalling = true;
            animator.SetTrigger("Falling");
            StartCoroutine(HandleFalling());
        }
    }

    private IEnumerator EnableMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canMove = true;
        isWalking = true;
    }

    private IEnumerator HideOverlayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Color color = blackOverlay.color; 
        color.a = 0f; 
        blackOverlay.color = color; 
        blackOverlay.gameObject.SetActive(false); 
    }

    private IEnumerator HandleFalling()
    {
        isWalking = false;
        yield return new WaitForSeconds(1f); 

        Vector3 reverseDirection = transform.forward; 
        float moveDuration = 1.5f; 
        float elapsedTime = 0f;

        animator.SetTrigger("GetUp");
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        
        while (elapsedTime < moveDuration)
        {
            
            transform.Translate(reverseDirection * Time.deltaTime * moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        animator.SetTrigger("Walk");
        isWalking = true;
        isFalling = false; 
    }
}
