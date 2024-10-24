using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Transform Target; // プレイヤーの参照
    public GameObject[] obstacles; // 障害物のプレハブ
    public List<GameObject> ObstacleList = new List<GameObject>(); // 障害物リスト

    public float minObstacleInterval = 1f; // 障害物生成の最小間隔
    public float maxObstacleInterval = 3f; // 障害物生成の最大間隔
    private float nextObstacleTime; // 次の障害物生成時間

    public float obstacleDistance = 40f; // プレイヤーから障害物を生成する距離
    private float minX = -0.2f; // X座標の最小値
    private float maxX = 3.0f;  // X座標の最大値

    private float timer = 0f; // タイマー

    // Startは最初に呼ばれる
    void Start()
    {
        ScheduleNextObstacle(); // 初回の障害物生成タイミングを設定
    }

    // Updateは毎フレーム呼ばれる
    void Update()
    {
        timer += Time.deltaTime; // 経過時間を更新

        // 障害物を生成するタイミングに達したら生成
        if (Time.time >= nextObstacleTime)
        {
            GenerateObstacle();
            ScheduleNextObstacle(); // 次の生成タイミングを設定
        }

        // 通り過ぎた障害物を削除
        RemovePassedObstacles();
    }

    // 障害物を生成する
    void GenerateObstacle()
    {
        float xPos = Random.Range(minX, maxX); // X座標をランダムに決定
        int obstacleIndex = Random.Range(0, obstacles.Length); // プレハブをランダム選択

        // プレイヤーの進行方向に合わせて、Z軸の負方向に生成する
        Vector3 spawnPosition = new Vector3(xPos, 0.5f, Target.position.z - obstacleDistance);

        // Z座標が 100 以上なら生成しない
        if (spawnPosition.z <= -100f)
        {
            Debug.Log("壁に生成しようとしたよ");
            return;
        }

        // プレハブの回転を取得して生成
        Quaternion spawnRotation = obstacles[obstacleIndex].transform.rotation;
        GameObject obstacle = Instantiate(obstacles[obstacleIndex], spawnPosition, spawnRotation);

        ObstacleList.Add(obstacle); // 障害物をリストに追加
    }

    // 次の障害物生成タイミングを設定
    void ScheduleNextObstacle()
    {
        nextObstacleTime = Time.time + Random.Range(minObstacleInterval, maxObstacleInterval);
    }

    // プレイヤーが通り過ぎた障害物を削除
    void RemovePassedObstacles()
    {
        for (int i = ObstacleList.Count - 1; i >= 0; i--)
        {
            // プレイヤーが障害物を通り過ぎたか判定（Z軸の進行方向に基づく）
            if (ObstacleList[i].transform.position.z > Target.position.z + 10f)
            {
                GameObject passedObstacle = ObstacleList[i];
                ObstacleList.RemoveAt(i);
                Destroy(passedObstacle); // 障害物を削除
            }
        }
    }
}
