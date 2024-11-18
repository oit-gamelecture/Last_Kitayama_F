using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicController : MonoBehaviour
{
    private Image comicImage;          // �����\������Image
    public Sprite[] comicFrames;       // ����̊e�R�}
    private int currentFrame = 0;      // ���݂̃t���[��
    public LoadingMainScene loadingScene; // ���[�h��ʂ̃X�N���v�g�Q��

    void Start()
    {
        // �q�I�u�W�F�N�g����Image�R���|�[�l���g���擾
        comicImage = GetComponentInChildren<Image>();

        // �ŏ��̃R�}��\��
        if (comicImage != null && comicFrames.Length > 0)
        {
            comicImage.sprite = comicFrames[currentFrame];
        }
    }

    void Update()
    {
        // Enter�L�[�Ŏ��̃R�}��
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShowNextFrame();
        }

        // ESC�L�[�Œ��ڃ��[�h��ʂ�\�����ăV�[���J��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC�L�[��������܂����B���[�h��ʂ�\�����ă��C���V�[���ֈڍs���܂��B");
            StartLoadingScene();
        }
    }

    void ShowNextFrame()
    {
        if (comicImage != null && currentFrame < comicFrames.Length - 1)
        {
            // ���̃R�}��\��
            currentFrame++;
            comicImage.sprite = comicFrames[currentFrame];
        }
        else if (currentFrame == comicFrames.Length - 1)
        {
            // �Ō�̃R�}�̂Ƃ��Ƀ��[�h��ʂ�\�����ăV�[���J��
            Debug.Log("����I���I ���[�h��ʂ�\�����ă��C���V�[���ֈڍs���܂��B");
            StartLoadingScene();
        }
    }

    void StartLoadingScene()
    {
        // ���[�h��ʂ�\�����ăV�[���J��
        if (loadingScene != null)
        {
            loadingScene.LoadNextScene();
        }
        else
        {
            Debug.LogError("LoadingMainScene���ݒ肳��Ă��܂���B");
        }
    }
}
