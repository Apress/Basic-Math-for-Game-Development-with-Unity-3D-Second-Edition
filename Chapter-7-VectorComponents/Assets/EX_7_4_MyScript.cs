using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_7_4_MyScript : MonoBehaviour
{
    // for defining the frame
    public GameObject Po = null;    // Origin of axis frame
    public GameObject Pt = null;    // x-direction of frame
    public GameObject Pz = null;    // z-direction of frame
    public GameObject P = null;    // 
    public Vector3 P1Components = Vector3.zero; // Begin Position Components
    public Vector3 P2Components = Vector3.one;  // Destination position Components

    public bool DrawAxisFrame = true;
    public bool MotionInAxisFrame = false;

    private float Traveled = 0f;    
    private const float kSpeed = 0.01f * 60f;

    #region For visualizing the vectors
    private GameObject ShowP2 = null;    // 
    private GameObject ShowP1 = null;    // 
    private MyVector DrawP;
    private MyLineSegment DrawP1P2;
    private MyAxisFrame DrawFrame;
    private MyShowComponents DrawComp;
    private MyLineSegment lx, ly, lz;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(P != null); // Verify proper setting in the editor
        Debug.Assert(Po != null); 
        Debug.Assert(Pt != null);
        Debug.Assert(Pz != null);

        #region For visualizing the vectors
        DrawP1P2 = new MyLineSegment {
            VectorColor = Color.white,
            VectorAt = Vector3.zero,
            LineWidth = 0.05f
        };
        
        // To support visualizing the vectors
        DrawFrame = new MyAxisFrame {
            PlaneColor = Color.red
        };
        DrawComp = new MyShowComponents();

        DrawFrame.At = Po.transform.position;
        Vector3 Vt = (Pt.transform.position - Po.transform.position).normalized;
        Vector3 Vz = (Pz.transform.position - Po.transform.position).normalized;
        Vector3 Vy = Vector3.Cross(Vz, Vt);
        Vector3 Vx = Vector3.Cross(Vy, Vz);
        DrawFrame.SetFrame(Vy, Vy, Vz);

        ShowP1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ShowP1.GetComponent<Renderer>().material.color = Color.black;
        ShowP1.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        ShowP1.transform.parent = GameObject.Find("zIgnoreThisObject").transform;

        ShowP2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ShowP2.GetComponent<Renderer>().material.color = Color.black;
        ShowP2.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        ShowP2.transform.parent = GameObject.Find("zIgnoreThisObject").transform;

        lx = new MyLineSegment {
            LineWidth = 0.05f,
            VectorColor = Color.red
        };
        ly = new MyLineSegment {
            LineWidth = 0.05f,
            VectorColor = Color.green
        };
        lz = new MyLineSegment {
            LineWidth = 0.05f,
            VectorColor = Color.blue
        };
        var sv = UnityEditor.SceneVisibilityManager.instance;
        sv.DisablePicking(ShowP2, true);
        sv.DisablePicking(ShowP1, true);
        sv.DisablePicking(P, true);
        #endregion 
    }

    // Update is called once per frame
    void Update()
    {
        // Parameters of an axis frame
        Vector3 origin, xDir, yDir, zDir;

        // Step 1: Set up the axis frame
        if (MotionInAxisFrame) { 
            // Derive the axis frame
            origin = Po.transform.localPosition;
            Vector3 Vt = (Pt.transform.localPosition - origin);
            zDir = (Pz.transform.localPosition - origin).normalized;
            yDir = Vector3.Cross(zDir, Vt).normalized;
            xDir = Vector3.Cross(yDir, zDir).normalized;
        } else {    
            // Default Cartesian axis frame
            origin = Vector3.zero;
            xDir = Vector3.right;
            yDir = Vector3.up;
            zDir = Vector3.forward;
        }       

        // Step 2: direction and distance traveled
        Vector3 Vc = P2Components - P1Components; 
        Traveled += kSpeed * Time.deltaTime; //         
        if (Traveled > Vc.magnitude)
            Traveled = 0f; // restart
        Vector3 Tc = Traveled * Vc.normalized;

        // Step 3: components and coordinate of P
        Vector3 Pc = P1Components + Tc;
        P.transform.localPosition = origin + Pc.x * xDir + Pc.y * yDir + Pc.z * zDir;

        #region Compute motion vector based on component
        /*
            Vector3 P1 = origin + P1Components.x * xDir + P1Components.y * yDir + P1Components.z * zDir;
            Vector3 P2 = origin + P2Components.x * xDir + P2Components.y * yDir + P2Components.z * zDir;
            V = P2 - P1;
            P.transform.localPosition = P1 + Traveled * V.normalized;
        */
        #endregion 

        #region  For visualizing the vectors
        {
        Vector3 Pd = origin + P2Components.x * xDir + P2Components.y * yDir + P2Components.z * zDir;
        Vector3 Ps = origin + P1Components.x * xDir + P1Components.y * yDir + P1Components.z * zDir;
        ShowP2.transform.localPosition = Pd;
        ShowP1.transform.localPosition = Ps;
        DrawP1P2.VectorFromTo(Ps, Pd);

        Po.SetActive(MotionInAxisFrame);
        Pt.SetActive(MotionInAxisFrame);
        Pz.SetActive(MotionInAxisFrame);
        {
            Vector3 p1 = Ps;
            Vector3 p2 = p1;
            Vector3 vel = Pd - p1; // vDir.normalized * kSpeed * 500f;
            p2 += Vector3.Dot(vel,xDir) * xDir;
            lx.VectorFromTo(p1, p2);
            p1 = p2;
            p2 +=  Vector3.Dot(vel,yDir) * yDir;
            ly.VectorFromTo(p1, p2);
            p1 = p2;
            p2 +=  Vector3.Dot(vel,zDir) * zDir;
            lz.VectorFromTo(p1, p2);
            lx.DrawVector = DrawAxisFrame;
            ly.DrawVector = DrawAxisFrame;
            lz.DrawVector = DrawAxisFrame;
        }

        DrawFrame.At = origin;
        DrawFrame.SetFrame(xDir, yDir, zDir);
        DrawFrame.DrawAxisFrame = DrawAxisFrame;
        DrawComp.DrawComponentsTo(DrawAxisFrame, P.transform.localPosition, DrawFrame);
        }
        #endregion
    }
}