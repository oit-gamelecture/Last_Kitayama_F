using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("プレイヤー設定")]
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
    public bool isGuarding = false; // ガード状態を管理
    private bool flag = true;
    private bool isRotating = false;
    private bool canGuard = false;
    private bool isCameraShaking = false; // カメラシェイクの状態を追跡
    private Vector3 initialCameraPosition; // カメラの初期位置を記録


    public Image blackOverlay;
    public float delayBeforeHiding = 3.3f;

    [Header("画面揺れ")]
    public Transform cameraTransform;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 0.1f;

    [Header("パーティクル設定")]
    public ParticleSystem impactParticlePrefab;
    public float particleDuration = 2f;

    private bool canUseQ = false; // Qキーが使用可能かどうか
    private bool isUsingQ = false; // Qキー処理中かどうか
    private Coroutine currentQActionCoroutine;
    private bool canUseE = false;
    private bool isUsingE = false;
    private Coroutine currentEActionCoroutine;

    [Header("Raycast 設定")]
    public float raycastDistance = 5.0f; // Rayの距離
    public Transform raycastOriginLeft; // 左のRay発射位置
    public Transform raycastOriginRight; // 右のRay発射位置

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
        // 左のRaycast
        RaycastHit hitLeft;
        bool hitLeftEnemy = Physics.Raycast(raycastOriginLeft.position, transform.forward, out hitLeft, raycastDistance) &&
                            hitLeft.collider.CompareTag("enemy");

        // 右のRaycast
        RaycastHit hitRight;
        bool hitRightEnemy = Physics.Raycast(raycastOriginRight.position, transform.forward, out hitRight, raycastDistance) &&
                             hitRight.collider.CompareTag("enemy");

        // どちらかに敵がいればQキーを有効化
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

                // XboxコントローラーのX-Axisを使用して左右移動
                float horizontalInput = Input.GetAxis("Horizontal"); // X-Axisの入力値を取得
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

        if (canGuard && !isFalling) // こけている間はガードを無効化
        {
            if (Input.GetKeyDown(KeyCode.F)) // Fキーが押されたとき
            {
                isGuarding = true;
                canMove = false; // 移動を禁止
            }
            else if (Input.GetKeyUp(KeyCode.F)) // Fキーが離されたとき
            {
                isGuarding = false;
                canMove = true; // 移動を許可
            }
        }
    }


    private void UpdateAnimationState()
    {
        if (canGuard)
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
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("enemy"))
        {
            if (isGuarding) return; // ガード中は処理をスキップ

            Collider enemyCollider = collision.collider;

            // QキーまたはEキー処理中の場合、エフェクトとコルーチンを確実に実行
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
                return; // それ以上の処理をスキップ
            }

            // 通常の衝突処理
            if (!isFalling)
            {
                TriggerCollisionEffects();
                StartCoroutine(HandleFalling(enemyCollider));
            }

            Debug.Log("Enemyに衝突しました！");
        }
    }

    private void TriggerCollisionEffects()
    {
        StartCoroutine(CameraShake());

        // ParticleSystemエフェクト生成
        Vector3 spawnPosition = transform.position + new Vector3(0, 1.0f, 0);
        Quaternion spawnRotation = Quaternion.identity;

        if (impactParticlePrefab != null)
        {
            ParticleSystem particleInstance = Instantiate(impactParticlePrefab, spawnPosition, spawnRotation);
            float newScale = 8.0f;
            particleInstance.transform.localScale = new Vector3(newScale, newScale, newScale);

            Destroy(particleInstance.gameObject, particleDuration);
        }

        // ランダムで効果音を再生
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
        if (isUsingQ) yield break; // 処理中の場合はスキップ

        isUsingQ = true;
        canUseQ = false; // Qキーを一時的に無効化
        bool originalCanMove = canMove; // 元のcanMove状態を保存
        canMove = true; // 前方移動は維持

        isWalking = false; // 左右移動無効化
        isGuarding = false; // ガード無効化

        float elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            Vector3 leftDirection = -transform.right * avoidSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + leftDirection);

            Vector3 forwardDirection = transform.forward * avoidMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            elapsedTime += Time.fixedDeltaTime;

            // 敵との衝突を確認し、処理を中断
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

        // Qキーを再度有効化する前に1秒間待機
        yield return new WaitForSeconds(1.0f);
        canUseQ = true; // Qキーを再び有効化
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
        // 衝突したenemyの当たり判定を無効化
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

        // enemyの当たり判定を再び有効化
        if (enemyCollider != null)
        {
            enemyCollider.enabled = true;
        }
    }


    private IEnumerator CameraShake()
    {
        // すでにシェイク中なら新しいシェイクを無視
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

        // シェイク終了後、カメラ位置を正確に初期位置に戻す
        cameraTransform.localPosition = initialCameraPosition;
        isCameraShaking = false;
    }
}