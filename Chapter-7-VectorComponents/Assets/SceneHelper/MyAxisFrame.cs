using UnityEngine;
public class MyAxisFrame {
    private MyVector X, Y, Z;
    private MyXZPlane Plane;
    
    public float Size {
        set { 
            X.Magnitude = value;
            Y.Magnitude = value;
            Z.Magnitude = value;
            UpdatePlane();
         }
        get { return X.Magnitude; }
    }

    public Color PlaneColor { 
        set { Plane.PlaneColor = value; }
    }

    public Vector3 At {
        set {
            X.VectorAt = value;
            Y.VectorAt = value;
            Z.VectorAt = value;
            UpdatePlane();
        }
        get { return X.VectorAt; }
    }

    // Assume x/y/zDir are proper: orthonormal
    public void SetFrame(Vector3 xDir, Vector3 yDir, Vector3 zDir) {
        X.Direction = xDir;
        Y.Direction = yDir;
        Z.Direction = zDir;
        UpdatePlane();
    }

    public Vector3 xDir { get {return X.Direction;}}
    public Vector3 yDir { get {return Y.Direction;}}
    public Vector3 zDir { get {return Z.Direction;}}

    public MyAxisFrame() {
        X = new MyVector {
            VectorColor = Color.red,
            Magnitude = 2f,
            VectorAt = Vector3.zero
        };
        Y = new MyVector {
            VectorColor = Color.green,
            Magnitude = 2f,
            VectorAt = Vector3.zero
        };
        Z = new MyVector {
            VectorColor = Color.blue,
            Magnitude = 2f,
            VectorAt = Vector3.zero
        };
        Plane = new MyXZPlane {
            PlaneColor = Color.white,
            XSize = 2f,
            ZSize = 2f,
            Center = Vector3.one
        };
    }

    public bool DrawAxisFrame {
        set {
            X.DrawVector = value;
            Y.DrawVector = value;
            Z.DrawVector = value;
            Plane.DrawPlane = value;
        }
    }

    private void UpdatePlane() {
        Vector3 org = X.VectorAt;
        Plane.XSize = X.Magnitude * 0.5f;
        Plane.ZSize = Z.Magnitude * 0.5f;
        Plane.Center = org + Plane.XSize * 5.0f * X.Direction + Plane.ZSize * 5.0f * Z.Direction;
        Plane.SetAlign(Z.Direction, Y.Direction);
    }
}