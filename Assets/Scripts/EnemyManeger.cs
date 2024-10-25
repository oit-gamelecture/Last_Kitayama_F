using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMeshを使用するための名前空間

public class EnemyManager : MonoBehaviour
{
    public Transform Target; // プレイヤーの参照
    public GameObject[] obstacles; // 障害物のプレハブ
    public List<GameObject> ObstacleList = new List<GameObject>(); // 障害物リスト

    public float minObstacleInterval = 1f; // 障害物生成の最小間隔
    public float maxObstacleInterval = 3f; // 障害物生成の最大間隔
    private float nextObstacleTime; // 次の障害物生成時間

    public float obstacleDistance = 40f; // プレイヤーから障害物を生成する距離
    public float sampleRadius = 5f; // NavMeshサンプリングの半径
    public float spawnAngleRange = 30f; // 生成する方向の角度範囲（度数法）

    private float timer = 0f; // タイマー

    void Start()
    {
        ScheduleNextObstacle(); // 初回の障害物生成タイミングを設定
    }

    void Update()
    {
        timer += Time.deltaTime; // 経過時間を更新

        // 障害物を生成するタイミングに達したら生成
        if (Time.time >= nextObstacleTime)
        {
            GenerateObstacleOnNavMesh(); // NavMesh上に障害物を生成
            ScheduleNextObstacle(); // 次の生成タイミングを設定
        }

        // 通り過ぎた障害物を削除
        RemovePassedObstacles();
    }

    // NavMesh上に障害物を生成する
    void GenerateObstacleOnNavMesh()
    {
        // ランダムな角度で方向を決定
        float randomAngle = Random.Range(-spawnAngleRange, spawnAngleRange);
        Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * Target.forward;

        // プレイヤーの前方でランダムな方向に生成位置を計算
        Vector3 spawnPosition = Target.position + randomDirection.normalized * obstacleDistance;

        // NavMesh上の有効な位置を取得
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, sampleRadius, NavMesh.AllAreas))
        {
            // 有効な位置が見つかった場合、障害物を生成する
            int obstacleIndex = Random.Range(0, obstacles.Length);
            Quaternion spawnRotation = obstacles[obstacleIndex].transform.rotation;
            GameObject obstacle = Instantiate(obstacles[obstacleIndex], hit.position, spawnRotation);

            ObstacleList.Add(obstacle); // 障害物をリストに追加
        }
        else
        {
            //Debug.Log("NavMesh上に適切な生成位置が見つかりませんでした");
        }
    }

    // 次の障害物生成タイミングを設定
    void ScheduleNextObstacle()
    {
        nextObstacleTime = Time.time + Random.Range(minObstacleInterval, maxObstacleInterval);
    }

    // 通り過ぎた障害物を削除
    void RemovePassedObstacles()
    {
        for (int i = ObstacleList.Count - 1; i >= 0; i--)
        {
            if (ObstacleList[i].transform.position.z > Target.position.z + 10f)
            {
                GameObject passedObstacle = ObstacleList[i];
                ObstacleList.RemoveAt(i);
                Destroy(passedObstacle); // 障害物を削除
            }
        }
    }
}
