using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform followTransform;

    [SerializeField]
    private float displacementThreshold = 0.3f;

    [SerializeField]
    private float approachThreshold = 0.1f;

    [SerializeField]
    private float approachRate = 7f;

    private bool isApproaching = false;


    // Update is called once per frame
    void Update()
    {
        Vector2 transformPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 followPosition = new Vector2(followTransform.position.x, followTransform.position.y);

        float distance = Vector2.Distance(transformPosition, followPosition);

        if (distance > displacementThreshold || isApproaching)
        {
            isApproaching = true;
            float _z = transform.position.z;

            Vector3 nextPosition;

            if (distance < approachThreshold)
            {
                nextPosition = followPosition;
                isApproaching = false;
            } 
            else
            {
                nextPosition = Vector3.Lerp(transform.position, followTransform.position, approachRate * Time.deltaTime);
            }

            nextPosition.z = _z;
            transform.position = nextPosition;
        }
    }

    public void SetFollowing(Transform transform)
    {
        followTransform = transform;
    }

    private IEnumerator Follow()
    {
        while (Vector3.Distance(transform.position, followTransform.position) > Mathf.Epsilon)
        {
            float _z = transform.position.z;
            Vector3 nextPosition = Vector3.Lerp(transform.position, followTransform.position, Time.deltaTime);
            nextPosition.z = _z;
            transform.position = nextPosition;
            yield return null;
        }

        float z = transform.position.z;
        Vector3 newPosition = followTransform.position;
        newPosition.z = z;
        transform.position = newPosition;
    }
}
