using UnityEngine;
public class MyShowComponents {
    // Show with max of 20 spheres along each axis
    private const int kMaxAlongAxis = 20;
    private const float kSphereSize = 0.1f;

    private const float kMinDistance = 0.2f;
    private const float kMaxDistance = kMinDistance * kMaxAlongAxis;

    private MyVector X, Y, Z;
    
    public MyShowComponents() {
        X = new MyVector {
            VectorColor = Color.red
        };
        Y = new MyVector {
            VectorColor = Color.green
        };
        Z = new MyVector {
            VectorColor = Color.blue
        };
    }


    public void DrawComponentsTo(bool show, Vector3 p, MyAxisFrame frame) {
        DrawComponents(show);

        if (!show)
            return; 

        Vector3 v = p - frame.At;
        float x = Vector3.Dot(v, frame.xDir);
        float y = Vector3.Dot(v, frame.yDir);
        float z = Vector3.Dot(v, frame.zDir);

        X.VectorFromTo(frame.At, frame.At+x*frame.xDir);
        Y.VectorFromTo(frame.At+x*frame.xDir, frame.At+x*frame.xDir+y*frame.yDir);
        Z.VectorFromTo(frame.At+x*frame.xDir+y*frame.yDir, frame.At+x*frame.xDir+y*frame.yDir+z*frame.zDir);
    }

    private void DrawComponents(bool b) {
            X.DrawVector = b;
            Y.DrawVector = b;
            Z.DrawVector = b;
    }
}
