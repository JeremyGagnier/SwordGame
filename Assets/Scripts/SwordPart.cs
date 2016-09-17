using UnityEngine;

public class SwordPart : MonoBehaviour
{
    private SwordPart parent;
    
    [HideInInspector] public FInt depthInSword = 0L;
    [HideInInspector] public Node consumedNode;

    public FInt rotation;
    public Player player;
    public int damage;
    public int weight;
    public FInt radius;
    public Node[] nodePoints;

    private FVector _position;
    public FVector position
    {
        get
        {
            return GetGlobalPosition(GetGlobalRotation());
        }
        set
        {
            _position = value;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
        Gizmos.DrawSphere(new Vector3(position.x.ToFloat(), position.y.ToFloat()), radius.ToFloat());

        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        FInt myGlobalRotation = GetGlobalRotation();
        foreach(Node node in nodePoints)
        {
            FVector nodePos = new FVector(node.pos).Rotate(myGlobalRotation);
            nodePos += position;
            Gizmos.DrawSphere(new Vector3(nodePos.x.ToFloat(), nodePos.y.ToFloat()), 20);
        }
        if (parent != null)
        {
            Gizmos.color = new Color(1f, 0f, 1f, 0.5f);
            FVector consumedNodePos = new FVector(consumedNode.pos).Rotate(myGlobalRotation - rotation);
            consumedNodePos += parent.position;
            Gizmos.DrawSphere(new Vector3(consumedNodePos.x.ToFloat(), consumedNodePos.y.ToFloat()), 20);
        }
    }

    void Awake()
    {
        for (int i = 0; i < nodePoints.Length; ++i)
        {
            nodePoints[i].parent = this;
        }
    }

    public void Setup(FVector pos)
    {
        position = pos;
    }

    public FVector GetGlobalPosition(FInt globalRotation)
    {
        // The sword part is the hilt (or on the ground) if there is no parent
        if (parent == null)
        {
            // The sword part is on the ground if there is no player
            if (player == null)
            {
                return _position;
            }
            return _position + player.position;
        }
        globalRotation -= rotation;
        FVector rotatedPosition = new FVector(_position).Rotate(globalRotation);
        return rotatedPosition + parent.GetGlobalPosition(globalRotation);
    }

    public FInt GetGlobalRotation()
    {
        if (parent == null)
        {
            return rotation;
        }
        return rotation + parent.GetGlobalRotation();
    }

    public void Attach(Node attachPoint, int myPoint, GameObject sword, Player player)
    {
        this.player = player;
        parent = attachPoint.parent;

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
        transform.localPosition = new Vector3(_position.x.ToFloat(), _position.y.ToFloat());

        // TODO: Find depthInSword via recursive transformations
        // BREAKS: Proper sword construction
    }

    public void Attack()
    {
        Game.instance.Attack(player.team, position, radius, damage);
    }
}
