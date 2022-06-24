using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Extensions;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RadialLayout : MonoBehaviour
{
    [Range(0f, 360f)]
    public float MinAngle, MaxAngle, StartAngle;

    private RectTransform rect;

    public void Awake()
    {
        rect = GetComponent<RectTransform>();
    }


    public void CalculateRadial()
    {
        if (transform.childCount == 0)
            return;

        float fOffsetAngle = ((MaxAngle - MinAngle)) / (transform.childCount - 1);

        float fAngle = StartAngle;

        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = (RectTransform)transform.GetChild(i);
            if (child == null)
            {
                continue;
            }

            Vector3 vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0) / 2 + (Vector3)rect.pivot;

            fAngle += fOffsetAngle;
            child.SetCenterAnchor(vPos);
        }
    }
}
