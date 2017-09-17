using UnityEngine;
using System.Collections;

public class CharCollider : MonoBehaviour {
    public FVector position;
    public FInt width;
    public FInt height;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Gizmos.DrawCube(
            new Vector3(position.x.ToFloat(), position.y.ToFloat()),
            new Vector3(width.ToFloat(), height.ToFloat()));
    }
}
