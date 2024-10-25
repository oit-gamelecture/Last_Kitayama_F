using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraOnReach : MonoBehaviour
{
    public Transform player; // �v���C���[�̃A�o�^�[
    private Camera mainCamera; // ���C���J����
    private bool hasRotated = false; // 1�񂾂���]�����邽�߂̃t���O

    void Start()
    {
        mainCamera = Camera.main; // ���C���J�������擾
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player && !hasRotated)
        {
            RotateCamera();
            hasRotated = true; // ��]�ς݃t���O�𗧂Ă�
        }
    }

    void RotateCamera()
    {
        // �v���C���[�̌��݂�Y��]���擾
        float currentYRotation = player.eulerAngles.y;
        // �V����Y��]��ݒ�i180�x�ǉ��j
        float newYRotation = currentYRotation - 90f;

        // �v���C���[�̃A�o�^�[����]������
        player.eulerAngles = new Vector3(player.eulerAngles.x, newYRotation, player.eulerAngles.z);

        // ���C���J��������]������
        mainCamera.transform.eulerAngles = new Vector3(mainCamera.transform.eulerAngles.x, newYRotation, mainCamera.transform.eulerAngles.z);
    }
}
