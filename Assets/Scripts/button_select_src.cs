using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class button_select_src : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;
    bool select_f = false;                      //�{�^�����I��������true
    public GameObject yajirusi;                 //���̃C���[�W���Z�b�g
    GameObject selectedObj;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (select_f == false)
        {
            try
            {
                selectedObj = eventSystem.currentSelectedGameObject.gameObject;
                if (this.gameObject == selectedObj)    �@//�{�^�����I������Ă�����
                {
                    yajirusi.SetActive(true);           //����\��
                    select_f = true;
                }
            }
            catch
            {
                //�I������Ȃ��ꍇ�͉������Ȃ�
            }
        }
        else
        {
            try
            {
                selectedObj = eventSystem.currentSelectedGameObject.gameObject;
                if(this.gameObject != eventSystem.currentSelectedGameObject.gameObject) //�I������O�ꂽ��
                {
                    yajirusi.SetActive(false);  //�����\��
                    select_f = false;
                }
            }
            catch
            {
                //�I������Ȃ��ꍇ�͉������Ȃ�
            }
        }
    }
}
