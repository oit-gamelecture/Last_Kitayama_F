using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public Transform Target;
    public GameObject[] obstacles;
    public GameObject[] obstaclesList2; // ���X�g2�Ƃ��Ďg�p�����Q���̃v���n�u�z��
    public List<GameObject> ObstacleList = new List<GameObject>();

    public float minObstacleInterval = 1.5f;
    public float maxObstacleInterval = 2f;
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

        IncreaseEnemies();

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
        float zPos = Target.position.z;
        Vector3 spawnPosition = Target.position + Target.forward * obstacleDistance;

        // �e�͈͂ł��ꂼ��̏�Q�����X�g���琶��
        if (yPos >= 0)
        {
            // y >= 0 �̏ꍇ�̐���
            float xPos1 = Random.Range(3f, -1f);
            float xPos2 = Random.Range(-6f, -2f);
            // Z���W�� -109.3 �ȉ��̎��������Ȃ�
            if (spawnPosition.z < -109.3f) return;
            GenerateFromList(new Vector3(xPos1, spawnPosition.y, spawnPosition.z), obstacles);
            GenerateFromList(new Vector3(xPos2, spawnPosition.y, spawnPosition.z), obstaclesList2);
        }
        if (yPos >= -6f)
        {
            // y >= -6 �̏ꍇ�̐���
            float zPos1 = Random.Range(-109f, -106f);
            float zPos2 = Random.Range(-110f, -113f);
            // X���W�� 115 �����̎��̂ݐ���
            if (spawnPosition.x >= 115f) return;
            GenerateFromList(new Vector3(spawnPosition.x, spawnPosition.y, zPos1), obstacles);
            GenerateFromList(new Vector3(spawnPosition.x, spawnPosition.y, zPos2), obstaclesList2);
        }
        if (yPos >= -10f)
        {
            // y >= -10 �̏ꍇ�̐���
            float xPos1 = Random.Range(100.3f, 106.3f);
            float xPos2 = Random.Range(107.3f, 112.3f);
            // Z���W�� -20 �ȉ��̎��̂ݐ���
            if (spawnPosition.z > -30f) return;
            GenerateFromList(new Vector3(xPos1, spawnPosition.y, spawnPosition.z), obstacles);
            GenerateFromList(new Vector3(xPos2, spawnPosition.y, spawnPosition.z), obstaclesList2);
            GenerateFromList(new Vector3(xPos1, spawnPosition.y, spawnPosition.z), obstacles);
            GenerateFromList(new Vector3(xPos2, spawnPosition.y, spawnPosition.z), obstaclesList2);
        }
        if (yPos < -10f)
        {
            // y < -10 �̏ꍇ�̐���
            //�����[�g
            if (zPos > -100f)
            {
                float zPos1 = Random.Range(-20f, -17f);
                float zPos2 = Random.Range(-13f, -16f);
                GenerateFromList(new Vector3(spawnPosition.x, spawnPosition.y, zPos1), obstacles);
                GenerateFromList(new Vector3(spawnPosition.x, spawnPosition.y, zPos2), obstaclesList2);
            }
            else//�E���[�g
            {
                float zPos1 = Random.Range(-235f, -227f);
                float zPos2 = Random.Range(-220f, -226f);
                GenerateFromList(new Vector3(spawnPosition.x, spawnPosition.y + 0.5f, zPos1), obstacles);
                GenerateFromList(new Vector3(spawnPosition.x, spawnPosition.y + 0.5f, zPos2), obstaclesList2);
                GenerateFromList(new Vector3(spawnPosition.x, spawnPosition.y + 0.5f, zPos1), obstacles);
                GenerateFromList(new Vector3(spawnPosition.x, spawnPosition.y + 0.5f, zPos2), obstaclesList2);
            }
        }
    }

    void GenerateFromList(Vector3 spawnPosition, GameObject[] selectedObstacles)
    {
        // �v���C���[����\���ȋ�����ۂ`�F�b�N
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

    void IncreaseEnemies()
    {
        float yPos = Target.position.y;

        if (yPos >= 0)
        {
            minObstacleInterval = 1.5f;
            maxObstacleInterval = 2f;
        }
        else if (yPos >= -6f)
        {
            minObstacleInterval = 0.8f;
            maxObstacleInterval = 1.5f;
        }
        else if (yPos >= -10f)
        {
            minObstacleInterval = 0.5f;
            maxObstacleInterval = 1.2f;
        }
        else
        {
            minObstacleInterval = 0.3f;
            maxObstacleInterval = 1f;
        }
    }

    IEnumerator WaitAndDestroy(GameObject obstacle)
    {
        yield return new WaitForSeconds(1f);
        Destroy(obstacle);
    }
}