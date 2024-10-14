using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemymovement : MonoBehaviour
{
    public Animator enemyanimator;
    public bool fall;
    public bool help;
    public Vector3 targetPosition;
    public float speed = 3.0f;
    public float downspeed = -0.6f;
    [SerializeField] Transform movetarget;
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
        if (fall == false)//îÒì]ì|éû
        {
            Vector3 direction = (movetarget.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, movetarget.position, speed * Time.deltaTime);
        }

        if (fall == true)//ì]ì|éû
        {
            boxCol.enabled = false;//Ç±Ç±Ç≈ìñÇΩÇËîªíËÇè¡Ç∑
            StartCoroutine(Down());
        }

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
