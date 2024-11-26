using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMove : MonoBehaviour
{
    public RectTransform textTransform;
    public float speed = 100f;
    public float stopPositionY = 0f;
    public GameObject[] objectsToActivate;

    private bool isMoving = true;

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = textTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            textTransform.localPosition -= new Vector3(0, speed * Time.deltaTime, 0);

            if (textTransform.localPosition.y <= stopPositionY)
            {
                textTransform.localPosition = new Vector3(textTransform.localPosition.x, stopPositionY, textTransform.localPosition.z);
                isMoving = false;

                ActivateObjects();
            }
        }
    }

    private void ActivateObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    private void ActivateObjectsWithDelay(float delay)
    {
        StartCoroutine(ActivateAfterDelay(delay));
    }

    private IEnumerator ActivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

}
