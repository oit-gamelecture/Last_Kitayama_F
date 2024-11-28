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
    private bool isFalling = false;
    private bool isWalking = false;
    private bool isGuarding = false; // ガード状態を管理
    private bool flag = true;
    private bool isRotating = false;
    private bool canGuard = false;

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
        // ガード処理を常に呼び出す
        HandleGuardInput();

        // 状態によって移動入力を無効化
        if (!canMove || isFalling || isGuarding || !isWalking)
        {
            return;
        }

        // 他の入力処理（移動、Q/Eなど）
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
        // 重力を適用
        rb.AddForce(new Vector3(0f, gravity, 0f));

        if (!canMove || isFalling || isGuarding || !isWalking) // 状態を確認して移動を無効化
        {
            return;
        }

        float currentSpeed = isRotating ? moveSpeed * 0.5f : moveSpeed; // 回転中は速度を半分に
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
        // ガード可能かつ転倒していない場合にのみ処理
        if (canGuard && !isFalling)
        {
            // ガード開始
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown("joystick button 0"))
            {
                isGuarding = true;
                canMove = false; // 移動を禁止
                animator.SetBool("Guard", true); // ガードアニメーションを開始
            }
            // ガード解除
            else if (Input.GetKeyUp(KeyCode.F) || Input.GetKeyUp("joystick button 0"))
            {
                isGuarding = false;
                canMove = true; // 移動を許可
                animator.SetBool("Guard", false); // ガードアニメーションを終了
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
            if (isGuarding) return; // Skip if guarding

            // Qキー処理中の場合、強制的にコルーチンを停止してノックバック処理に移行
            if (isUsingQ)
            {
                // Qキー処理中ならそのコルーチンを停止
                if (currentQActionCoroutine != null)
                {
                    StopCoroutine(currentQActionCoroutine);
                    currentQActionCoroutine = null; // コルーチン参照をリセット
                }

                // ノックバック処理に移行
                StartCoroutine(HandleFalling());
                return; // それ以上の処理をスキップ
            }

            if (isUsingE)
            {
                // Qキー処理中ならそのコルーチンを停止
                if (currentEActionCoroutine != null)
                {
                    StopCoroutine(currentEActionCoroutine);
                    currentEActionCoroutine = null; // コルーチン参照をリセット
                }

                // ノックバック処理に移行
                StartCoroutine(HandleFalling());
                return; // それ以上の処理をスキップ
            }

            // 通常の衝突処理
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
        canMove = false;
        isWalking = false;
        canGuard = false; // ガードを無効化
        animator.SetBool("Idle", true);

        yield return new WaitForSeconds(3.3f);

        animator.SetBool("Idle", false);
        animator.SetTrigger("Walk");
        canMove = true;
        isWalking = true;
        canGuard = true; // ガードを再び有効化
    }



    private IEnumerator HandleQAction()
    {
        if (isUsingQ) yield break; // 処理中の場合はスキップ

        isUsingQ = true;
        bool originalCanMove = canMove; // 元のcanMove状態を保存
        canMove = true; // 前方移動は維持

        isWalking = false; // 左右移動無効化
        isGuarding = false; // ガード無効化

        // 左に移動
        float elapsedTime = 0f;
        while (elapsedTime < 0.5f) // 1秒間左移動
        {
            Vector3 leftDirection = -transform.right * avoidSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + leftDirection);

            // 前方移動を維持
            Vector3 forwardDirection = transform.forward * avoidMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 右に戻る
        elapsedTime = 0f;
        while (elapsedTime < 0.5f) // 1秒間右移動
        {
            Vector3 rightDirection = transform.right * avoidSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + rightDirection);

            // 前方移動を維持
            Vector3 forwardDirection = transform.forward * avoidMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 状態を元に戻す
        isUsingQ = false;
        canMove = originalCanMove; // 元のcanMove状態を復元
        isWalking = true; // 左右移動を再び有効化

        // コルーチンが終了した時にnullにリセット
        currentQActionCoroutine = null;
    }

    private IEnumerator HandleEAction()
    {
        if (isUsingE) yield break; // 処理中の場合はスキップ

        isUsingE = true;
        bool originalCanMove = canMove; // 元のcanMove状態を保存
        canMove = true; // 前方移動は維持

        isWalking = false; // 左右移動無効化
        isGuarding = false; // ガード無効化

        // 左に移動
        float elapsedTime = 0f;
        while (elapsedTime < 0.5f) // 1秒間左移動
        {
            Vector3 rightDirection = transform.right * avoidSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + rightDirection);

            // 前方移動を維持
            Vector3 forwardDirection = transform.forward * avoidMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 右に戻る
        elapsedTime = 0f;
        while (elapsedTime < 0.5f) // 1秒間右移動
        {
            Vector3 rightDirection = -transform.right * avoidSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + rightDirection);

            // 前方移動を維持
            Vector3 forwardDirection = transform.forward * avoidMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 状態を元に戻す
        isUsingE = false;
        canMove = originalCanMove; // 元のcanMove状態を復元
        isWalking = true; // 左右移動を再び有効化

        // コルーチンが終了した時にnullにリセット
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

        // 転倒後の状態をリセット
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