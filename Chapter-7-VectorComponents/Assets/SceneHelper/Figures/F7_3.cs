using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F7_3 : MonoBehaviour
{
    public GameObject Po = null;    // Origin of the reference frame
    public GameObject Px = null;    // x-axis position of the reference frame
    public GameObject Pz = null;    // z-axis position of the reference frame

    public GameObject P = null;     // Position to show components for
    public GameObject Pt = null ;   // Position  from axis frame components
    
    private Vector3 iV = new Vector3(1f, 0f, 0f);  // unit vector in x-direction
    private Vector3 jV = new Vector3(0f, 1f, 0f);  // unit vector in y-direction
    private Vector3 kV = new Vector3(0f, 0f, 1f);  // unit vector in z-direction
    public bool DrawDefaultFrame = true;
    public bool DrawDerivedFrame = true;

    #region For visualizing the vectors
    private MyVector DrawDefaultP;
    private MyAxisFrame DefaultFrameToDraw;
    private MyShowComponents DrawDefaultComp;
    private MyVector DrawP;
    private MyVector ToX;
    private MyAxisFrame RefFrameToDraw;
    private MyShowComponents DrawComp;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(P != null);   // Verify proper setting in the editor
        Debug.Assert(Pt != null);
        Debug.Assert(Po != null);
        Debug.Assert(Px != null);
        Debug.Assert(Pz != null);
        
        #region For visualizing the vectors
        DrawP = new MyVector {
            VectorColor = Color.black,
            VectorAt = P.transform.localPosition
        };
        ToX = new MyVector {
            VectorColor = Color.black,
            VectorAt = Px.transform.localPosition
        };
        // To support visualizing the vectors
        RefFrameToDraw = new MyAxisFrame {
            PlaneColor = Color.white
        };
        DrawComp = new MyShowComponents();

        RefFrameToDraw.At = Po.transform.localPosition;
        Vector3 Vt = (Px.transform.localPosition - Po.transform.localPosition).normalized;
        Vector3 Vz = (Pz.transform.localPosition - Po.transform.localPosition).normalized;
        Vector3 Vy = Vector3.Cross(Vz, Vt);
        Vector3 Vx = Vector3.Cross(Vy, Vz);
        RefFrameToDraw.SetFrame(Vx, Vy, Vz);

        // Default original 
        DrawDefaultP = new MyVector {
            VectorColor = Color.white
        };
        // To support visualizing the vectors
        DefaultFrameToDraw = new MyAxisFrame();
        DrawDefaultComp = new MyShowComponents();

        DefaultFrameToDraw.At = Vector3.zero;
        DefaultFrameToDraw.SetFrame(Vector3.right, Vector3.up, Vector3.forward);
        #endregion 
    }

    // Update is called once per frame
    void Update()
    {
        // Show/hide the non-collinear positions
        Po.SetActive(DrawDerivedFrame);
        Px.SetActive(DrawDerivedFrame);
        Pz.SetActive(DrawDerivedFrame);

        // Compute the axis frame and the associated components
        Vector3 origin = Po.transform.localPosition;
        Vector3 Vt = (Px.transform.localPosition - origin).normalized;
        Vector3 Vz = (Pz.transform.localPosition - origin).normalized;
        Vector3 Vy = Vector3.Cross(Vz, Vt);
        Vector3 Vx = Vector3.Cross(Vy, Vz);  // make sure z is perpendicular to x/y
        Vector3 V = P.transform.localPosition - origin;

        float vx = Vector3.Dot(V, Vx);
        float vy = Vector3.Dot(V, Vy);
        float vz = Vector3.Dot(V, Vz);

        Pt.transform.localPosition = origin + vx * Vx + vy * Vy + vz * Vz;
        Pt.SetActive(false);
        P.SetActive(false);

        #region  For visualizing the vectors
        // Make sure axis passes through the origin

        // User Frame
        DrawP.VectorFromTo(origin, P.transform.localPosition);
        DrawP.DrawVector = false;

        ToX.VectorFromTo(origin, Px.transform.localPosition);

        RefFrameToDraw.At = origin;
        RefFrameToDraw.SetFrame(Vx, Vy, Vz);
        RefFrameToDraw.DrawAxisFrame = DrawDerivedFrame;
        DrawComp.DrawComponentsTo(DrawDerivedFrame, P.transform.localPosition, RefFrameToDraw);

        // Default frame
        DrawDefaultP.VectorFromTo(Vector3.zero, P.transform.localPosition);
        DrawDefaultP.DrawVector = DrawDefaultFrame;
        DefaultFrameToDraw.DrawAxisFrame = DrawDefaultFrame;
        DrawDefaultComp.DrawComponentsTo(DrawDefaultFrame, P.transform.localPosition, DefaultFrameToDraw);
        #endregion
    }
}