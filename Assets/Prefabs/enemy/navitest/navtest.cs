using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;  // NavMesh�̎g�p�ɕK�{

public class NavTest : MonoBehaviour
{
    public GameObject goal;  // �ړI�n�I�u�W�F�N�g�̕ϐ�
    public NavMeshAgent agent;  // NavMeshAgent�̕ϐ�

    void Start()
    {
        // NavMeshAgent�̃R���|�[�l���g�擾
        agent = GetComponent<NavMeshAgent>();

        // �ړI�n�I�u�W�F�N�g���擾
        goal = GameObject.Find("goalo");

        // �f�o�b�O�p�F�ړI�n��������Ȃ��ꍇ�̃��O
        if (goal == null)
        {
            Debug.LogError("�ړI�n 'goalo' ��������܂���B");
        }
    }

    void Update()
    {
        if (goal != null)
        {
            // �ړI�n�̍��W��ݒ�
            agent.destination = goal.transform.position;
        }
    }
}
