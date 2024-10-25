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

        // �v���C���[�̑O���ɐ����ʒu���v�Z�i�����̓v���C���[�̍����ɍ��킹��j
        Vector3 spawnPosition = Target.position + randomDirection.normalized * obstacleDistance;
        spawnPosition.y = Target.position.y; // �v���C���[�Ɠ��������ɒ���

        // NavMesh��̗L���Ȉʒu���擾
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, sampleRadius, NavMesh.AllAreas))
        {
            // �������v���C���[�Ɠ����ɒ���
            Vector3 validPosition = hit.position;
            validPosition.y = Target.position.y; // �v���C���[�Ɠ��������ɐݒ�

            // �����_���ȏ�Q���𐶐�
            int obstacleIndex = Random.Range(0, obstacles.Length);
            Quaternion spawnRotation = obstacles[obstacleIndex].transform.rotation;
            GameObject obstacle = Instantiate(obstacles[obstacleIndex], validPosition, spawnRotation);

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
        // �v���C���[�̌���x�N�g�����v�Z�iforward�̔��Ε����j
        Vector3 playerBackward = -Target.forward;

        for (int i = ObstacleList.Count - 1; i >= 0; i--)
        {
            // ��Q���ƃv���C���[�Ԃ̈ʒu�x�N�g��
            Vector3 obstacleDirection = ObstacleList[i].transform.position - Target.position;

            // ���ς��g���ăv���C���[����ɂ��邩�𔻒�
            float dotProduct = Vector3.Dot(obstacleDirection.normalized, playerBackward);

            if (dotProduct > 0) // ���ς����Ȃ����ɂ���Ɣ��f
            {
                GameObject passedObstacle = ObstacleList[i];
                ObstacleList.RemoveAt(i); // ���X�g����폜
                Destroy(passedObstacle); // ��Q�����폜
            }
        }
    }

}
