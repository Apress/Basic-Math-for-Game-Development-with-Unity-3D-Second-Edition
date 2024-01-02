using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F7_1 : MonoBehaviour
{
    public GameObject P = null;     // Position to show components for
    public bool DrawPositionVector = true;
    public bool DrawAxisFrame = true;
    public bool DrawComponents = false;

    private Vector3 iV = new Vector3(1f, 0f, 0f);  // unit vector in x-direction
    private Vector3 jV = new Vector3(0f, 1f, 0f);  // unit vector in y-direction
    private Vector3 kV = new Vector3(0f, 0f, 1f);  // unit vector in z-direction
    

    #region For visualizing the vectors
    private MyVector ShowP;

    private MyAxisFrame ShowFrame;
    private MyShowComponents ShowComp;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(P != null);   // Verify proper setting in the editor
        
        #region For visualizing the vectors
        ShowP = new MyVector {
            VectorColor = Color.black
        };
        // To support visualizing the vectors
        ShowFrame = new MyAxisFrame();
        ShowComp = new MyShowComponents();

        ShowFrame.At = Vector3.zero;
        ShowFrame.SetFrame(Vector3.right, Vector3.up, Vector3.forward);
        #endregion 
    }

    // Update is called once per frame
    void Update()
    {
        // 1. position and  the position vector
        Vector3 Po = Vector3.zero;
        Vector3 v = P.transform.localPosition - Po;


        #region  For visualizing the vectors
        // Make sure axis passes through the origin
        ShowP.VectorFromTo(Vector3.zero, P.transform.localPosition);

        ShowP.DrawVector = DrawPositionVector;
        ShowFrame.DrawAxisFrame = DrawAxisFrame;

        ShowComp.DrawComponentsTo(DrawComponents, P.transform.localPosition, ShowFrame);
        #endregion
    }
}
