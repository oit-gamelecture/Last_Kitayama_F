using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMesh���g�p���邽�߂̖��O���

public class EnemyManager : MonoBehaviour
{
    public Transform Target; // �v���C���[�̎Q��
    public GameObject[] obstacles; // ��Q���̃v���n�u
    public List<GameObject> ObstacleList = new List<GameObject>(); // ��Q�����X�g

    public float minObstacleInterval = 1f; // ��Q�������̍ŏ��Ԋu
    public float maxObstacleInterval = 3f; // ��Q�������̍ő�Ԋu
    private float nextObstacleTime; // ���̏�Q����������

    public float obstacleDistance = 40f; // �v���C���[�����Q���𐶐����鋗��
    public float sampleRadius = 5f; // NavMesh�T���v�����O�̔��a
    public float spawnAngleRange = 30f; // ������������̊p�x�͈́i�x���@�j

    private float timer = 0f; // �^�C�}�[

    void Start()
    {
        ScheduleNextObstacle(); // ����̏�Q�������^�C�~���O��ݒ�
    }

    void Update()
    {
        timer += Time.deltaTime; // �o�ߎ��Ԃ��X�V

        // ��Q���𐶐�����^�C�~���O�ɒB�����琶��
        if (Time.time >= nextObstacleTime)
        {
            GenerateObstacleOnNavMesh(); // NavMesh��ɏ�Q���𐶐�
            ScheduleNextObstacle(); // ���̐����^�C�~���O��ݒ�
        }

        // �ʂ�߂�����Q�����폜
        RemovePassedObstacles();
    }

    // NavMesh��ɏ�Q���𐶐�����
    void GenerateObstacleOnNavMesh()
    {
        // �����_���Ȋp�x�ŕ���������
        float randomAngle = Random.Range(-spawnAngleRange, spawnAngleRange);
        Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * Target.forward;

        // �v���C���[�̑O���Ń����_���ȕ����ɐ����ʒu���v�Z
        Vector3 spawnPosition = Target.position + randomDirection.normalized * obstacleDistance;

        // NavMesh��̗L���Ȉʒu���擾
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, sampleRadius, NavMesh.AllAreas))
        {
            // �L���Ȉʒu�����������ꍇ�A��Q���𐶐�����
            int obstacleIndex = Random.Range(0, obstacles.Length);
            Quaternion spawnRotation = obstacles[obstacleIndex].transform.rotation;
            GameObject obstacle = Instantiate(obstacles[obstacleIndex], hit.position, spawnRotation);

            ObstacleList.Add(obstacle); // ��Q�������X�g�ɒǉ�
        }
        else
        {
            //Debug.Log("NavMesh��ɓK�؂Ȑ����ʒu��������܂���ł���");
        }
    }

    // ���̏�Q�������^�C�~���O��ݒ�
    void ScheduleNextObstacle()
    {
        nextObstacleTime = Time.time + Random.Range(minObstacleInterval, maxObstacleInterval);
    }

    // �ʂ�߂�����Q�����폜
    void RemovePassedObstacles()
    {
        for (int i = ObstacleList.Count - 1; i >= 0; i--)
        {
            if (ObstacleList[i].transform.position.z > Target.position.z + 10f)
            {
                GameObject passedObstacle = ObstacleList[i];
                ObstacleList.RemoveAt(i);
                Destroy(passedObstacle); // ��Q�����폜
            }
        }
    }
}
