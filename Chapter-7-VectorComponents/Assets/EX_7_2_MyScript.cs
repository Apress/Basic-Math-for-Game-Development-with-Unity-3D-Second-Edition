using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_7_2_MyScript : MonoBehaviour
{
    public GameObject Po = null;    // Origin of the reference frame
    public GameObject Pt = null;    // x-axis position of the reference frame
    public GameObject Pz = null;    // z-axis position of the reference frame

    public GameObject P = null;     // Position to show components for
    public GameObject Pr = null;   // Position  from axis frame components

    public bool DrawCartesianFrame = true;
    public bool DrawDerivedFrame = true;

    #region For visualizing the vectors
    private MyVector DrawDefaultP;
    private MyAxisFrame DefaultFrameToDraw;
    private MyShowComponents DrawDefaultComp;
    private MyVector DrawP;
    private MyAxisFrame RefFrameToDraw;
    private MyShowComponents DrawComp;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(P != null);   // Verify proper setting in the editor
        Debug.Assert(Pr != null);
        Debug.Assert(Po != null);
        Debug.Assert(Pt != null);
        Debug.Assert(Pz != null);

        #region For visualizing the vectors
        DrawP = new MyVector
        {
            VectorColor = Color.black,
            VectorAt = P.transform.localPosition
        };
        // To support visualizing the vectors
        RefFrameToDraw = new MyAxisFrame
        {
            PlaneColor = Color.white
        };
        DrawComp = new MyShowComponents();

        RefFrameToDraw.At = Po.transform.localPosition;
        Vector3 Vt = (Pt.transform.localPosition - Po.transform.localPosition).normalized;
        Vector3 Vz = (Pz.transform.localPosition - Po.transform.localPosition).normalized;
        Vector3 Vy = Vector3.Cross(Vz, Vt);
        Vector3 Vx = Vector3.Cross(Vy, Vz);
        RefFrameToDraw.SetFrame(Vx, Vy, Vz);

        // Default original 
        DrawDefaultP = new MyVector
        {
            VectorColor = Color.white
        };
        // To support visualizing the vectors
        DefaultFrameToDraw = new MyAxisFrame();
        DrawDefaultComp = new MyShowComponents();

        DefaultFrameToDraw.At = Vector3.zero;
        DefaultFrameToDraw.SetFrame(Vector3.right, Vector3.up, Vector3.forward);

        var sv = UnityEditor.SceneVisibilityManager.instance;
        sv.DisablePicking(Pr, true);
        #endregion 
    }

    // Update is called once per frame
    void Update()
    {
        // Step 1: Derive the axis frame 
        Vector3 origin = Po.transform.localPosition;
        Vector3 Vt = Pt.transform.localPosition - origin;
        Vector3 zDir = (Pz.transform.localPosition - origin).normalized;
        Vector3 yDir = Vector3.Cross(zDir, Vt).normalized;
        Vector3 xDir = Vector3.Cross(yDir, zDir).normalized;

        // Step 2: Position vector and the components
        Vector3 V = P.transform.localPosition - origin;
        float vx = Vector3.Dot(V, xDir);
        float vy = Vector3.Dot(V, yDir);
        float vz = Vector3.Dot(V, zDir);

        // Step 3: Compute Pt position from the components
        Pr.transform.localPosition = origin + vx * xDir + vy * yDir + vz * zDir;
        

        #region  For visualizing the vectors
        // Show/hide the non-collinear positions
        Po.SetActive(DrawDerivedFrame);
        Pt.SetActive(DrawDerivedFrame);
        Pz.SetActive(DrawDerivedFrame);

        Pr.SetActive(DrawDerivedFrame);

        // User Frame
        DrawP.VectorFromTo(origin, P.transform.localPosition);
        DrawP.DrawVector = DrawDerivedFrame;

        RefFrameToDraw.At = origin;
        RefFrameToDraw.SetFrame(xDir, yDir, zDir);
        RefFrameToDraw.DrawAxisFrame = DrawDerivedFrame;
        DrawComp.DrawComponentsTo(DrawDerivedFrame, P.transform.localPosition, RefFrameToDraw);

        // Default frame
        DrawDefaultP.VectorFromTo(Vector3.zero, P.transform.localPosition);
        DrawDefaultP.DrawVector = DrawCartesianFrame;
        DefaultFrameToDraw.DrawAxisFrame = DrawCartesianFrame;
        DrawDefaultComp.DrawComponentsTo(DrawCartesianFrame, P.transform.localPosition, DefaultFrameToDraw);
        #endregion
    }
}