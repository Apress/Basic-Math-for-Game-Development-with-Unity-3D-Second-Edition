using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameUpdatePt : MonoBehaviour
{
    // on Pt
    public GameObject Po;
    public bool ValueSetByFrame = false;
    private Vector3 oldPos = Vector3.zero;
    protected FrameOriginUI originUpdate;

    // Start is called before the first frame update
    void Start()
    {
        oldPos = transform.localPosition;
        originUpdate = Po.GetComponent<FrameOriginUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ValueSetByFrame)    // value has already been updated, don't do anything
            ValueSetByFrame = false;    
        else {
             if ((oldPos - transform.localPosition).magnitude > 0.1f) {
                updatePosition();
                oldPos = transform.localPosition;
             }
        }
    }

    public void OriginSetPos(Vector3 d) {
        transform.localPosition = d;
        oldPos = d;
    }

    protected virtual void updatePosition() {
        transform.localPosition = originUpdate.PtNewPosition(transform.localPosition);
    }
}
