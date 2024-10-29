using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class button_select_src : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;
    bool select_f = false;                      //ボタンが選択されるとtrue
    public GameObject yajirusi;                 //矢印のイメージをセット
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
                if (this.gameObject == selectedObj)    　//ボタンが選択されていたら
                {
                    yajirusi.SetActive(true);           //矢印を表示
                    select_f = true;
                }
            }
            catch
            {
                //選択されない場合は何もしない
            }
        }
        else
        {
            try
            {
                selectedObj = eventSystem.currentSelectedGameObject.gameObject;
                if(this.gameObject != eventSystem.currentSelectedGameObject.gameObject) //選択から外れたら
                {
                    yajirusi.SetActive(false);  //矢印を非表示
                    select_f = false;
                }
            }
            catch
            {
                //選択されない場合は何もしない
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(this.gameObject == selectedObj)
            {
                yajirusi.SetActive(false);
            }
        }
    }
}
