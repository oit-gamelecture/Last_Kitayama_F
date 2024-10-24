using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Transform Target; // �v���C���[�̎Q��
    public GameObject[] obstacles; // ��Q���̃v���n�u
    public List<GameObject> ObstacleList = new List<GameObject>(); // ��Q�����X�g

    public float minObstacleInterval = 1f; // ��Q�������̍ŏ��Ԋu
    public float maxObstacleInterval = 3f; // ��Q�������̍ő�Ԋu
    private float nextObstacleTime; // ���̏�Q����������

    public float obstacleDistance = 40f; // �v���C���[�����Q���𐶐����鋗��
    private float minX = -0.2f; // X���W�̍ŏ��l
    private float maxX = 3.0f;  // X���W�̍ő�l

    private float timer = 0f; // �^�C�}�[

    // Start�͍ŏ��ɌĂ΂��
    void Start()
    {
        ScheduleNextObstacle(); // ����̏�Q�������^�C�~���O��ݒ�
    }

    // Update�͖��t���[���Ă΂��
    void Update()
    {
        timer += Time.deltaTime; // �o�ߎ��Ԃ��X�V

        // ��Q���𐶐�����^�C�~���O�ɒB�����琶��
        if (Time.time >= nextObstacleTime)
        {
            GenerateObstacle();
            ScheduleNextObstacle(); // ���̐����^�C�~���O��ݒ�
        }

        // �ʂ�߂�����Q�����폜
        RemovePassedObstacles();
    }

    // ��Q���𐶐�����
    void GenerateObstacle()
    {
        float xPos = Random.Range(minX, maxX); // X���W�������_���Ɍ���
        int obstacleIndex = Random.Range(0, obstacles.Length); // �v���n�u�������_���I��

        // �v���C���[�̐i�s�����ɍ��킹�āAZ���̕������ɐ�������
        Vector3 spawnPosition = new Vector3(xPos, 0.5f, Target.position.z - obstacleDistance);

        // Z���W�� 100 �ȏ�Ȃ琶�����Ȃ�
        if (spawnPosition.z <= -100f)
        {
            Debug.Log("�ǂɐ������悤�Ƃ�����");
            return;
        }

        // �v���n�u�̉�]���擾���Đ���
        Quaternion spawnRotation = obstacles[obstacleIndex].transform.rotation;
        GameObject obstacle = Instantiate(obstacles[obstacleIndex], spawnPosition, spawnRotation);

        ObstacleList.Add(obstacle); // ��Q�������X�g�ɒǉ�
    }

    // ���̏�Q�������^�C�~���O��ݒ�
    void ScheduleNextObstacle()
    {
        nextObstacleTime = Time.time + Random.Range(minObstacleInterval, maxObstacleInterval);
    }

    // �v���C���[���ʂ�߂�����Q�����폜
    void RemovePassedObstacles()
    {
        for (int i = ObstacleList.Count - 1; i >= 0; i--)
        {
            // �v���C���[����Q����ʂ�߂���������iZ���̐i�s�����Ɋ�Â��j
            if (ObstacleList[i].transform.position.z > Target.position.z + 10f)
            {
                GameObject passedObstacle = ObstacleList[i];
                ObstacleList.RemoveAt(i);
                Destroy(passedObstacle); // ��Q�����폜
            }
        }
    }
}
