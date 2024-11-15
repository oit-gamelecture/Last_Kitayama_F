using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("�v���C���[�ݒ�")]
    public float moveSpeed = 3.0f;
    public float leftRightSpeed = 4.0f;
    public float knockBackSpeed = 2.0f;
    public float gravity = -3f;
    private Rigidbody rb;
    private Animator animator;
    public AudioClip slip;
    public AudioClip bone;
    AudioSource audioSource;

    public float movementInputValue;

    private bool canMove = false;
    private bool isFalling = false;
    private bool isWalking = false;

    public Image blackOverlay;
    public float delayBeforeHiding = 3.3f;

    [Header("�_���[�W�G�t�F�N�g")]
    public Renderer playerRenderer; // �v���C���[�̃}�e���A���𐧌�
    public Color blinkColor = Color.red; // �_�Ŏ��̐F
    public float blinkDuration = 0.1f; // �_�ŊԊu
    public int blinkCount = 5; // �_�ŉ�

    [Header("��ʗh��")]
    public Transform cameraTransform; // �J������Transform
    public float shakeDuration = 1f; // �h��̎���
    public float shakeMagnitude = 0.1f; // �h��̋���

    private Color originalColor; // �v���C���[�̌��̐F
    private Vector3 originalCameraPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        Color color = blackOverlay.color;
        color.a = 0.5f;
        blackOverlay.color = color;

        blackOverlay.gameObject.SetActive(true);
        StartCoroutine(IdleCoroutine());
        StartCoroutine(HideOverlayAfterDelay(delayBeforeHiding));

        Debug.Log(moveSpeed);

        // �v���C���[�̏����ݒ�
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }

        // �J�����̏����ʒu��ۑ�
        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.position;
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(0f, gravity, 0f));

        if (canMove)
        {
            // �O���ړ�
            Vector3 forwardDirection = transform.forward * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            // ���E�ړ�
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

                movementInputValue = Input.GetAxis("Horizontal");
                Vector3 movement = transform.right * movementInputValue * leftRightSpeed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + movement);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("enemy") && !isFalling)
        {
            if (Random.Range(0f, 1f) < 0.2f)
            {
                audioSource.PlayOneShot(bone);
            }
            else
            {
                audioSource.PlayOneShot(slip);
            }
            StartCoroutine(HandleFalling());
        }

        if (collision.gameObject.CompareTag("enemy"))
        {
            // �Ԃ��_�łƉ�ʗh����g���K�[
            StartCoroutine(BlinkEffect());
            StartCoroutine(CameraShake());
        }
    }

    private IEnumerator BlinkEffect()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            playerRenderer.material.color = blinkColor;
            yield return new WaitForSeconds(blinkDuration);
            playerRenderer.material.color = originalColor;
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    private IEnumerator CameraShake()
    {
        float elapsed = 0f;

        Vector3 initialPosition = cameraTransform.localPosition; // ���[�J�����W����ɂ���

        while (elapsed < shakeDuration)
        {
            // �����ʒu����Ƀ����_���ȗh���������
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;

            // �����iY���W�j�͈��ɂ���i�㉺�ɗh�炵�����Ȃ��ꍇ�j
            

            // �J�����ʒu���X�V
            cameraTransform.localPosition = initialPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // �J���������̈ʒu�ɖ߂�
        cameraTransform.localPosition = initialPosition;
    }

    private IEnumerator IdleCoroutine()
    {
        // Idle�A�j���[�V�����̊J�n
        animator.SetBool("Idle", true);
        yield return new WaitForSeconds(3.3f);

        // Idle�A�j���[�V�������~���AWalk�A�j���[�V�������J�n
        animator.SetBool("Idle", false);
        animator.SetTrigger("Walk");
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
        canMove = false;
        isFalling = true;
        animator.SetTrigger("Falling");
        yield return new WaitForSeconds(0.5f);

        Vector3 reverseDirection = -transform.forward;
        float moveDuration = 0.6f;
        float elapsedTime = 0f;

        animator.SetTrigger("GetUp");

        while (elapsedTime < moveDuration)
        {
            Vector3 newPosition = rb.position + reverseDirection * knockBackSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        animator.SetTrigger("Walk");
        canMove = true;
        isWalking = true;
        isFalling = false;
    }
}
