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
    private bool isFalling = false;
    private bool isWalking = false;
    private bool isGuarding = false; // �K�[�h��Ԃ��Ǘ�
    private bool flag = true;
    private bool isRotating = false;
    private bool canGuard = false;

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
        // �K�[�h��������ɌĂяo��
        HandleGuardInput();

        // ��Ԃɂ���Ĉړ����͂𖳌���
        if (!canMove || isFalling || isGuarding || !isWalking)
        {
            return;
        }

        // ���̓��͏����i�ړ��AQ/E�Ȃǁj
        movementInputValue = Input.GetAxis("Horizontal");
        Vector3 movement = transform.right * movementInputValue * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        if (canUseQ && (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown("joystick button 4")))
        {
            if (currentQActionCoroutine != null)
            {
                StopCoroutine(currentQActionCoroutine);
            }
            currentQActionCoroutine = StartCoroutine(HandleQAction());
        }

        if (canUseE && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown("joystick button 5")))
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
        canUseE = hitLeftEnemy || hitRightEnemy;

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
        // �d�͂�K�p
        rb.AddForce(new Vector3(0f, gravity, 0f));

        if (!canMove || isFalling || isGuarding || !isWalking) // ��Ԃ��m�F���Ĉړ��𖳌���
        {
            return;
        }

        float currentSpeed = isRotating ? moveSpeed * 0.5f : moveSpeed; // ��]���͑��x�𔼕���
        Vector3 forwardDirection = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardDirection);

        float currentLeftRightSpeed = isRotating ? leftRightSpeed * 0.5f : leftRightSpeed;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") < 0)
        {
            Vector3 leftDirection = -transform.right * currentLeftRightSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + leftDirection);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("Horizontal") > 0)
        {
            Vector3 rightDirection = transform.right * currentLeftRightSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + rightDirection);
        }
    }


    private void HandleGuardInput()
    {
        // �K�[�h�\���]�|���Ă��Ȃ��ꍇ�ɂ̂ݏ���
        if (canGuard && !isFalling)
        {
            // �K�[�h�J�n
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown("joystick button 0"))
            {
                isGuarding = true;
                canMove = false; // �ړ����֎~
                animator.SetBool("Guard", true); // �K�[�h�A�j���[�V�������J�n
            }
            // �K�[�h����
            else if (Input.GetKeyUp(KeyCode.F) || Input.GetKeyUp("joystick button 0"))
            {
                isGuarding = false;
                canMove = true; // �ړ�������
                animator.SetBool("Guard", false); // �K�[�h�A�j���[�V�������I��
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
            if (isGuarding) return; // Skip if guarding

            // Q�L�[�������̏ꍇ�A�����I�ɃR���[�`�����~���ăm�b�N�o�b�N�����Ɉڍs
            if (isUsingQ)
            {
                // Q�L�[�������Ȃ炻�̃R���[�`�����~
                if (currentQActionCoroutine != null)
                {
                    StopCoroutine(currentQActionCoroutine);
                    currentQActionCoroutine = null; // �R���[�`���Q�Ƃ����Z�b�g
                }

                // �m�b�N�o�b�N�����Ɉڍs
                StartCoroutine(HandleFalling());
                return; // ����ȏ�̏������X�L�b�v
            }

            if (isUsingE)
            {
                // Q�L�[�������Ȃ炻�̃R���[�`�����~
                if (currentEActionCoroutine != null)
                {
                    StopCoroutine(currentEActionCoroutine);
                    currentEActionCoroutine = null; // �R���[�`���Q�Ƃ����Z�b�g
                }

                // �m�b�N�o�b�N�����Ɉڍs
                StartCoroutine(HandleFalling());
                return; // ����ȏ�̏������X�L�b�v
            }

            // �ʏ�̏Փˏ���
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

            Debug.Log("Enemy�ɏՓ˂��܂����I");
        }
    }


    private IEnumerator IdleCoroutine()
    {
        canMove = false;
        isWalking = false;
        canGuard = false; // �K�[�h�𖳌���
        animator.SetBool("Idle", true);

        yield return new WaitForSeconds(3.3f);

        animator.SetBool("Idle", false);
        animator.SetTrigger("Walk");
        canMove = true;
        isWalking = true;
        canGuard = true; // �K�[�h���ĂїL����
    }



    private IEnumerator HandleQAction()
    {
        if (isUsingQ) yield break; // �������̏ꍇ�̓X�L�b�v

        isUsingQ = true;
        bool originalCanMove = canMove; // ����canMove��Ԃ�ۑ�
        canMove = true; // �O���ړ��͈ێ�

        isWalking = false; // ���E�ړ�������
        isGuarding = false; // �K�[�h������

        // ���Ɉړ�
        float elapsedTime = 0f;
        while (elapsedTime < 0.5f) // 1�b�ԍ��ړ�
        {
            Vector3 leftDirection = -transform.right * avoidSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + leftDirection);

            // �O���ړ����ێ�
            Vector3 forwardDirection = transform.forward * avoidMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // �E�ɖ߂�
        elapsedTime = 0f;
        while (elapsedTime < 0.5f) // 1�b�ԉE�ړ�
        {
            Vector3 rightDirection = transform.right * avoidSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + rightDirection);

            // �O���ړ����ێ�
            Vector3 forwardDirection = transform.forward * avoidMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // ��Ԃ����ɖ߂�
        isUsingQ = false;
        canMove = originalCanMove; // ����canMove��Ԃ𕜌�
        isWalking = true; // ���E�ړ����ĂїL����

        // �R���[�`�����I����������null�Ƀ��Z�b�g
        currentQActionCoroutine = null;
    }

    private IEnumerator HandleEAction()
    {
        if (isUsingE) yield break; // �������̏ꍇ�̓X�L�b�v

        isUsingE = true;
        bool originalCanMove = canMove; // ����canMove��Ԃ�ۑ�
        canMove = true; // �O���ړ��͈ێ�

        isWalking = false; // ���E�ړ�������
        isGuarding = false; // �K�[�h������

        // ���Ɉړ�
        float elapsedTime = 0f;
        while (elapsedTime < 0.5f) // 1�b�ԍ��ړ�
        {
            Vector3 rightDirection = transform.right * avoidSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + rightDirection);

            // �O���ړ����ێ�
            Vector3 forwardDirection = transform.forward * avoidMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // �E�ɖ߂�
        elapsedTime = 0f;
        while (elapsedTime < 0.5f) // 1�b�ԉE�ړ�
        {
            Vector3 rightDirection = -transform.right * avoidSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + rightDirection);

            // �O���ړ����ێ�
            Vector3 forwardDirection = transform.forward * avoidMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // ��Ԃ����ɖ߂�
        isUsingE = false;
        canMove = originalCanMove; // ����canMove��Ԃ𕜌�
        isWalking = true; // ���E�ړ����ĂїL����

        // �R���[�`�����I����������null�Ƀ��Z�b�g
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

    private IEnumerator HandleFalling()
    {
        var gamepad = Gamepad.current;

        isWalking = false;
        canMove = false;
        isFalling = true;
        animator.SetTrigger("Falling");
        gamepad.SetMotorSpeeds(0.0f, 1.0f);
        yield return new WaitForSeconds(0.3f);
        gamepad.SetMotorSpeeds(0.0f, 0.0f);
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

        // �]�|��̏�Ԃ����Z�b�g
        isUsingQ = false;
        isUsingE = false;

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