using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public Transform Target;
    public GameObject[] obstacles;
    public GameObject[] obstaclesList2; // リスト2として使用する障害物のプレハブ配列
    public List<GameObject> ObstacleList = new List<GameObject>();

    public float minObstacleInterval = 1f;
    public float maxObstacleInterval = 3f;
    private float nextObstacleTime;

    public float obstacleDistance = 40f;
    public float sampleRadius = 5f;
    public float spawnAngleRange = 30f;

    private float timer = 0f;

    void Start()
    {
        ScheduleNextObstacle();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (Time.time >= nextObstacleTime)
        {
            GenerateObstacleOnNavMesh();
            ScheduleNextObstacle();
        }

        RemovePassedObstacles();
    }

    void GenerateObstacleOnNavMesh()
    {
        float yPos = Target.position.y;
        Vector3 spawnPosition = Target.position + Target.forward * obstacleDistance;

        // 各範囲でそれぞれの障害物リストから生成
        if (yPos >= 0)
        {
            // y >= 0 の場合の生成
            float xPos1 = Random.Range(3f, -1f);
            float xPos2 = Random.Range(-6f, -2f);
            GenerateFromList(new Vector3(xPos1, spawnPosition.y, spawnPosition.z), obstacles);
            GenerateFromList(new Vector3(xPos2, spawnPosition.y, spawnPosition.z), obstaclesList2);
        }
        if (yPos >= -6f)
        {
            // y >= -6 の場合の生成
            float zPos1 = Random.Range(-109f, -106f);
            float zPos2 = Random.Range(-110f, -113f);
            GenerateFromList(new Vector3(spawnPosition.x, spawnPosition.y, zPos1), obstacles);
            GenerateFromList(new Vector3(spawnPosition.x, spawnPosition.y, zPos2), obstaclesList2);
        }
        if (yPos >= -10f)
        {
            // y >= -10 の場合の生成
            float xPos1 = Random.Range(103.3f, 106.3f);
            float xPos2 = Random.Range(107.3f, 110.3f);
            GenerateFromList(new Vector3(xPos1, spawnPosition.y, spawnPosition.z), obstacles);
            GenerateFromList(new Vector3(xPos2, spawnPosition.y, spawnPosition.z), obstaclesList2);
        }
        if (yPos < -10f)
        {
            // y < -10 の場合の生成
            float zPos1 = Random.Range(-20f, -17f);
            float zPos2 = Random.Range(-13f, -16f);
            GenerateFromList(new Vector3(spawnPosition.x, spawnPosition.y, zPos1), obstacles);
            GenerateFromList(new Vector3(spawnPosition.x, spawnPosition.y, zPos2), obstaclesList2);
        }
    }

    void GenerateFromList(Vector3 spawnPosition, GameObject[] selectedObstacles)
    {
        // プレイヤーから十分な距離を保つチェック
        if (Vector3.Distance(spawnPosition, Target.position) < obstacleDistance) return;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, sampleRadius, NavMesh.AllAreas))
        {
            Vector3 validPosition = hit.position;
            validPosition.y = Target.position.y;

            int obstacleIndex = Random.Range(0, selectedObstacles.Length);
            Quaternion spawnRotation = selectedObstacles[obstacleIndex].transform.rotation;
            GameObject obstacle = Instantiate(selectedObstacles[obstacleIndex], validPosition, spawnRotation);

            ObstacleList.Add(obstacle);
        }
    }

    void ScheduleNextObstacle()
    {
        nextObstacleTime = Time.time + Random.Range(minObstacleInterval, maxObstacleInterval);
    }

    void RemovePassedObstacles()
    {
        Vector3 playerBackward = -Target.forward;

        for (int i = ObstacleList.Count - 1; i >= 0; i--)
        {
            Vector3 obstacleDirection = ObstacleList[i].transform.position - Target.position;
            float dotProduct = Vector3.Dot(obstacleDirection.normalized, playerBackward);

            if (dotProduct > 0)
            {
                GameObject passedObstacle = ObstacleList[i];
                ObstacleList.RemoveAt(i);
                StartCoroutine(WaitAndDestroy(passedObstacle));
            }
        }
    }

    IEnumerator WaitAndDestroy(GameObject obstacle)
    {
        yield return new WaitForSeconds(1f);
        Destroy(obstacle);
    }
}
