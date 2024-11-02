using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActEne : MonoBehaviour
{
    public GameObject targetObject; // アクティブ化したいオブジェクト
    public float activationDistance = 5f; // プレイヤーが近づく距離のしきい値

    private GameObject player;

    void Start()
    {
        // プレイヤータグがついたオブジェクトを探す
        player = GameObject.FindGameObjectWithTag("Player");

        // 対象オブジェクトを初期状態で非アクティブ化
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player != null)
        {
            // プレイヤーとの距離を計算
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // 指定した距離内にプレイヤーが来たら
            if (distanceToPlayer <= activationDistance)
            {
                // 自分自身を非アクティブ化
                gameObject.SetActive(false);

                // シリアライズされたオブジェクトをアクティブ化
                if (targetObject != null)
                {
                    targetObject.SetActive(true);
                }
            }
        }
    }
}
