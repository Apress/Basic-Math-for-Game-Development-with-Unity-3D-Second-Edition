using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameYPosUI : MonoBehaviour
{
    private const float kAxisPointDistance = 2.0f;
    public Transform origin, zPos;
    public Vector3 oldPos = Vector3.zero;
    private Vector3 yDir, zDir;
    // Start is called before the first frame update
    void Start()
    {
        // oldPos = transform.localPosition;
        yDir = origin.localPosition - oldPos;
        zDir = zPos.localPosition - oldPos;
    }

    // Update is called once per frame
    void Update()
    {
        if ((oldPos - transform.localPosition).magnitude > 0.1f) {
            Vector3 v = transform.localPosition - origin.localPosition;
            Vector3 xDir = Vector3.Cross(v, zDir);
            zDir = (Vector3.Cross(xDir, v)).normalized * kAxisPointDistance;
            zPos.localPosition = origin.localPosition + zDir;

            transform.localPosition = origin.localPosition + v.normalized * kAxisPointDistance;
            oldPos = transform.localPosition;
        }
        
    }
}
