using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float leftRightSpeed = 4.0f;
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
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);

        if (canMove && isWalking)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(Vector3.left * Time.deltaTime * leftRightSpeed);
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector3.left * Time.deltaTime * leftRightSpeed * -1);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy") && !isFalling)
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
