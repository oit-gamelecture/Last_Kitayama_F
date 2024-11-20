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
    private bool flag = true;

    public Image blackOverlay;
    public float delayBeforeHiding = 3.3f;

    [Header("ダメージエフェクト")]
    public SkinnedMeshRenderer playerRenderer; // SkinnedMeshRendererに変更
    public Color blinkColor = Color.red; // 点滅時の色
    public float blinkDuration = 0.2f; // 点滅間隔
    public int blinkCount = 2; // 点滅回数


    [Header("画面揺れ")]
    public Transform cameraTransform; // カメラのTransform
    public float shakeDuration = 1f; // 揺れの時間
    public float shakeMagnitude = 0.1f; // 揺れの強さ

    [Header("パーティクル設定")]
    public ParticleSystem impactParticlePrefab; // パーティクルのプレハブ
    public float particleDuration = 2f;        // パーティクルを自動で消すまでの時間

    private Material[] originalMaterials; // プレイヤーの元のマテリアル


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

        if (playerRenderer != null)
        {
            originalMaterials = playerRenderer.materials; // 元のマテリアルを保存
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(0f, gravity, 0f));

        if (canMove)
        {
            // 前方移動
            Vector3 forwardDirection = transform.forward * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardDirection);

            if (!flag) return;

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

                movementInputValue = Input.GetAxis("Horizontal");
                Vector3 movement = transform.right * movementInputValue * leftRightSpeed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + movement);
            }
        }
    }

    public void SetMovement(bool enabled)
    {
        flag = enabled;
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
            // 赤く点滅と画面揺れをトリガー
            StartCoroutine(BlinkEffect());
            StartCoroutine(CameraShake());
        }

        if (collision.collider.CompareTag("enemy"))
        {
            // 衝突位置と方向を取得
            ContactPoint contact = collision.contacts[0];
            Vector3 spawnPosition = transform.position; // 衝突位置
            Quaternion spawnRotation = Quaternion.identity; ; // 衝突方向

            // パーティクルを生成
            if (impactParticlePrefab != null)
            {
                spawnPosition += new Vector3(0, 1.0f, 0); // 必要に応じてオフセット (例: Y方向に1.0f上)

                // パーティクルを生成
                if (impactParticlePrefab != null)
                {
                    ParticleSystem particleInstance = Instantiate(impactParticlePrefab, spawnPosition, spawnRotation);

                    float newScale = 8.0f; // 好きなスケールに設定
                    particleInstance.transform.localScale = new Vector3(newScale, newScale, newScale);

                    // 一定時間後に削除
                    Destroy(particleInstance.gameObject, particleDuration);
                }
            }

            // 他のエフェクトや動作もここで実行可能
            Debug.Log("Enemyに衝突しました！");
        }
    }

    private IEnumerator BlinkEffect()
    {
        // インスタンス化されたマテリアルを取得
        Material[] materials = playerRenderer.materials;

        // 各マテリアルの元の色を保存
        Color[] originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }

        bool resetPending = true; // リセットフラグを設定

        // 点滅ループ
        for (int i = 0; i < blinkCount; i++)
        {
            // すべてのマテリアルを赤にする
            foreach (Material mat in materials)
            {
                mat.color = blinkColor;
            }
            yield return new WaitForSeconds(blinkDuration);

            // すべてのマテリアルを元の色に戻す
            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].color = originalColors[j];
            }
            yield return new WaitForSeconds(blinkDuration);
        }

        // 1秒後に元の色に戻す処理を保証
        StartCoroutine(EnsureOriginalColor(materials, originalColors, 1f));
    }

    private IEnumerator EnsureOriginalColor(Material[] materials, Color[] originalColors, float delay)
    {
        // 指定時間待機
        yield return new WaitForSeconds(delay);

        // プレイヤーのマテリアルを確認し、赤のままなら元の色に戻す
        foreach (Material mat in materials)
        {
            if (mat.color == blinkColor) // 赤のままの場合
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].color = originalColors[i]; // 元の色に戻す
                }
                break;
            }
        }
    }



    private IEnumerator CameraShake()
    {
        float elapsed = 0f;

        Vector3 initialPosition = cameraTransform.localPosition; // ローカル座標を基準にする

        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;

            // カメラ位置を更新
            cameraTransform.localPosition = initialPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.localPosition = initialPosition;
    }

    private IEnumerator IdleCoroutine()
    {
        // Idleアニメーションの開始
        animator.SetBool("Idle", true);
        yield return new WaitForSeconds(3.3f);

        // Idleアニメーションを停止し、Walkアニメーションを開始
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