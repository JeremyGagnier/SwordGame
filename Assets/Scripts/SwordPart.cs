using UnityEngine;
using System.Collections;

public class SwordPart : MonoBehaviour
{
    public int rarity;
    // TODO: Make parent and player set properly
    // BREAKS: Sword part functionality
    public SwordPart parent;
    public Player player;
    public FInt rotation;
    public FInt weight;
    public FInt damage;
    // TODO: Add radius property and fix sword part prefabs
    // BREAKS: Sword collisions
    public FInt radius;
    public Node[] nodePoints;
    public FInt depthInSword = FInt.Zero();
    public Node consumedNode;

    private FVector _position;
    public FVector position
    {
        get
        {
            // The sword part is the hilt (or on the ground) if there is no parent
            if (parent == null)
            {
                // The sword part is on the ground if there is no player
                if (player == null)
                {
                    return _position;
                }
                return player.position;
            }
            return _position + parent.position;
        }
        set
        {
            _position = value;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(position.x.ToFloat(), position.y.ToFloat()), radius.ToFloat());
    }

    void Awake()
    {
        for (int i = 0; i < nodePoints.Length; ++i)
        {
            nodePoints[i].parent = this;
        }
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
        transform.localPosition = new Vector3(position.x.ToFloat(), position.y.ToFloat());

        // TODO: Find depthInSword via recursive transformations
        // BREAKS: Proper sword construction
        /*
        Vector3 localrot = sword.transform.localEulerAngles;
        sword.transform.localEulerAngles = new Vector3(0, 0, 0);
        Vector3 diff = transform.position - sword.transform.position;
        truey = diff.y;
        sword.transform.localEulerAngles = localrot;
         */
    }

    public void Attack(World world)
    {
        world.Attack(player.team, position, radius, player.position, damage, FInt.Zero(), null);
    }
}
