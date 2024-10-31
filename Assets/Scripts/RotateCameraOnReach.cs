using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraOnReach : MonoBehaviour
{
    public Transform player; // プレイヤーのアバター
    private Camera mainCamera; // メインカメラ
    private bool hasRotated = false; // 回転を1回だけ行うためのフラグ

    void Start()
    {
        mainCamera = Camera.main; // メインカメラを取得
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player && !hasRotated)
        {
            Invoke("RotatePlayerAndCameraOnce", 1.0f); // 2秒後に回転を開始
            hasRotated = true; // 回転済みフラグを立てる
            Destroy(gameObject, 2.0f); // このスクリプトがアタッチされているオブジェクトを2秒後に破壊
        }
    }

    private void RotatePlayerAndCameraOnce()
    {
        // 現在のプレイヤーの回転を取得
        Vector3 playerRotation = player.eulerAngles;
        // プレイヤーをy軸で-90度回転
        playerRotation.y -= 90f;
        player.eulerAngles = playerRotation;

        // 現在のカメラの回転を取得
        Vector3 cameraRotation = mainCamera.transform.eulerAngles;
        // カメラもy軸で-90度回転
        cameraRotation.y -= 90f;
        mainCamera.transform.eulerAngles = cameraRotation;
    }
}


