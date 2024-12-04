using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("�v���C���[�ݒ�")]
    public float moveSpeed = 3.0f;
    public float avoidMoveSpeed = 1.0f;
    public float leftRightSpeed = 4.0f;
    public float avoidSpeed = 8.0f;
    public float knockBackSpeed = 2.0f;
    public float gravity = -3f;
    private Rigidbody rb;
    private Animator animator;
    public AudioClip slip;
    public AudioClip bone;
    AudioSource audioSource;

    public float movementInputValue;

    private bool canMove = false;
    public bool isFalling = false;
    private bool isWalking = false;
    public bool isGuarding = false; // �K�[�h��Ԃ��Ǘ�
    private bool flag = true;
    private bool isRotating = false;
    private bool canGuard = false;
    private bool isCameraShaking = false; // �J�����V�F�C�N�̏�Ԃ�ǐ�
    private Vector3 initialCameraPosition; // �J�����̏����ʒu���L�^


    public Image blackOverlay;
    public float delayBeforeHiding = 3.3f;

    [Header("��ʗh��")]
    public Transform cameraTransform;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 0.1f;

    [Header("�p�[�e�B�N���ݒ�")]
    public ParticleSystem impactParticlePrefab;
    public float particleDuration = 2f;

    private bool canUseQ = false; // Q�L�[���g�p�\���ǂ���
    private bool isUsingQ = false; // Q�L�[���������ǂ���
    private Coroutine currentQActionCoroutine;
    private bool canUseE = false;
    private bool isUsingE = false;
    private Coroutine currentEActionCoroutine;

    [Header("Raycast �ݒ�")]
    public float raycastDistance = 5.0f; // Ray�̋���
    public Transform raycastOriginLeft; // ����Ray���ˈʒu
    public Transform raycastOriginRight; // �E��Ray���ˈʒu

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
        CheckForEnemiesWithRaycast();

        if (canUseQ && Input.GetKeyDown(KeyCode.JoystickButton4) || canUseQ && Input.GetKeyDown(KeyCode.Q)) // joystickbutton4 for Q
        {
            if (currentQActionCoroutine != null)
            {
                StopCoroutine(currentQActionCoroutine);
            }

            currentQActionCoroutine = StartCoroutine(HandleQAction());
        }

        if (canUseE && Input.GetKeyDown(KeyCode.JoystickButton5) || canUseE && Input.GetKeyDown(KeyCode.E)) // joystickbutton5 for E
        {
            if (currentEActionCoroutine != null)
            {
                StopCoroutine(currentEActionCoroutine);
            }

            currentEActionCoroutine = StartCoroutine(HandleEAction());
        }
    }

    private void CheckForEnemiesWithRaycast()
    {
        // ����Raycast
        RaycastHit hitLeft;
        bool hitLeftEnemy = Physics.Raycast(raycastOriginLeft.position, transform.forward, out hitLeft, raycastDistance) &&
                            hitLeft.collider.CompareTag("enemy");

        // �E��Raycast
        RaycastHit hitRight;
        bool hitRightEnemy = Physics.Raycast(raycastOriginRight.position, transform.forward, out hitRight, raycastDistance) &&
                             hitRight.collider.CompareTag("enemy");

        // �ǂ��炩�ɓG�������Q�L�[��L����
        canUseQ = hitLeftEnemy || hitRightEnemy;
        canUseE = hitRightEnemy || hitRightEnemy;

        Debug.DrawRay(raycastOriginLeft.position, transform.forward * raycastDistance, Color.red);
        Debug.DrawRay(raycastOriginRight.position, transform.forward * raycastDistance, Color.red);

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
            float currentSpeed = isRotating ? moveSpeed * 0.5f : moveSpeed;
            Vector3 forwardDirection = transform.forward * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            if (!flag) return;

            if (isWalking)
            {
                float currentLeftRightSpeed = isRotating ? leftRightSpeed * 0.5f : leftRightSpeed;

                // Xbox�R���g���[���[��X-Axis���g�p���č��E�ړ�
                float horizontalInput = Input.GetAxis("Horizontal"); // X-Axis�̓��͒l���擾
                Vector3 leftRightDirection = transform.right * horizontalInput * currentLeftRightSpeed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + leftRightDirection);
            }
        }
    }

    private void HandleGuardInput()
    {
        if (canGuard && !isFalling)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton0)) // joystickbutton0 for guard
            {
                isGuarding = true;
                canMove = false;
            }
            else if (Input.GetKeyUp(KeyCode.JoystickButton0)) // joystickbutton0 release
            {
                isGuarding = false;
                canMove = true;
            }
        }

        if (canGuard && !isFalling) // �����Ă���Ԃ̓K�[�h�𖳌���
        {
            if (Input.GetKeyDown(KeyCode.F)) // F�L�[�������ꂽ�Ƃ�
            {
                isGuarding = true;
                canMove = false; // �ړ����֎~
            }
            else if (Input.GetKeyUp(KeyCode.F)) // F�L�[�������ꂽ�Ƃ�
            {
                isGuarding = false;
                canMove = true; // �ړ�������
            }
        }
    }


    private void UpdateAnimationState()
    {
        if (canGuard)
        {
            // �A�j���[�V�����̏�Ԃ����ɐ������ꍇ�͏������Ȃ�
            if (animator.GetBool("Guard") != isGuarding)
            {
                animator.SetBool("Guard", isGuarding);

                if (!isGuarding)
                {
                    animator.SetTrigger("Walk");
                }
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("enemy"))
        {
            if (isGuarding) return; // �K�[�h���͏������X�L�b�v

            Collider enemyCollider = collision.collider;

            // Q�L�[�܂���E�L�[�������̏ꍇ�A�G�t�F�N�g�ƃR���[�`�����m���Ɏ��s
            if (isUsingQ || isUsingE)
            {
                TriggerCollisionEffects();

                if (isUsingQ && currentQActionCoroutine != null)
                {
                    StopCoroutine(currentQActionCoroutine);
                    currentQActionCoroutine = null;
                    isUsingQ = false;
                }

                if (isUsingE && currentEActionCoroutine != null)
                {
                    StopCoroutine(currentEActionCoroutine);
                    currentEActionCoroutine = null;
                    isUsingE = false;
                }

                StartCoroutine(HandleFalling(enemyCollider));
                return; // ����ȏ�̏������X�L�b�v
            }

            // �ʏ�̏Փˏ���
            if (!isFalling)
            {
                TriggerCollisionEffects();
                StartCoroutine(HandleFalling(enemyCollider));
            }

            Debug.Log("Enemy�ɏՓ˂��܂����I");
        }
    }

    private void TriggerCollisionEffects()
    {
        StartCoroutine(CameraShake());

        // ParticleSystem�G�t�F�N�g����
        Vector3 spawnPosition = transform.position + new Vector3(0, 1.0f, 0);
        Quaternion spawnRotation = Quaternion.identity;

        if (impactParticlePrefab != null)
        {
            ParticleSystem particleInstance = Instantiate(impactParticlePrefab, spawnPosition, spawnRotation);
            float newScale = 8.0f;
            particleInstance.transform.localScale = new Vector3(newScale, newScale, newScale);

            Destroy(particleInstance.gameObject, particleDuration);
        }

        // �����_���Ō��ʉ����Đ�
        if (Random.Range(0f, 1f) < 0.2f)
        {
            audioSource.PlayOneShot(bone);
        }
        else
        {
            audioSource.PlayOneShot(slip);
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
        canGuard = true;

    }

    private IEnumerator HandleQAction()
    {
        if (isUsingQ) yield break; // �������̏ꍇ�̓X�L�b�v

        isUsingQ = true;
        canUseQ = false; // Q�L�[���ꎞ�I�ɖ�����
        bool originalCanMove = canMove; // ����canMove��Ԃ�ۑ�
        canMove = true; // �O���ړ��͈ێ�

        isWalking = false; // ���E�ړ�������
        isGuarding = false; // �K�[�h������

        float elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            Vector3 leftDirection = -transform.right * avoidSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + leftDirection);

            Vector3 forwardDirection = transform.forward * avoidMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            elapsedTime += Time.fixedDeltaTime;

            // �G�Ƃ̏Փ˂��m�F���A�����𒆒f
            if (isFalling)
            {
                isUsingQ = false;
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            Vector3 rightDirection = transform.right * avoidSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + rightDirection);

            Vector3 forwardDirection = transform.forward * avoidMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            elapsedTime += Time.fixedDeltaTime;

            if (isFalling)
            {
                isUsingQ = false;
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        isUsingQ = false;
        canMove = originalCanMove;
        isWalking = true;

        currentQActionCoroutine = null;

        // Q�L�[���ēx�L��������O��1�b�ԑҋ@
        yield return new WaitForSeconds(1.0f);
        canUseQ = true; // Q�L�[���ĂїL����
    }



    private IEnumerator HandleEAction()
    {
        if (isUsingE) yield break;

        isUsingE = true;
        bool originalCanMove = canMove;
        canMove = true;

        isWalking = false;
        isGuarding = false;

        float elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            Vector3 rightDirection = transform.right * avoidSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + rightDirection);

            Vector3 forwardDirection = transform.forward * avoidMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            elapsedTime += Time.fixedDeltaTime;

            if (isFalling)
            {
                isUsingE = false;
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            Vector3 leftDirection = -transform.right * avoidSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + leftDirection);

            Vector3 forwardDirection = transform.forward * avoidMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            elapsedTime += Time.fixedDeltaTime;

            if (isFalling)
            {
                isUsingE = false;
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        isUsingE = false;
        canMove = originalCanMove;
        isWalking = true;

        currentEActionCoroutine = null;
    }



    private IEnumerator HideOverlayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Color color = blackOverlay.color;
        color.a = 0f;
        blackOverlay.color = color;
        blackOverlay.gameObject.SetActive(false);
    }

    private IEnumerator HandleFalling(Collider enemyCollider)
    {
        // �Փ˂���enemy�̓����蔻��𖳌���
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        isWalking = false;
        canMove = false;
        isFalling = true;
        animator.SetTrigger("Falling");

        Vector3 reverseDirection = -transform.forward;
        float moveDuration = 1.6f;
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

        // enemy�̓����蔻����ĂїL����
        if (enemyCollider != null)
        {
            enemyCollider.enabled = true;
        }
    }


    private IEnumerator CameraShake()
    {
        // ���łɃV�F�C�N���Ȃ�V�����V�F�C�N�𖳎�
        if (isCameraShaking) yield break;

        isCameraShaking = true;
        initialCameraPosition = cameraTransform.localPosition;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            cameraTransform.localPosition = initialCameraPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // �V�F�C�N�I����A�J�����ʒu�𐳊m�ɏ����ʒu�ɖ߂�
        cameraTransform.localPosition = initialCameraPosition;
        isCameraShaking = false;
    }
}