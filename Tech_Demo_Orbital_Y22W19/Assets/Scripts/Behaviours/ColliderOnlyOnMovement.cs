using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderOnlyOnMovement : MonoBehaviour
{
    private Collider2D collider2d;
    private Vector3 lastPosition;
    public float timeInterval = 0.5f;

    private void Awake()
    {
        collider2d = GetComponent<Collider2D>();
        StartCoroutine(UpdateLastPosition());
    }
    private void Update()
    {
        if (transform.position != lastPosition)
        {
            collider2d.enabled = false;
        }
    }

    private IEnumerator UpdateLastPosition()
    {
        yield return new WaitForSeconds(timeInterval);
        collider2d.enabled = transform.position == lastPosition;
        lastPosition = transform.position;
        StartCoroutine(UpdateLastPosition());
    }
}
