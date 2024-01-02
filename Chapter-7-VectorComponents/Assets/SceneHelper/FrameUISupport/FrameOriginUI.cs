using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameOriginUI : MonoBehaviour
{
    private const float kAxisPointDistance = 2.0f;
    public Transform Pt, Pz;
    private FrameUpdatePt ptUpdate;
    private FrameUpdatePz pzUpdate;
    private Vector3 oldPos = Vector3.zero;
    private Vector3 tDir, zDir;
    private Vector3 yDir, xDir;
    // Start is called before the first frame update
    void Start()
    {
        // oldPos = transform.localPosition;
        tDir = Pt.localPosition - oldPos;
        zDir = (Pz.localPosition - oldPos).normalized  * kAxisPointDistance;
        ComputeFrame();

        ptUpdate = Pt.gameObject.GetComponent<FrameUpdatePt>();
        pzUpdate = Pz.gameObject.GetComponent<FrameUpdatePz>();

        ptUpdate.OriginSetPos(oldPos + xDir);
        pzUpdate.OriginSetPos(oldPos + zDir);
    }

    void ComputeFrame() {
        yDir = Vector3.Cross(zDir, tDir).normalized;
        xDir = Vector3.Cross(yDir, zDir).normalized * kAxisPointDistance;
    }

    public Vector3 PtNewPosition(Vector3 pt) {
        tDir = pt - oldPos;
        ComputeFrame();
        pt = oldPos + xDir;
        pzUpdate.OriginSetPos(oldPos + zDir);
        pzUpdate.ValueSetByFrame = true;
        return pt;
    }

    public Vector3 PzNewPosition(Vector3 pz) {
        zDir = (pz - oldPos).normalized;
        ComputeFrame();
        pz = oldPos + zDir;
        ptUpdate.OriginSetPos(oldPos + xDir);
        return pz;
    }

    // Update is called once per frame
    void Update()
    {
        // If Po has been moved
        if ((oldPos - transform.localPosition).magnitude > 0.005f) {
            oldPos = transform.localPosition;
            ptUpdate.OriginSetPos(oldPos + xDir);
            ptUpdate.ValueSetByFrame = true;
            pzUpdate.OriginSetPos(oldPos + zDir);
            pzUpdate.ValueSetByFrame = true;
        }
    }
}
