using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //FPS�l
        const int FPS = 60;

        // fps��ݒ�ł���悤�ɂ��邽�߂̏���
        // 0 : fps�ݒ�ł���悤�ɂ���
        QualitySettings.vSyncCount = 0;

        //�E�B���h�E�T�C�Y�Œ�
        //�����F�X�N���[��Width
        //�����F�X�N���[��Height
        //�����F�t���X�N���[����ԁitrue:�t���X�N���[��,false:Not�t���X�N���[���j
        //�����F��ʍX�V��FPS
        Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreen, FPS);


        //FPS�̌Œ�
        //�����ɂČŒ肷��FPS�́AUpdate�̊Ԋu���w�肷����̂ł���
        //60fps�Œ�
        Application.targetFrameRate = FPS;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
