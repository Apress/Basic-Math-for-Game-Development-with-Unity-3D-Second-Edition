using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_9_1_MyScript : MonoBehaviour
{
    // Aim System
    public GameObject Pb = null;
    public GameObject Pc = null;
    public float Aspeed = 2.0f;           // Agend Speed

    // Agent Support
    public bool MoveAgent = false;
    public float AgentSentInterval = 4f;  // Every so many seconds will re-send
    public GameObject Pa = null;
    private Vector3 Adir = Vector3.zero;
    private float AgentSinceTime = 100f;    // Keep track on when to send again

    // Hero
    public GameObject Ph = null;
    public bool HeroXMotion = true;
    public bool HeroYMotion = true;
    private Vector3 Vh = Vector3.zero;
    private float HeroSpeed = 0.5f; 
    private const float kHeroZMotionRange = 1f;
    
    //  Plane
    public bool ShowAxisFrame = false;
    public float D = -6.7f; // The distance to the plane
    public Vector3 Vn;     // Normal vector of reflection plane
    public GameObject Pn;  // Location where the plane center is
    
    // Shadow
    public bool CastShadow = true;
    public GameObject Ps;  // Location of Shadow of Agent
    
    // Reflection
    public bool DoReflection = true;
    public GameObject Pon; // Collision point of Agent
    public GameObject Pr;  // Reflection of current Agent position

    // Treasure Collision
    public bool CollideTreasure = true;
    public GameObject Pt;   // Treasure position
    public float Tr = 2f;   // Treasure radius
    

    public bool ShowDebugLines = true;

    #region For visualization
    // AimSystem
    private MyVector ShowAim;
    
    MyVector ShowVrN;
    // MyLineSegment ShowProj, ShowToPn, ShowFromPn;
    MyXZPlane ShowReflectionPlane;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(Pa != null);     // Verify proper setting in the editor
        Debug.Assert(Pb != null);
        Debug.Assert(Pc != null);
        Debug.Assert(Pn != null);
        Debug.Assert(Ps != null);
        Debug.Assert(Pon != null);
        Debug.Assert(Pr != null);
        Debug.Assert(Pt != null);
        Debug.Assert(Ph != null);

        #region For visualization
        // To support visualizing the vectors
        ShowAim = new MyVector
        {
            VectorColor = new Color(1.0f, 0f, 0f, 1.0f)
        };

        ShowVrN = new MyVector
        {
            VectorColor = Color.black
        };
        ShowReflectionPlane = new MyXZPlane
        {
            XSize = 2f,
            ZSize = 2f,
            PlaneColor = new Color(0.8f, 1.0f, 0.8f, 1.0f)
        };
        /*
        ShowProj = new MyLineSegment
        {
            VectorColor = Color.black,
            LineWidth = 0.02f
        };
        ShowToPn = new MyLineSegment
        {
            VectorColor = Color.red,
            LineWidth = 0.02f
        };
        ShowFromPn = new MyLineSegment
        {
            VectorColor = Color.red,
            LineWidth = 0.02f
        }; */
        var sv = UnityEditor.SceneVisibilityManager.instance;
        sv.DisablePicking(Pa, true);
        sv.DisablePicking(Pb, true);
        sv.DisablePicking(Pn, true);
        sv.DisablePicking(Ps, true);
        sv.DisablePicking(Pon, true);
        sv.DisablePicking(Pr, true);
        sv.DisablePicking(Ph, true);
        #endregion 
    }

    // Update is called once per frame
    void Update()
    {
        #region Step 0: Initial error checking
        Debug.Assert((Pc.transform.localPosition - Pb.transform.localPosition).magnitude > float.Epsilon);
        Debug.Assert(Vn.magnitude > float.Epsilon);        
        Debug.Assert(Aspeed > float.Epsilon);
        Debug.Assert(Tr > float.Epsilon);
        // recoveries from the errors
        if ((Pc.transform.localPosition - Pb.transform.localPosition).magnitude < float.Epsilon)
            Pc.transform.localPosition = Pb.transform.localPosition - Vector3.forward;
        if (Vn.magnitude < float.Epsilon)
            Vn = Vector3.forward;
        if (Aspeed < float.Epsilon)
            Aspeed = 0.01f;
        if (Tr < float.Epsilon)
            Tr = 0.01f;
        #endregion

        #region Step 1: The Aiming System
        Vector3 aDir = Pc.transform.localPosition - Pb.transform.localPosition;
        aDir.Normalize(); // assuming the two are not located at the same point
        Pc.transform.localPosition = Pb.transform.localPosition + Aspeed * aDir;
        if (!MoveAgent) {  // only affect the agent if it is not moving
            Pa.transform.localPosition = Pb.transform.localPosition + 2 * Aspeed * aDir;
            Pa.transform.localRotation = Quaternion.LookRotation(aDir, Vector3.up);
            Adir = aDir;
        }
        #endregion

        #region Step 2: The Agent
        if (MoveAgent)
        {
            Pa.transform.localPosition += Aspeed * Time.deltaTime * Adir;
            AgentSinceTime += Time.deltaTime;
            if (AgentSinceTime > AgentSentInterval)
            {  // Time to re-send the agent from base
                Pa.transform.localPosition = Pc.transform.localPosition;
                Adir = aDir;  
                    // Adir is the front direction (z)
                    // Up is always Vector3.up (0, 1, 0)
                Pa.transform.localRotation = Quaternion.LookRotation(Adir, Vector3.up);
                AgentSinceTime = 0f;
            }
        }
        if (ShowDebugLines)
            Debug.DrawLine(Pa.transform.localPosition, Pa.transform.localPosition + 20f * Adir, Color.red);
        #endregion

        #region Step 3: The Hero motion
        // Hero follows agent (Pa) axis frame
        Vector3 po = Pa.transform.localPosition;
        Vector3 vx = Pa.transform.right;
        Vector3 vy = Pa.transform.up;
        Vector3 vz = Pa.transform.forward; // 

        Vh.z += HeroSpeed * Time.deltaTime;  // moved
        if (Mathf.Abs(Vh.z) > kHeroZMotionRange) {
            Vh.z = (Vh.z>0f) ? 1f : -1f;
            HeroSpeed = -HeroSpeed;
        }

        if (HeroYMotion)
            Vh.y = Mathf.Abs(Mathf.Cos(Mathf.PI * Vh.z));

        if (HeroXMotion)
            Vh.x = Mathf.Sin(Mathf.PI * Vh.z);       

        Vector3 vhc = Vh.x * vx + Vh.y * vy + Vh.z * vz;
        Ph.transform.localPosition = po + vhc;
        Ph.transform.localRotation = Pa.transform.localRotation;
        #endregion 

        #region Step 4: The Plane and  in-front/parallel checks
        // Plane equation
        Vn.Normalize(); 
        Pn.transform.localPosition = D * Vn;

        // agent position checks
        float paDotVn = Vector3.Dot(Pa.transform.localPosition, Vn);
        bool infrontOfPlane = (paDotVn > D);

        // Agent motion direction checks
        float aDirDotVn = Vector3.Dot(Adir, Vn);
        bool isApproaching = (aDirDotVn < 0f);
        bool notParallel = (Mathf.Abs(aDirDotVn) > float.Epsilon);                
        #endregion

        #region Step 5: The Shadow
        Ps.SetActive(CastShadow && infrontOfPlane);
        if (CastShadow && infrontOfPlane)
        {
            float h = Vector3.Dot(Pa.transform.localPosition, Vn);
            Ps.transform.localPosition = Pa.transform.localPosition - (h-D) * Vn;

            if (ShowDebugLines)
                Debug.DrawLine(Pa.transform.localPosition, Ps.transform.localPosition, Color.black);
        }
        #endregion

        #region Step 6: The Reflection
        Pon.SetActive(DoReflection && notParallel && infrontOfPlane && isApproaching);
        Pr.SetActive(DoReflection && notParallel && infrontOfPlane && isApproaching);
        Vector3 vr = Vector3.up;  // Reflection vector
        bool vrIsValid = false;
        if (DoReflection && notParallel && isApproaching)
        {
            if (infrontOfPlane)
            {
                float d = (D - Vector3.Dot(Pa.transform.localPosition, Vn)) / aDirDotVn;
                Pon.transform.localPosition = Pa.transform.localPosition + d * Adir;
                Vector3 von = Pa.transform.localPosition - Pon.transform.localPosition; // von is simply -d*Adir
                Vector3 m = (Vector3.Dot(von, Vn) * Vn) - von;
                vr = 2 * m + von;
                Pr.transform.localPosition = Pon.transform.localPosition + vr;
                vrIsValid = true;
                if (ShowDebugLines)
                {
                    Debug.DrawLine(Pa.transform.localPosition, Pon.transform.localPosition, Color.red);
                    Debug.DrawLine(Pon.transform.localPosition, Pr.transform.localPosition, Color.red);
                }

                // if (von.magnitude < float.Epsilon) What will happen if you do this?
                if (von.magnitude < 0.1f)
                {
                    // collision!
                    Adir = vr.normalized;
                    Pa.transform.localRotation = Quaternion.LookRotation(Adir, Vector3.up);
                }
            }
            else
            {

                Debug.Log("Potential problem!: high speed Agent, missing the plane collision?");
                // What can you do?
            }
        }
        #endregion

        #region Step 7: The collision with treasure
        Pt.SetActive(DoReflection && CollideTreasure);
        Pt.transform.localScale = new Vector3(2 * Tr, 2 * Tr, 2 * Tr);  // this is the diameter
        Pt.GetComponent<Renderer>().material.color = MyDrawObject.NoCollisionColor;
        if (DoReflection && CollideTreasure && vrIsValid)
        {
            Vector3 vt = Pt.transform.localPosition - Pon.transform.localPosition;
            float dt = Vector3.Dot(vt, vr.normalized);
            if ((dt >= 0) && (dt <= vr.magnitude))
            {
                Vector3 pdt = Pon.transform.localPosition + dt * vr.normalized;
                if ((pdt - Pt.transform.localPosition).magnitude <= Tr)
                    Pt.GetComponent<Renderer>().material.color = MyDrawObject.CollisionColor;
            }
        }
        #endregion

        #region  For visualization
        AxisFrame.ShowAxisFrame = ShowAxisFrame;
        if (ShowAxisFrame && ShowDebugLines)
            Debug.DrawLine(Vector3.zero, Pn.transform.localPosition, Color.white);

        // Aiming 
        ShowAim.VectorFromTo(Pb.transform.localPosition, Pc.transform.localPosition);

        // Refleciton plane
        ShowVrN.VectorAt = Pn.transform.localPosition;
        ShowVrN.Magnitude = 2.0f;
        ShowVrN.Direction = Vn;

        ShowReflectionPlane.Center = Pn.transform.localPosition; //  + new Vector3(-4f, 0f, 0f);
        ShowReflectionPlane.PlaneNormal = Vn;
        ShowReflectionPlane.XSize = ShowReflectionPlane.ZSize = 2f;
        /*
         * Do not adjust the plane size, it is confusing.
        const float S = 3f;
        float s = S;
        if (CastShadow || DoReflection)
        {
            if (CastShadow) {
                s = (Ps.transform.localPosition - Pn.transform.localPosition).magnitude * 0.3f;
                // ShowProj.VectorFromTo(Pa.transform.localPosition, Ps.transform.localPosition);
            }
            if (DoReflection)
            {
                float t = (Pon.transform.localPosition - Pn.transform.localPosition).magnitude * 0.3f;
                if (t > s)
                    s *= 1.02f;
                else if (t < s)
                        s *= 0.98f;
                // ShowToPn.VectorFromTo(Pa.transform.localPosition, Pon.transform.localPosition);
                // ShowFromPn.VectorFromTo(Pon.transform.localPosition, Pr.transform.localPosition);
            } 
        }
        ShowReflectionPlane.XSize = ShowReflectionPlane.ZSize = s;
        */
        #endregion
    }
}