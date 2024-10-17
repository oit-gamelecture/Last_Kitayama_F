
using System.Collections;
using UnityEngine;

public class enemymovement : MonoBehaviour
{
    public Animator enemyanimator;
    public bool fall;
    public bool help;
    public float speed = 3.0f;
    public float downspeed = -0.6f;

    [SerializeField] private float moveDirectionZ = 1.0f; // Z²‚ÌˆÚ“®•ûŒüi1: ‘Oi, -1: Œã‘Şj
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
        if (!fall) // ”ñ“]“|
        {
            MoveEnemy();
        }
        else // “]“|
        {
            boxCol.enabled = false; // “–‚½‚è”»’è‚ğÁ‚·
            StartCoroutine(Down());
        }
    }

    // “G‚ÌˆÚ“®ˆ—
    void MoveEnemy()
    {
        // Z²•ûŒü‚ÉˆÚ“® (1‚Å‘OiA-1‚ÅŒã‘Ş)
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
