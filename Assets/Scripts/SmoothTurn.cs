using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothTurn : MonoBehaviour
{
    public Transform[] turnPoints;  // 曲がり角の位置（Transformで設定）
    private int currentTurnPoint = 0; // 現在のターンポイント
    private Vector3 targetDirection;  // 目標方向
    public float turnSpeed = 5f;  // 方向転換の速度

    private void Start()
    {
        // 初期進行方向をキャラクターが進んでいる方向に設定
        targetDirection = transform.forward;
    }

    private void Update()
    {
        // 曲がり角の位置が設定されている場合
        if (currentTurnPoint < turnPoints.Length)
        {
            Vector3 turnPoint = turnPoints[currentTurnPoint].position;

            // 目標地点に近づいたら、ターンを開始
            if (Vector3.Distance(transform.position, turnPoint) < 1f) // 近づいたらターン
            {
                // プレイヤーの現在の進行方向から-90度回転
                targetDirection = Quaternion.Euler(0, 0f, 0) * transform.forward;

                // キャラクター自体もターゲット方向に回転
                transform.rotation = Quaternion.LookRotation(targetDirection);

                // 次のターンポイントへ進む
                currentTurnPoint++;
            }
        }

        // 方向転換
        RotateTowardsTargetDirection();
    }

    private void RotateTowardsTargetDirection()
    {
        // 現在の方向からターゲット方向へスムーズに回転させる
        Vector3 direction = Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Time.deltaTime, 0f);

        // キャラクターの向きを更新
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
