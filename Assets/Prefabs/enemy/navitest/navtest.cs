using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;  // NavMeshの使用に必須

public class NavTest : MonoBehaviour
{
    public GameObject goal;  // 目的地オブジェクトの変数
    public NavMeshAgent agent;  // NavMeshAgentの変数

    void Start()
    {
        // NavMeshAgentのコンポーネント取得
        agent = GetComponent<NavMeshAgent>();

        // 目的地オブジェクトを取得
        goal = GameObject.Find("goalo");

        // デバッグ用：目的地が見つからない場合のログ
        if (goal == null)
        {
            Debug.LogError("目的地 'goalo' が見つかりません。");
        }
    }

    void Update()
    {
        if (goal != null)
        {
            // 目的地の座標を設定
            agent.destination = goal.transform.position;
        }
    }
}
