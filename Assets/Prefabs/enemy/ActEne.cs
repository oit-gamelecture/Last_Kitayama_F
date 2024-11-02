using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActEne : MonoBehaviour
{
    public GameObject targetObject; // �A�N�e�B�u���������I�u�W�F�N�g
    public float activationDistance = 5f; // �v���C���[���߂Â������̂������l

    private GameObject player;

    void Start()
    {
        // �v���C���[�^�O�������I�u�W�F�N�g��T��
        player = GameObject.FindGameObjectWithTag("Player");

        // �ΏۃI�u�W�F�N�g��������ԂŔ�A�N�e�B�u��
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player != null)
        {
            // �v���C���[�Ƃ̋������v�Z
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // �w�肵���������Ƀv���C���[��������
            if (distanceToPlayer <= activationDistance)
            {
                // �������g���A�N�e�B�u��
                gameObject.SetActive(false);

                // �V���A���C�Y���ꂽ�I�u�W�F�N�g���A�N�e�B�u��
                if (targetObject != null)
                {
                    targetObject.SetActive(true);
                }
            }
        }
    }
}
