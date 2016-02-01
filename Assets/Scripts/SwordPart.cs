using UnityEngine;
using System.Collections;

public class SwordPart : MonoBehaviour
{
    public int rarity;
    public float weight;
    public float damage;
    public Node[] nodePoints;
    public float truex = 0.0f;
    public float truey = 0.0f;
    public Node consumedNode;

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
        float a1 = Mathf.Atan2(nodePoints[myPoint].dir.y, nodePoints[myPoint].dir.x);
        float a2 = Mathf.Atan2(attachPoint.dir.y, attachPoint.dir.x);
        float angle = a2 - a1 + Mathf.PI;
        transform.localEulerAngles = new Vector3(0, 0, angle * 180 / Mathf.PI);

        float px = nodePoints[myPoint].pos.x;
        float py = nodePoints[myPoint].pos.y;
        float length = Mathf.Sqrt((px * px) + (py * py));
        float a3 = Mathf.Atan2(py, px);
        float transx = length * Mathf.Cos(a3 + angle);
        float transy = length * Mathf.Sin(a3 + angle);

        transform.localPosition = new Vector3(attachPoint.pos.x - transx, attachPoint.pos.y - transy);
        Vector3 localrot = sword.transform.localEulerAngles;
        sword.transform.localEulerAngles = new Vector3(0, 0, 0);
        Vector3 diff = transform.position - sword.transform.position;
        truey = diff.y;
        sword.transform.localEulerAngles = localrot;
    }
}
