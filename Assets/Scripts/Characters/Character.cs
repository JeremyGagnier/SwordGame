using UnityEngine;

public class Character : MonoBehaviour
{
    [HideInInspector] public FVector position = new FVector(FInt.Zero(), FInt.Zero());
    [HideInInspector] public int team;
    [HideInInspector] public FInt invincibility = FInt.Zero();
    public FInt radius;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(new Vector3(position.x.ToFloat(), position.y.ToFloat()), radius.ToFloat());
    }

    public virtual void Advance()
    {
        if (invincibility.rawValue > 0)
        {
            invincibility -= Game.TIMESTEP;
        }
        position = Game.instance.GetWrappedPosition(position);
        transform.position = new Vector3(position.x.ToFloat(), position.y.ToFloat());
    }

    public virtual void Damage(FInt damage)
    {

    }
}
