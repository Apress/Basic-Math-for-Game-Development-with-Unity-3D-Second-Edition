using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameUpdatePz : FrameUpdatePt
{
    // on Pz
    protected override void updatePosition() {
        transform.localPosition = originUpdate.PzNewPosition(transform.localPosition);
    }
}
