using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private Button volumeButton;      // ���ʒ��߃{�^��
    [SerializeField] private Slider volumeSlider;      // ���ʃX���C�_�[

    private void Start()
    {
        // �����ݒ�
        volumeSlider.value = AudioListener.volume;

        volumeButton.Select(); // �����I�������ʒ��߃{�^����
        volumeSlider.gameObject.SetActive(true); // �X���C�_�[����ɕ\��
    }

    private void Update()
    {
        // Esc�L�[��Title�V�[���ɑJ��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Title");
        }

        // A�L�[��D�L�[�ŃX���C�_�[�̒l�𒲐�
        if (Input.GetKey(KeyCode.D))
        {
            volumeSlider.value = Mathf.Clamp(volumeSlider.value + 0.005f, 0, 1);
            AudioListener.volume = volumeSlider.value; // ���ʂ��X�V
        }
        else if (Input.GetKey(KeyCode.A))
        {
            volumeSlider.value = Mathf.Clamp(volumeSlider.value - 0.005f, 0, 1);
            AudioListener.volume = volumeSlider.value; // ���ʂ��X�V
        }

        // Enter�L�[�Ń{�^���I���ɖ߂�
        if (Input.GetKeyDown(KeyCode.Return))
        {
            volumeButton.Select(); // ���ʃ{�^���ɃJ�[�\����߂�
        }
    }
}
