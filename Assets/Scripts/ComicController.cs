using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicController : MonoBehaviour
{
    private Image comicImage;                  // �����\������Image
    public Sprite[] comicFrames;               // ����̊e�R�}
    private int currentAudioIndex = 0;         // ���݂�AudioClip�C���f�b�N�X
    public AudioClip[] audioClips;             // �e�R�}�ɑΉ����鉹��
    public AudioSource audioSource;            // �������Đ�����AudioSource
    public LoadingMainScene loadingScene;      // ���[�h��ʂ̃X�N���v�g�Q��

    // AudioClip��ComicFrame�̑Ή����X�g
    private readonly Dictionary<int, int> audioToFrameMap = new Dictionary<int, int>
    {
        { 0, 0 },  // AudioClip[0] �ɑ΂��� ComicFrame[0] ��\��
        { 2, 1 },
        { 8, 2 },
        { 12, 3 },
        { 14, 4 },
        { 17, 5 }
    };

    private bool isPlaying = false;            // �Đ������ǂ����̃t���O

    void Start()
    {
        // �q�I�u�W�F�N�g����Image�R���|�[�l���g���擾
        comicImage = GetComponentInChildren<Image>();

        // �ŏ��̃R�}�Ɖ�����ݒ�
        if (comicImage != null && comicFrames.Length > 0)
        {
            comicImage.sprite = comicFrames[0]; // �ŏ��̃R�}��\��
            PlayAudio(); // �ŏ��̉������Đ�
        }
    }

    void Update()
    {
        // Enter�L�[�Ŏ��̉������Đ�
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShowNextFrameAndAudio();
        }

        // ESC�L�[�Œ��ڃ��[�h��ʂ�\�����ăV�[���J��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC�L�[��������܂����B���[�h��ʂ�\�����ă��C���V�[���ֈڍs���܂��B");
            StartLoadingScene();
        }

        // �����̍Đ��I�����`�F�b�N���Ď��̃t���[���Ɖ������Đ�
        if (!audioSource.isPlaying && isPlaying)
        {
            ShowNextFrameAndAudio();
        }
    }

    void ShowNextFrameAndAudio()
    {
        if (currentAudioIndex < audioClips.Length - 1)
        {
            currentAudioIndex++; // ���̉����ɐi��
            PlayAudio();

            // AudioClip�C���f�b�N�X���Ή����X�g�Ɋ܂܂��ꍇ�̂�ComicFrame���X�V
            if (audioToFrameMap.ContainsKey(currentAudioIndex))
            {
                int frameIndex = audioToFrameMap[currentAudioIndex];
                if (frameIndex < comicFrames.Length)
                {
                    comicImage.sprite = comicFrames[frameIndex];
                }
                else
                {
                    Debug.LogError($"�Ή�����ComicFrame�̃C���f�b�N�X {frameIndex} ���͈͊O�ł��B");
                }
            }
        }
        else
        {
            Debug.Log("�Ō�̉������Đ�����܂����B");
            StartLoadingScene(); // �Ō�̉����Đ���̓V�[���J��
        }
    }

    void PlayAudio()
    {
        // �������Đ�
        if (audioClips.Length > currentAudioIndex && audioSource != null)
        {
            audioSource.clip = audioClips[currentAudioIndex];
            audioSource.Play();
            isPlaying = true; // �Đ����t���O���I���ɂ���
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
