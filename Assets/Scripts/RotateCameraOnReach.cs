using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraOnReach : MonoBehaviour
{
    public Transform player; // プレイヤーのアバター
    private Camera mainCamera; // メインカメラ
    private bool hasRotated = false; // 1回だけ回転させるためのフラグ

    void Start()
    {
        mainCamera = Camera.main; // メインカメラを取得
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player && !hasRotated)
        {
            RotateCamera();
            hasRotated = true; // 回転済みフラグを立てる
        }
    }

    void RotateCamera()
    {
        // プレイヤーの現在のY回転を取得
        float currentYRotation = player.eulerAngles.y;
        // 新しいY回転を設定（180度追加）
        float newYRotation = currentYRotation - 90f;

        // プレイヤーのアバターを回転させる
        player.eulerAngles = new Vector3(player.eulerAngles.x, newYRotation, player.eulerAngles.z);

        // メインカメラを回転させる
        mainCamera.transform.eulerAngles = new Vector3(mainCamera.transform.eulerAngles.x, newYRotation, mainCamera.transform.eulerAngles.z);
    }
}
