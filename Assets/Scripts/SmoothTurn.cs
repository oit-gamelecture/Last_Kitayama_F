using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothTurn : MonoBehaviour
{
    public Transform[] turnPoints;  // �Ȃ���p�̈ʒu�iTransform�Őݒ�j
    private int currentTurnPoint = 0; // ���݂̃^�[���|�C���g
    private Vector3 targetDirection;  // �ڕW����
    public float turnSpeed = 5f;  // �����]���̑��x

    private void Start()
    {
        // �����i�s�������L�����N�^�[���i��ł�������ɐݒ�
        targetDirection = transform.forward;
    }

    private void Update()
    {
        // �Ȃ���p�̈ʒu���ݒ肳��Ă���ꍇ
        if (currentTurnPoint < turnPoints.Length)
        {
            Vector3 turnPoint = turnPoints[currentTurnPoint].position;

            // �ڕW�n�_�ɋ߂Â�����A�^�[�����J�n
            if (Vector3.Distance(transform.position, turnPoint) < 1f) // �߂Â�����^�[��
            {
                // �v���C���[�̌��݂̐i�s��������-90�x��]
                targetDirection = Quaternion.Euler(0, 0f, 0) * transform.forward;

                // �L�����N�^�[���̂��^�[�Q�b�g�����ɉ�]
                transform.rotation = Quaternion.LookRotation(targetDirection);

                // ���̃^�[���|�C���g�֐i��
                currentTurnPoint++;
            }
        }

        // �����]��
        RotateTowardsTargetDirection();
    }

    private void RotateTowardsTargetDirection()
    {
        // ���݂̕�������^�[�Q�b�g�����փX���[�Y�ɉ�]������
        Vector3 direction = Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Time.deltaTime, 0f);

        // �L�����N�^�[�̌������X�V
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
