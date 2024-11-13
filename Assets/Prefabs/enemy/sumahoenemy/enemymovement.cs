using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemymovement : MonoBehaviour
{
    public Animator enemyAnimator;
    public float normalSpeed = 3.0f;
    public float retreatSpeed = 6.0f;
    private bool isFalling = false;

    private NavMeshAgent navMeshAgent;
    private Transform currentTarget;

    [Header("Targets for each height range")]
    private Transform[] targetsLevel1 = new Transform[2]; // 高さ0以上のターゲット
    private Transform[] targetsLevel2 = new Transform[2]; // 高さ -6 ～ 0
    private Transform[] targetsLevel3 = new Transform[2]; // 高さ -11 ～ -6
    private Transform[] targetsLevel4 = new Transform[2]; // 高さ -11以下


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError($"NavMeshAgentが {gameObject.name} に見つかりません。");
            return;
        }

        enemyAnimator = GetComponent<Animator>();
        navMeshAgent.speed = normalSpeed;

        EnsureOnNavMesh();
        InitializeTargets();
        SetTargetsBasedOnHeight(); // 高さに応じた目標地点を設定
        navMeshAgent.SetDestination(SetRandomizedTargetPosition());
    }

    void Update()
    {
        if (!isFalling && navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            if (navMeshAgent.remainingDistance < 0.5f)
            {
                ToggleTargetPosition();
                navMeshAgent.SetDestination(currentTarget.position);
            }
        }
    }

    void InitializeTargets()
    {
        // シーン内のオブジェクトを名前で検索し、ターゲット配列に格納
        targetsLevel1[0] = GameObject.Find("Target1_1").transform;
        targetsLevel1[1] = GameObject.Find("Target1_2").transform;
        targetsLevel2[0] = GameObject.Find("Target2_1").transform;
        targetsLevel2[1] = GameObject.Find("Target2_2").transform;
        targetsLevel3[0] = GameObject.Find("Target3_1").transform;
        targetsLevel3[1] = GameObject.Find("Target3_2").transform;
        targetsLevel4[0] = GameObject.Find("Target4_1").transform;
        targetsLevel4[1] = GameObject.Find("Target4_2").transform;
    }

    void SetTargetsBasedOnHeight()
    {
        float yPosition = transform.position.y;

        if (yPosition >= 0)
        {
            currentTarget = Random.value < 0.5f ? targetsLevel1[0] : targetsLevel1[1];
        }
        else if (yPosition >= -6 && yPosition < 0)
        {
            currentTarget = Random.value < 0.5f ? targetsLevel2[0] : targetsLevel2[1];
        }
        else if (yPosition >= -11 && yPosition < -6)
        {
            currentTarget = Random.value < 0.5f ? targetsLevel3[0] : targetsLevel3[1];
        }
        else
        {
            currentTarget = Random.value < 0.5f ? targetsLevel4[0] : targetsLevel4[1];
        }
    }

    Vector3 SetRandomizedTargetPosition()
    {
        Vector3 randomizedPosition = currentTarget.position;

        if (System.Array.IndexOf(targetsLevel1, currentTarget) >= 0)
        {
            randomizedPosition.x = Random.Range(3f, -6f);
        }
        else if (System.Array.IndexOf(targetsLevel2, currentTarget) >= 0)
        {
            randomizedPosition.z = Random.Range(-106f, -113f);
        }
        else if (System.Array.IndexOf(targetsLevel3, currentTarget) >= 0)
        {
            randomizedPosition.x = Random.Range(103.3f, 110f);
        }
        else if (System.Array.IndexOf(targetsLevel4, currentTarget) >= 0)
        {
            randomizedPosition.z = Random.Range(-13f, -20f);
        }

        return randomizedPosition;
    }


    void ToggleTargetPosition()
    {
        if (System.Array.IndexOf(targetsLevel1, currentTarget) >= 0)
        {
            currentTarget = currentTarget == targetsLevel1[0] ? targetsLevel1[1] : targetsLevel1[0];
        }
        else if (System.Array.IndexOf(targetsLevel2, currentTarget) >= 0)
        {
            currentTarget = currentTarget == targetsLevel2[0] ? targetsLevel2[1] : targetsLevel2[0];
        }
        else if (System.Array.IndexOf(targetsLevel3, currentTarget) >= 0)
        {
            currentTarget = currentTarget == targetsLevel3[0] ? targetsLevel3[1] : targetsLevel3[0];
        }
        else if (System.Array.IndexOf(targetsLevel4, currentTarget) >= 0)
        {
            currentTarget = currentTarget == targetsLevel4[0] ? targetsLevel4[1] : targetsLevel4[0];
        }
    }



    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            isFalling = true;
            StartCoroutine(HandleFall(collision.transform));
        }
    }

    IEnumerator HandleFall(Transform playerTransform)
    {
        enemyAnimator.SetBool("IsWalking", false);
        enemyAnimator.SetTrigger("Fall");
        navMeshAgent.isStopped = true;

        Vector3 retreatDirection = (transform.position - playerTransform.position).normalized;
        float elapsedTime = 0f;
        float retreatDuration = 0.5f;

        while (elapsedTime < retreatDuration)
        {
            transform.Translate(retreatDirection * retreatSpeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
    }

    void EnsureOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
    }
}
