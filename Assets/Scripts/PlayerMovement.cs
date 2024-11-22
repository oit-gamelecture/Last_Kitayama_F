using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("プレイヤー設定")]
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
    private bool isGuarding = false; // ガード状態を管理
    private bool flag = true;
    private bool isRotating = false;

    public Image blackOverlay;
    public float delayBeforeHiding = 3.3f;

    [Header("画面揺れ")]
    public Transform cameraTransform;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 0.1f;

    [Header("パーティクル設定")]
    public ParticleSystem impactParticlePrefab;
    public float particleDuration = 2f;

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
    }

    private void Update()
    {
        HandleGuardInput();
        UpdateAnimationState();
    }

    public void SetMovement(bool enabled)
    {
        canMove = enabled;
    }

    public void SetRotating(bool rotating)
    {
        isRotating = rotating;
    }


    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(0f, gravity, 0f));

        if (canMove)
        {
            float currentSpeed = isRotating ? moveSpeed * 0.5f : moveSpeed; // 回転中は速度を半分に
            Vector3 forwardDirection = transform.forward * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            if (!flag) return;

            if (isWalking)
            {
                float currentLeftRightSpeed = isRotating ? leftRightSpeed * 0.5f : leftRightSpeed;
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    Vector3 leftDirection = -transform.right * currentLeftRightSpeed * Time.fixedDeltaTime;
                    rb.MovePosition(rb.position + leftDirection);
                }

                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    Vector3 rightDirection = transform.right * currentLeftRightSpeed * Time.fixedDeltaTime;
                    rb.MovePosition(rb.position + rightDirection);
                }
            }
        }
    }

    private void HandleGuardInput()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Eキーが押されたとき
        {
            isGuarding = true;
            canMove = false; // 移動を禁止
        }
        else if (Input.GetKeyUp(KeyCode.E)) // Eキーが離されたとき
        {
            isGuarding = false;
            canMove = true; // 移動を許可
        }
    }


    private void UpdateAnimationState()
    {
        // アニメーションの状態が既に正しい場合は処理しない
        if (animator.GetBool("Guard") != isGuarding)
        {
            animator.SetBool("Guard", isGuarding);

            if (!isGuarding)
            {
                animator.SetTrigger("Walk");
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("enemy"))
        {
            if (isGuarding) return; // ガード中は処理をスキップ

            if (!isFalling)
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

            StartCoroutine(CameraShake());

            ContactPoint contact = collision.contacts[0];
            Vector3 spawnPosition = transform.position;
            Quaternion spawnRotation = Quaternion.identity;

            if (impactParticlePrefab != null)
            {
                spawnPosition += new Vector3(0, 1.0f, 0);
                ParticleSystem particleInstance = Instantiate(impactParticlePrefab, spawnPosition, spawnRotation);

                float newScale = 8.0f;
                particleInstance.transform.localScale = new Vector3(newScale, newScale, newScale);

                Destroy(particleInstance.gameObject, particleDuration);
            }

            Debug.Log("Enemyに衝突しました！");
        }
    }

    private IEnumerator IdleCoroutine()
    {
        animator.SetBool("Idle", true);
        yield return new WaitForSeconds(3.3f);

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

    private IEnumerator CameraShake()
    {
        float elapsed = 0f;
        Vector3 initialPosition = cameraTransform.localPosition;

        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            cameraTransform.localPosition = initialPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.localPosition = initialPosition;
    }
}
