using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //FPS値
        const int FPS = 60;

        // fpsを設定できるようにするための処理
        // 0 : fps設定できるようにする
        QualitySettings.vSyncCount = 0;

        //ウィンドウサイズ固定
        //引数：スクリーンWidth
        //引数：スクリーンHeight
        //引数：フルスクリーン状態（true:フルスクリーン,false:Notフルスクリーン）
        //引数：画面更新のFPS
        Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreen, FPS);


        //FPSの固定
        //ここにて固定するFPSは、Updateの間隔を指定するものである
        //60fps固定
        Application.targetFrameRate = FPS;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
