using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Button volumeButton;
    public Button titleButton;
    public Slider volumeSlider;
    public GameObject defaultSelectedButton;

    private bool isAdjustingVolume = false;

    void Start()
    {
        // �ŏ��ɑI�������{�^����ݒ�
        EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
        EventSystem.current.SetSelectedGameObject(volumeButton.gameObject);

        // �X���C�_�[���\���ɂ��Ė�����
        volumeSlider.interactable = false;
        volumeSlider.value = AudioListener.volume;
    }

    void Update()
    {
        if (isAdjustingVolume)
        {
            HandleVolumeAdjustment();
        }
        else
        {
            HandleButtonSelection();
        }
    }

    void HandleButtonSelection()
    {
        // Enter�L�[�ŉ��ʒ��߃��[�h�ɐ؂�ւ�
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0")) && EventSystem.current.currentSelectedGameObject == volumeButton.gameObject)
        {
            EnterVolumeAdjustmentMode();
        }

        // �^�C�g���V�[���ɖ߂鏈��
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0")) && EventSystem.current.currentSelectedGameObject == titleButton.gameObject)
        {
            SceneManager.LoadScene("Title");
        }
    }

    void HandleVolumeAdjustment()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        volumeSlider.value += horizontalInput * Time.deltaTime;

        // AudioListener.volume�ɃX���C�_�[�̒l�𔽉f
        AudioListener.volume = volumeSlider.value;

        // Enter�L�[�ŉ��ʒ��߃��[�h���I��
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
        {
            ExitVolumeAdjustmentMode();
        }
    }

    void EnterVolumeAdjustmentMode()
    {
        isAdjustingVolume = true;

        // �X���C�_�[��\�����đI��
        volumeSlider.interactable = true;
        EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);

        // ���̃{�^���𖳌���
        volumeButton.interactable = false;
        titleButton.interactable = false;
    }

    void ExitVolumeAdjustmentMode()
    {
        isAdjustingVolume = false;

        // �X���C�_�[���\���ɂ��ă��Z�b�g

        volumeSlider.interactable = false;
        // ���̃{�^�����ĂїL����
        volumeButton.interactable = true;
        titleButton.interactable = true;

        // ���ʒ��߃{�^�����ĂёI��
        EventSystem.current.SetSelectedGameObject(volumeButton.gameObject);
    }
}
