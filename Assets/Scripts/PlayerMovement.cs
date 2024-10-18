using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float leftRightSpeed = 4.0f;
    Rigidbody rb;
    public Animator animator;

    private bool canMove = false;

    public Image blackOverlay; 
    public float delayBeforeHiding = 3.3f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

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

        if (canMove)
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

    private IEnumerator EnableMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canMove = true;
    }

    private IEnumerator HideOverlayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Color color = blackOverlay.color; 
        color.a = 0f; 
        blackOverlay.color = color; 
        blackOverlay.gameObject.SetActive(false); 
    }

}
