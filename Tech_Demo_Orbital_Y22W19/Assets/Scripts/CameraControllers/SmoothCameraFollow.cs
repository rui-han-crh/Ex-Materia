using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour, ISaveable
{
    private static SmoothCameraFollow instance;
    public static SmoothCameraFollow Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SmoothCameraFollow>();
                Debug.Assert(instance != null, "There was no SmoothCameraFollow in this scene, consider adding one");
            }
            return instance;
        }
    }

    [SerializeField]
    private Transform followTransform;

    [SerializeField]
    private bool enableSmoothing = true;

    public bool EnableSmoothing
    {
        get { return enableSmoothing; }
        set { enableSmoothing = value; }
    }

    [SerializeField]
    private float displacementThreshold = 0.3f;

    [SerializeField]
    private float approachThreshold = 0.01f;

    [SerializeField]
    private float approachRate = 5f;

    private bool isApproaching = false;


    // Update is called once per frame
    void Update()
    {
        if (!enableSmoothing)
        {
            float _z = transform.position.z;
            Vector3 snapPosition = followTransform.position;
            snapPosition.z = _z;
            transform.position = snapPosition;
            return;
        }

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

    public void SaveData()
    {
        SaveFile.file.Save(typeof(SmoothCameraFollow), "enableSmoothing", enableSmoothing);
    }

    public void LoadData()
    {
        if (SaveFile.file.HasData(typeof(SmoothCameraFollow), "enableSmoothing"))
        {
            enableSmoothing = SaveFile.file.Load<bool>(typeof(SmoothCameraFollow), "enableSmoothing");
        }
    }
}
