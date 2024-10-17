
using System.Collections;
using UnityEngine;

public class enemymovement : MonoBehaviour
{
    public Animator enemyanimator;
    public bool fall;
    public bool help;
    public float speed = 3.0f;
    public float downspeed = -0.6f;

    [SerializeField] private float moveDirectionZ = 1.0f; // Z軸の移動方向（1: 前進, -1: 後退）
    BoxCollider boxCol;

    // Start is called before the first frame update
    void Start()
    {
        boxCol = GetComponent<BoxCollider>();
        fall = false;
        help = false;
        enemyanimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!fall) // 非転倒時
        {
            MoveEnemy();
        }
        else // 転倒時
        {
            boxCol.enabled = false; // 当たり判定を消す
            StartCoroutine(Down());
        }
    }

    // 敵の移動処理
    void MoveEnemy()
    {
        // Z軸方向に移動 (1で前進、-1で後退)
        Vector3 movement = new Vector3(0, 0, moveDirectionZ) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    IEnumerator Down()
    {
        enemyanimator.SetTrigger("Fall");
        transform.Translate(Vector3.back * downspeed * Time.deltaTime, Space.World);
        yield return new WaitForSeconds(0.8f);
        downspeed = 0;
        yield return new WaitForSeconds(5.0f);
        Destroy(this.gameObject);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            fall = true;
            enemyanimator.CrossFade("Fall", 0);
            Debug.Log("Collision with player detected");
        }
    }
}
