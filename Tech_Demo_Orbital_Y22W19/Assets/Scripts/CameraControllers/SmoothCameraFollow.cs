using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform followTransform;


    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, followTransform.position) > Mathf.Epsilon)
        {
            float z = transform.position.z;
            Vector3 newPosition = followTransform.position;
            newPosition.z = z;
            transform.position = newPosition;
        }
    }

    public void SetFollowing(Transform transform)
    {
        followTransform = transform;
    }
}
