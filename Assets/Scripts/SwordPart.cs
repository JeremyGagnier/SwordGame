using UnityEngine;
using System.Collections;

public class SwordPart : MonoBehaviour
{
    public int rarity;
    public FVector position;
    public FInt rotation;
    public FInt weight;
    public FInt damage;
    public Node[] nodePoints;
    public FInt truex = FInt.Zero();
    public FInt truey = FInt.Zero();
    public Node consumedNode;

    // Recursively calculates the GLOBAL position of a sword part
    public static FVector Position(Sword s, SwordPart p)
    {
        // TODO: Implement the recursive calculation
        // BREAKS: Sword part collisions
        return new FVector();
    }

    void Awake()
    {
        for (int i = 0; i < nodePoints.Length; ++i)
        {
            nodePoints[i].parent = this;
        }
    }

    public void Attach(Node attachPoint, int myPoint, GameObject sword)
    {
        // Attach this sword part by making the node point at myPoint
        // equivelant to the attach point
        FInt a1 = FInt.Atan(nodePoints[myPoint].dir.x, nodePoints[myPoint].dir.y);
        FInt a2 = FInt.Atan(attachPoint.dir.x, attachPoint.dir.y);
        FInt angle = a2 - a1 + new FInt(3.1415f);
        rotation = angle;

        FInt px = nodePoints[myPoint].pos.x;
        FInt py = nodePoints[myPoint].pos.y;
        FInt length = FInt.Sqrt((px * px) + (py * py));
        FInt a3 = FInt.Atan(px, py);
        FInt transx = length * FInt.Cos(a3 + angle);
        FInt transy = length * FInt.Sin(a3 + angle);
        position = new FVector(attachPoint.pos.x - transx, attachPoint.pos.y - transy);

        transform.localEulerAngles = new Vector3(0, 0, rotation.ToFloat() * 180.0f / Mathf.PI);
        transform.localPosition = new Vector3(position.x.ToFloat(), position.y.ToFloat());

        // TODO: Find truey via recursive transformations
        // BREAKS: Proper sword construction
        /*
        Vector3 localrot = sword.transform.localEulerAngles;
        sword.transform.localEulerAngles = new Vector3(0, 0, 0);
        Vector3 diff = transform.position - sword.transform.position;
        truey = diff.y;
        sword.transform.localEulerAngles = localrot;
         */
    }
}
