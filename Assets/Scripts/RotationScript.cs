using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    public Transform player; // �v���C���[�̃A�o�^�[
    private Camera mainCamera; // ���C���J����
    private bool hasRotated = false; // ��]��1�񂾂��s�����߂̃t���O

    void Start()
    {
        mainCamera = Camera.main; // ���C���J�������擾
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player && !hasRotated)
        {
            TeleportPlayerToCenter();
            RotatePlayerAndCameraOnce();
            hasRotated = true; // ��]�ς݃t���O�𗧂Ă�
            Destroy(gameObject); // ���̃X�N���v�g���A�^�b�`����Ă���I�u�W�F�N�g��j��
        }
    }

    private void TeleportPlayerToCenter()
    {
        // �v���C���[�����̃I�u�W�F�N�g�̒��S�Ƀ��[�v������
        player.position = transform.position;
    }

    private void RotatePlayerAndCameraOnce()
    {
        // ���݂̃v���C���[�̉�]���擾
        Vector3 playerRotation = player.eulerAngles;
        // �v���C���[��y����-90�x��]
        playerRotation.y -= 90f;
        player.eulerAngles = playerRotation;

        // ���݂̃J�����̉�]���擾
        Vector3 cameraRotation = mainCamera.transform.eulerAngles;
        // �J������y����-90�x��]
        cameraRotation.y -= 90f;
        mainCamera.transform.eulerAngles = cameraRotation;
    }
}
