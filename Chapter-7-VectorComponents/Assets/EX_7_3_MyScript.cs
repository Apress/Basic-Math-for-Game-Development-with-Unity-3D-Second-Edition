using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_7_3_MyScript : MonoBehaviour
{
    public GameObject Po = null;    // Origin of axis frame
    public GameObject Pt = null;    // x-direction of frame
    public GameObject Pz = null;    // z-direction of frame

    public GameObject P1 = null;    // Position for manipulation 
    public GameObject P2 = null;    // V from P1
    public GameObject Pr = null;    // From derived vector
    public float vx = 3.0f; // Component values
    public float vy = 2.0f;
    public float vz = 1.0f;
    
    public bool DrawCurrentFrame = true;
    public bool DrawComponents = true; // Draw toggles
    public bool DrawCartesian = false;
    public bool VectorFromP1P2 = true;

    #region For visualizing the vectors
    private MyVector DrawP1, DrawP2, DrawCP1, DrawCP2, VP12;
    private MyAxisFrame DrawFrame, DrawCFrame;
    private MyShowComponents DrawComp, DrawCComp;
    private MyLineSegment CXLine, CYLine, CZLine, XLine, YLine, ZLine;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(P1 != null);   // Verify proper setting in the editor
        Debug.Assert(P2 != null); 
        Debug.Assert(Pr != null);
        Debug.Assert(Po != null);
        Debug.Assert(Pt != null);
        Debug.Assert(Pz != null);
        
        #region For visualizing the vectors
        DrawP1 = new MyVector {
            VectorColor = Color.black,
            VectorAt = P1.transform.localPosition
        };
        DrawP2 = new MyVector {
            VectorColor = Color.black,
            VectorAt = P2.transform.localPosition
        };
        DrawCP1 = new MyVector {
            VectorColor = Color.grey,
            VectorAt = P1.transform.localPosition
        };
        DrawCP2 = new MyVector {
            VectorColor = Color.grey,
            VectorAt = P2.transform.localPosition
        };
        VP12 = new MyVector {
            VectorColor = Color.white,
        };
        // To support visualizing the vectors
        DrawFrame = new MyAxisFrame {
            PlaneColor = Color.red
        };
        DrawCFrame = new MyAxisFrame {
            PlaneColor = Color.white
        };
        DrawComp = new MyShowComponents();
        DrawCComp = new MyShowComponents();

        CXLine = new MyLineSegment {
            VectorColor = Color.red,
            LineWidth = 0.08f
        };
        CYLine = new MyLineSegment {
            VectorColor = Color.green,
            LineWidth = 0.08f
        };
        CZLine = new MyLineSegment {
            VectorColor = Color.blue,
            LineWidth = 0.08f
        };

        XLine = new MyLineSegment {
            VectorColor = Color.red,
            LineWidth = 0.05f
        };
        YLine = new MyLineSegment {
            VectorColor = Color.green,
            LineWidth = 0.05f
        };
        ZLine = new MyLineSegment {
            VectorColor = Color.blue,
            LineWidth = 0.05f
        };


        DrawFrame.At = Vector3.zero;
        DrawFrame.SetFrame(Vector3.right, Vector3.up, Vector3.forward);

        DrawCFrame.At = Vector3.zero;
        DrawCFrame.SetFrame(Vector3.right, Vector3.up, Vector3.forward);

        var sv = UnityEditor.SceneVisibilityManager.instance;
        sv.DisablePicking(Pr, true);

        P2.transform.localPosition = P1.transform.localPosition + new Vector3(vx, vy, vz);
        Pr.transform.localPosition = P2.transform.localPosition;
        #endregion 
    }

    // Update is called once per frame
    void Update()
    {
        // Step 1: Drive the axis frame
        Vector3 origin = Po.transform.localPosition;
        Vector3 Vt = (Pt.transform.localPosition - origin);
        Vector3 zDir = (Pz.transform.localPosition - origin).normalized;
        Vector3 yDir = Vector3.Cross(zDir, Vt).normalized;
        Vector3 xDir = Vector3.Cross(yDir, zDir).normalized;
        
        // Step 2: Compute vector components if necessary
        if (VectorFromP1P2) {
            Vector3 V1 = P1.transform.localPosition - origin;
            float vx1 = Vector3.Dot(V1, xDir);
            float vy1 = Vector3.Dot(V1, yDir);
            float vz1 = Vector3.Dot(V1, zDir);

            Vector3 V2 = P2.transform.localPosition - origin;
            float vx2 = Vector3.Dot(V2, xDir);
            float vy2 = Vector3.Dot(V2, yDir);
            float vz2 = Vector3.Dot(V2, zDir);

            // Difference of the P1 and P2 components
            vx = vx2 - vx1;
            vy = vy2 - vy1;
            vz = vz2 - vz1;
        } 

        Debug.Log("Component values: vx=" + vx + " vy=" + vy + " vz=" + vz);

        // Step 3: compute the vector and position for P2
        Vector3 V = vx * xDir + vy * yDir + vz * zDir;
        // Derive Pr position from computed vector
        Pr.transform.localPosition = P1.transform.localPosition + V;
        // P1.transform.localPosition += 0.001f * V.normalized;
        // What does the above do?

        #region  For visualizing the vectors
        // Make sure axis passes through the origin
        Po.SetActive(DrawCurrentFrame);
        Pt.SetActive(DrawCurrentFrame);
        Pz.SetActive(DrawCurrentFrame);

        P2.SetActive(VectorFromP1P2);           
        if (!VectorFromP1P2)
            P2.transform.localPosition = Pr.transform.localPosition;

        // User Frame
        DrawP1.VectorFromTo(origin, P1.transform.localPosition);
        DrawP1.DrawVector = DrawCurrentFrame;

        DrawP2.VectorFromTo(origin, Pr.transform.localPosition);
        DrawP2.DrawVector = DrawCurrentFrame;

        DrawFrame.At = origin;
        DrawFrame.SetFrame(xDir, yDir, zDir);
        DrawFrame.DrawAxisFrame = DrawCurrentFrame;
        DrawComp.DrawComponentsTo(DrawCurrentFrame, Po.transform.localPosition, DrawFrame);

        VP12.VectorFromTo(P1.transform.localPosition, Pr.transform.localPosition);

        XLine.VectorFromTo(P1.transform.localPosition, P1.transform.localPosition+vx*xDir);
        YLine.VectorFromTo(P1.transform.localPosition+vx*xDir, P1.transform.localPosition+vx*xDir+vy*yDir);
        ZLine.VectorFromTo(P1.transform.localPosition+vx*xDir+vy*yDir, P1.transform.localPosition+V);
        XLine.DrawVector = DrawComponents;
        YLine.DrawVector = DrawComponents;
        ZLine.DrawVector = DrawComponents;

        DrawCP1.VectorFromTo(Vector3.zero, P1.transform.localPosition);
        DrawCP2.VectorFromTo(Vector3.zero, Pr.transform.localPosition);
        DrawCFrame.At = Vector3.zero;
        DrawCFrame.SetFrame(Vector3.right, Vector3.up, Vector3.forward);
        DrawCComp.DrawComponentsTo(DrawCartesian, Vector3.zero, DrawCFrame);
        CXLine.VectorFromTo(P1.transform.localPosition, P1.transform.localPosition+V.x*Vector3.right);
        CYLine.VectorFromTo(P1.transform.localPosition+V.x*Vector3.right, P1.transform.localPosition+V.x*Vector3.right+V.y*Vector3.up);
        CZLine.VectorFromTo(P1.transform.localPosition+V.x*Vector3.right+V.y*Vector3.up, P1.transform.localPosition+V);
        DrawCP1.DrawVector = DrawCartesian;
        DrawCP2.DrawVector = DrawCartesian;
        DrawCFrame.DrawAxisFrame = DrawCartesian;
        CXLine.DrawVector = DrawCartesian;
        CYLine.DrawVector = DrawCartesian;
        CZLine.DrawVector = DrawCartesian;
        #endregion
    }
}
